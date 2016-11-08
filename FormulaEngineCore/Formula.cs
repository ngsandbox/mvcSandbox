using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FormulaEngineCore.Creators;
using FormulaEngineCore.Elements;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operands;
using PerCederberg.Grammatica.Runtime;

namespace FormulaEngineCore
{
    /// <summary>
    ///     Represents a parsed formula
    /// </summary>
    /// <remarks>
    ///     Instances of this class are created by the formula engine after parsing an expression.  The class contains the
    ///     compiled
    ///     form of the given expression, exposes some useful properties, and allows you to evaluate the formula and format it.
    /// </remarks>
    [Serializable]
    public class Formula : ISerializable
    {
        // The compiled array of our operands and operators
        private const int VERSION = 1;
        private IFormulaComponent[] _components;
        // References used when we format
        private Reference[] _rawReferences;
        // References that this formula has
        // Properties we specific to each reference
        private ReferenceProperties[] _referenceProperties;

        private Formula()
        {
        }

        internal Formula(FormulaEngine owner, string expression)
        {
            Engine = owner;
            // By default, we evaluate to a primitive
            ResultType = OperandType.Primitive;
            // Get our analyzer, have it parse the expression, and get all our information from it
            var analyzer = (CustomFormulaAnalyzer)Engine.Parser.Analyzer;
            ParseTreeElement rootElement = Parse(expression);
            ReferenceParseInfo[] infos = analyzer.ReferenceInfos;
            ProcessParseInfos(infos);
            Template = CreateTemplateString(expression, infos);
            analyzer.ResetReferences();
            _components = CreateComponents(rootElement);
            ValidateComponents();
            ComputeRawReferenceHashCodes();
            ComputeDependencyReferences();
        }

        private Formula(SerializationInfo info, StreamingContext context)
        {
            Engine = (FormulaEngine)info.GetValue("Engine", typeof(FormulaEngine));
            _components = (IFormulaComponent[])info.GetValue("Components", typeof(IFormulaComponent[]));
            _rawReferences = (Reference[])info.GetValue("RawReferences", typeof(Reference[]));
            DependencyReferences = (Reference[])info.GetValue("DependencyReferences", typeof(Reference[]));
            SelfReference = (Reference)info.GetValue("SelfReference", typeof(Reference));
            Template = info.GetString("Template");
            _referenceProperties =
                (ReferenceProperties[])info.GetValue("ReferenceProperties", typeof(ReferenceProperties[]));
        }

        /// <summary>
        ///     Gets the engine that owns this formula
        /// </summary>
        /// <value>The engine that the current formula is bound to</value>
        /// <remarks>
        ///     All formulas are owned by a formula engine.  This property gets the engine that owns this particular formula.
        /// </remarks>
        public FormulaEngine Engine { get; private set; }

        internal Reference[] DependencyReferences { get; private set; }

        public ReferenceProperties[] ReferenceProperties
        {
            get { return _referenceProperties; }
        }

        /// <summary>
        ///     Gets all the references that this formula uses
        /// </summary>
        /// <value>An array with all the references of the formula</value>
        /// <remarks>
        ///     Formulas can reference other cells.  The formula engine analyzes each formula for its references so it can
        ///     determine dependencies.  This property returns all the references that a formula refers to.
        /// </remarks>
        /// <example>
        ///     A formula such as "=B2+C2" would return an array of two references to cells B2 and C2.  A formula like
        ///     "=cos(pi())" would return a zero length array since it does not reference any cells.
        /// </example>
        public IReference[] References
        {
            get
            {
                var arr = new IReference[_rawReferences.Length];
                Array.Copy(_rawReferences, arr, arr.Length);
                return arr;
            }
        }


        /// <summary>
        /// Template string for formatting
        /// </summary>
        private string Template { get; set; }


        /// <summary>
        ///     Gets the reference that this formula is bound to
        /// </summary>
        /// <value>The reference where this formula is located</value>
        /// <remarks>
        ///     All formulas are bound to a reference.  This property exposes the reference that this particular formula is bound
        ///     to.
        ///     <note>The value will be null if the formula hasn't been added to a formula engine</note>
        /// </remarks>
        //public IReference SelfReference { get; private set; }

        internal Reference SelfReference { get; private set; }

        /// <summary>
        ///     Gets or sets the data type of the formula's result
        /// </summary>
        /// <value>The data type of the result</value>
        /// <remarks>
        ///     This property gives you control over what the data type of this formula's result will be.
        ///     The default is for the formula to evaluate to a primitive.  The most common reason for changing this value is to
        ///     have the formula evaluate to a reference instead of the reference's value.  If the result of evaluating the formula
        ///     cannot be converted to the requested type, the formula will return a Value error.
        /// </remarks>
        public OperandType ResultType { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
            info.AddValue("Engine", Engine);
            info.AddValue("Components", _components);
            info.AddValue("RawReferences", _rawReferences);
            info.AddValue("DependencyReferences", DependencyReferences);
            info.AddValue("SelfReference", SelfReference);
            info.AddValue("Template", Template);
            info.AddValue("ReferenceProperties", _referenceProperties);
        }

        private IFormulaComponent[] CreateComponents(ParseTreeElement rootElement)
        {
            IList list = new ArrayList();
            rootElement.AddAsRPN(list);

            var arr = new IFormulaComponent[list.Count];
            list.CopyTo(arr, 0);
            return arr;
        }

        private void ValidateComponents()
        {
            foreach (IFormulaComponent component in _components)
            {
                component.Validate(Engine);
            }
        }

        /// <summary>
        ///     Create the template string that we use for formatting this formula.  It will have placeholders where all the
        ///     references will go.  This way, the formula's formatted value will be updated as our references change.
        /// </summary>
        private string CreateTemplateString(string expression, ReferenceParseInfo[] refInfos)
        {
            var sb = new StringBuilder(expression);
            int offset = 0;
            int ordinal = 0;

            // Create a place holder for each reference
            for (int i = 0; i <= _rawReferences.Length - 1; i++)
            {
                CharacterRange range = refInfos[i].Location;
                string placeHolder = "{" + ordinal + "}";
                range.First += offset;
                sb.Remove(range.First, range.Length);
                sb.Insert(range.First, placeHolder);
                offset += placeHolder.Length - range.Length;
                ordinal += 1;
            }

            return sb.ToString();
        }

        private void ProcessParseInfos(ReferenceParseInfo[] infos)
        {
            int len = infos.Length;
            _rawReferences = new Reference[len];
            _referenceProperties = new ReferenceProperties[len];

            for (int i = 0; i < len; i++)
            {
                ReferenceParseInfo info = infos[i];
                Reference @ref = info.Target;
                ReferenceParseProperties props = info.ParseProperties;
                @ref.SetEngine(Engine);
                @ref.ProcessParseProperties(props, Engine);
                _rawReferences[i] = @ref;
                _referenceProperties[i] = info.Properties;
            }
        }

        private void ComputeRawReferenceHashCodes()
        {
            for (int i = 0; i <= _rawReferences.Length - 1; i++)
            {
                Reference @ref = _rawReferences[i];
                @ref.ComputeHashCode();
            }
        }

        internal Formula Clone()
        {
            var cloneFormula = new Formula { Engine = Engine, Template = Template, ResultType = ResultType };

            CloneComponents(cloneFormula);
            cloneFormula.ComputeDependencyReferences();
            cloneFormula._referenceProperties = new ReferenceProperties[_referenceProperties.Length];
            CloneArray(_referenceProperties, cloneFormula._referenceProperties);

            return cloneFormula;
        }

        private void CloneComponents(Formula cloneFormula)
        {
            cloneFormula._components = new IFormulaComponent[_components.Length];
            cloneFormula._rawReferences = new Reference[_rawReferences.Length];
            int index = 0;

            // We clone all the components and make sure that the component and its corresponding raw reference 
            // are the same since this is how a new formula would be setup.
            for (int i = 0; i <= _components.Length - 1; i++)
            {
                IFormulaComponent component = _components[i];
                var cloneComponent = (IFormulaComponent)component.Clone();
                cloneFormula._components[i] = cloneComponent;
                var @ref = cloneComponent as Reference;
                if (@ref != null)
                {
                    cloneFormula._rawReferences[index] = @ref;
                    index += 1;
                }
            }
        }

        private void CloneArray(ReferenceProperties[] source, ReferenceProperties[] dest)
        {
            for (int i = 0; i <= source.Length - 1; i++)
            {
                dest[i] = (ReferenceProperties)(source[i]).Clone();
            }
        }

        /// <summary>
        ///     Evaluate and return the final operand on the stack
        /// </summary>
        internal IOperand EvaluateToOperand()
        {
            var state = new Stack();

            // Simply loop through each component and tell it to evaluate using the provided stack
            foreach (IFormulaComponent component in _components)
            {
                component.Evaluate(state, Engine);
            }

            // Get the final operand on the stack
            Debug.Assert(state.Count == 1, "incomplete stack");
            var result = state.Pop() as IOperand;

            // Get the real value based on our result type
            result = GetResultOperand(result);
            return result;
        }

        /// <summary>
        ///     Computes the result of the formula
        /// </summary>
        /// <returns>The result of evaluating the formula</returns>
        /// <remarks>
        ///     This method will compute the result of the current formula.  The formula engine will typically call this method
        ///     during
        ///     a recalculate but you are free to call it anytime you need the latest result of the formula.  Use this method
        ///     instead of the
        ///     formula engine's evaluate method if you have a static expression that you wish to evaluate many times
        /// </remarks>
        /// <example>
        ///     This example shows how you can create a formula and evaluate it.
        ///     <code>
        /// Dim engine As New FormulaEngine
        /// Dim f As Formula = engine.CreateFormula("=cos(pi())")
        /// ' result will contain the value -1 as a Double
        /// Dim result As Object = f.Evaluate()
        /// </code>
        /// </example>
        public object Evaluate()
        {
            IOperand result = EvaluateToOperand();
            return GetFinalValue(result);
        }

        /// <summary>
        ///     Get the value that we will expose to the outside
        /// </summary>
        private object GetFinalValue(IOperand op)
        {
            if (ReferenceEquals(op.GetType(), typeof(ErrorValueOperand)))
            {
                // We want to return error wrappers instead of error enums so that formatting will be automatic
                return ((ErrorValueOperand)op).ValueAsErrorWrapper;
            }
            return op.Value;
        }

        /// <summary>
        ///     Get the operand based on our result type
        /// </summary>
        private IOperand GetResultOperand(IOperand op)
        {
            op = op.Convert(ResultType) ?? new ErrorValueOperand(ErrorValueType.Value);
            return op;
        }

        internal void SetSelfReference(Reference @ref)
        {
            SelfReference = @ref;
        }

        internal void ClearSelfReference()
        {
            SelfReference = null;
        }

        private void ReplaceRawReference(int ordinal, Reference newRef)
        {
            Reference oldRef = _rawReferences[ordinal];
            int index = Array.IndexOf(_components, oldRef);
            _components[index] = newRef;
            _rawReferences[ordinal] = newRef;
        }

        /// <summary>
        ///     Compute the references that will be passed onto the dependency manager.
        /// </summary>
        private void ComputeDependencyReferences()
        {
            IList references = new ArrayList();

            foreach (IFormulaComponent c in _components)
            {
                c.EvaluateForDependencyReference(references, Engine);
            }

            DependencyReferences = GetUniqueValidDependencyReferences(references);
        }

        /// <summary>
        ///     Gets only unique and valid references from the computed dependency reference list
        /// </summary>
        private Reference[] GetUniqueValidDependencyReferences(IList refs)
        {
            IDictionary<Reference, Reference> seenReferences =
                new Dictionary<Reference, Reference>(new ReferenceEqualityComparer());

            foreach (Reference @ref in refs)
            {
                // Ignore invalid or duplicate references
                if (seenReferences.ContainsKey(@ref) == false & @ref.Valid)
                {
                    seenReferences.Add(@ref, @ref);
                }
            }

            var arr = new Reference[seenReferences.Keys.Count];
            seenReferences.Keys.CopyTo(arr, 0);
            return arr;
        }

        internal void SetDependencyReferences(Reference[] refs)
        {
            DependencyReferences = refs;
            foreach (Reference @ref in refs)
            {
                ReplaceRawReferences(@ref);
            }
        }

        internal Reference[] GetDependencyReferences()
        {
            return DependencyReferences;
        }

        private void ReplaceRawReferences(Reference target)
        {
            for (int i = 0; i <= _rawReferences.Length - 1; i++)
            {
                Reference @ref = _rawReferences[i];
                if (@ref.EqualsReference(target))
                {
                    ReplaceRawReference(i, target);
                }
            }
        }

        /// <summary>
        ///     Parse an expression and return the root of the parse tree
        /// </summary>
        private ParseTreeElement Parse(string formula)
        {
            var sr = new StringReader(formula);
            Engine.Parser.Reset(sr);
            try
            {
                Node result = Engine.Parser.Parse();
                var element = (ParseTreeElement)result.Values[0];
                return element;
            }
            catch (ParserLogException ex)
            {
                OnParseException();
                throw new InvalidFormulaException(ex);
            }
            catch (OverflowException ex)
            {
                OnParseException();
                // An number value is too big/small to fit into its type
                throw new InvalidFormulaException(ex);
            }
        }

        private void OnParseException()
        {
            var analyzer = (CustomFormulaAnalyzer)Engine.Parser.Analyzer;
            analyzer.ResetReferences();
        }

        /// <summary>
        ///     Offset all our references for a copy operation
        /// </summary>
        internal void OffsetReferencesForCopy(ISheet destSheet, int rowOffset, int columnOffset)
        {
            if (SelfReference != null)
            {
                throw new InvalidOperationException("Cannot offset references while formula is in engine");
            }
            for (int i = 0; i <= _rawReferences.Length - 1; i++)
            {
                // Ask each reference to offset itself and pass it its appropriate reference properties
                Reference @ref = _rawReferences[i];
                @ref.OnCopy(rowOffset, columnOffset, destSheet, _referenceProperties[i]);
                @ref.ComputeHashCode();
            }

            ComputeDependencyReferences();
        }

        internal void GetReferenceProperties(Reference target, IList dest)
        {
            for (int i = 0; i <= _rawReferences.Length - 1; i++)
            {
                Reference @ref = _rawReferences[i];
                if (ReferenceEquals(target, @ref) | ReferenceEquals(target, SelfReference))
                {
                    ReferenceProperties props = _referenceProperties[i];
                    dest.Add(props);
                }
            }
        }

        /// <summary>
        ///     Returns a string representation of the formula
        /// </summary>
        /// <returns>A string representing the formatted value of the formula</returns>
        /// <remarks>
        ///     This value will be the same as the expression that the formula was created from except that the text for all
        ///     references
        ///     is dynamically updated as the references change.
        /// </remarks>
        public override string ToString()
        {
            var refStrings = new object[_rawReferences.Length];

            for (int i = 0; i < _rawReferences.Length; i++)
            {
                Reference @ref = _rawReferences[i];
                refStrings[i] = @ref.ToStringFormula(_referenceProperties[i]);
            }

            return string.Format(Template, refStrings);
        }
    }
}
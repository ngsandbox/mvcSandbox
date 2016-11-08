using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using FormulaEngineCore.Creators;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operators;

namespace FormulaEngineCore
{
    /// <summary>
    ///     Implements all the functionality of a formula engine
    /// </summary>
    /// <remarks>
    ///     This is the main class of this library.  It is responsible for managing formulas, their dependencies, and
    ///     all recalculations.  You can think of this class as a container for formulas.  It has methods for adding and
    ///     removing formulas and
    ///     methods for recalculating all formulas that depend on a given reference.
    /// </remarks>
    [Serializable]
    public class FormulaEngine : ISerializable
    {
        private const int VERSION = 1;
        private readonly IDictionary _referenceFormulaMap;

        public FormulaEngine(FormulaParserSettings settings = null)
        {
            _referenceFormulaMap = new Hashtable();
            FunctionLibrary = new FunctionLibrary(this);
            DependencyManager = new DependencyManager(this);
            ReferenceFactory = new ReferenceFactory(this);
            ReferencePool = new ReferencePool(this);
            Sheets = new SheetCollection(this);
            Info = new FormulaEngineInfo(this);
            Settings = settings ?? new FormulaParserSettings();
            Parser = CreateParser();
        }

        private FormulaEngine(SerializationInfo info, StreamingContext context)
        {
            DependencyManager = (DependencyManager)info.GetValue("DependencyManager", typeof(DependencyManager));
            ReferencePool = (ReferencePool)info.GetValue("ReferencePool", typeof(ReferencePool));
            _referenceFormulaMap = (IDictionary)info.GetValue("ReferenceFormulaMap", typeof(IDictionary));
            Sheets = (SheetCollection)info.GetValue("SheetManager", typeof(SheetCollection));
            FunctionLibrary = (FunctionLibrary)info.GetValue("FunctionLibrary", typeof(FunctionLibrary));
            Info = new FormulaEngineInfo(this);
            ReferenceFactory = new ReferenceFactory(this);
            Settings = new FormulaParserSettings();
            Parser = CreateParser();
        }

        private FormulaParser CreateParser()
        {
            var sr = new StringReader(string.Empty);
            var analyzer = new CustomFormulaAnalyzer();
            return new FormulaParser(sr, Settings, analyzer);
        }

        /// <summary>
        ///     Recreates the parser used to parse formulas
        /// </summary>
        /// <remarks>
        ///     The parser used to parse formulas is cached and shared amongst all formulas to improve performance.  Calling this
        ///     method will
        ///     force the creation of a new instance of the parser.  You typically will not need to
        ///     call this method unless you are switching cultures at run-time.
        /// </remarks>
        public void ResetParser()
        {
            Parser = CreateParser();
        }
        public FormulaParserSettings Settings { get; private set; }

        public FormulaParser Parser { get; private set; }

        /// <summary>
        ///     Gets the function library the engine is using
        /// </summary>
        /// <value>An instance of FunctionLibrary</value>
        /// <remarks>This property lets you access the engine's function library</remarks>
        public FunctionLibrary FunctionLibrary { get; private set; }

        internal DependencyManager DependencyManager { get; private set; }

        /// <summary>
        ///     Gets the number of formulas this engine contains
        /// </summary>
        /// <value>A count of all the formulas</value>
        /// <remarks>Use this property when you need to know how many formulas are contained in the engine</remarks>
        public int FormulaCount { get { return _referenceFormulaMap.Count; } }

        /// <summary>Gets the engine's ReferenceFactory instance</summary>
        /// <value>The reference factory of the engine</value>
        /// <remarks>This property lets you access this engine's reference factory</remarks>
        public ReferenceFactory ReferenceFactory { get; private set; }

        internal ReferencePool ReferencePool { get; private set; }

        /// <summary>Gets the engine's SheetCollection instance</summary>
        /// <value>The SheetCollection of engine</value>
        /// <remarks>Use this property when you need to access the engine's SheetCollection</remarks>
        public SheetCollection Sheets { get; private set; }

        /// <summary>
        ///     Gets the engine's FormulaEngineInfo instance
        /// </summary>
        /// <value>An instance of the FormulaEngineInfo class</value>
        /// <remarks>This property lets you access the engine's FormulaEngineInfo instance</remarks>
        public FormulaEngineInfo Info { get; private set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
            info.AddValue("DependencyManager", DependencyManager);
            info.AddValue("ReferencePool", ReferencePool);
            info.AddValue("ReferenceFormulaMap", _referenceFormulaMap);
            info.AddValue("SheetManager", Sheets);
            info.AddValue("FunctionLibrary", FunctionLibrary);
        }

        /// <summary>Notifies listeners that the engine has detected one or more circular references</summary>
        /// <remarks>
        ///     This event will get fired when the engine detects one or more circular references.  Circular references are allowed
        ///     by the engine but will be ignored during any recalculations.  You would typically listen to this event when you
        ///     want
        ///     to notify users that they have caused a circular reference.
        /// </remarks>
        public event EventHandler CircularReferenceDetected;

        /// <summary>
        ///     Creates a formula
        /// </summary>
        /// <param name="expression">The expression to parse</param>
        /// <returns>A formula representing the parsed expression</returns>
        /// <remarks>
        ///     This method is used to create instances of the formula class from an expression.  The returned instance is
        ///     not part of the formula engine; you must eplicitly add it.
        /// </remarks>
        /// <exception cref="T:ciloci.FormulaEngine.InvalidFormulaException">The formula could not be created</exception>
        public Formula CreateFormula(string expression)
        {
            ValidateNonNull(expression, "expression");
            var f = new Formula(this, expression);
            return f;
        }

        /// <summary>
        ///     Evaluates an expression
        /// </summary>
        /// <param name="expression">The expression to evaluate</param>
        /// <returns>The result of evaluating the expression</returns>
        /// <remarks>
        ///     You can use this function to evaluate an expression without creating a formula.  If you plan on evaluating the same
        ///     expression many times, it is more efficient to create a formula from it first and then call its
        ///     <see cref="M:ciloci.FormulaEngine.Formula.Evaluate" /> method as many times as you need.
        /// </remarks>
        public object Evaluate(string expression)
        {
            ValidateNonNull(expression, "expression");
            Formula f = CreateFormula(expression);
            object result = f.Evaluate();
            return result;
        }

        /// <overloads>Adds a formula to the engine</overloads>
        /// <summary>
        ///     Adds a formula parsed from an expression into the engine
        /// </summary>
        /// <param name="expression">The expression to create the formula from</param>
        /// <param name="Ref">The reference the formula will be bound to</param>
        /// <returns>The added formula</returns>
        /// <remarks>
        ///     This method does the same thing as
        ///     <see
        ///         cref="M:ciloci.FormulaEngine.FormulaEngine.AddFormula(ciloci.FormulaEngine.Formula,ciloci.FormulaEngine.IReference)" />
        ///     except that takes an expression instead of a formula.
        /// </remarks>
        public Formula AddFormula(string expression, IReference Ref)
        {
            Formula target = CreateFormula(expression);
            AddFormula(target, Ref);
            return target;
        }

        /// <summary>
        ///     Adds a formula to the formula engine
        /// </summary>
        /// <param name="target">The formula instance to add</param>
        /// <param name="ref">The reference the formula will be bound to</param>
        /// <remarks>
        ///     Use this method when you want to add a formula to the engine.  The engine will bind the formula to the given
        ///     reference
        ///     and analyze its dependencies.  Currently, formulas can only be bound to cell, named, and external references.
        /// </remarks>
        /// <exception cref="System.ArgumentException">
        ///     <list type="bullet">
        ///         <item>The given formula was not created by this engine</item>
        ///         <item>The formula cannot be bound to the type of reference given</item>
        ///         <item>There is already a formula bound to the given reference</item>
        ///     </list>
        /// </exception>
        /// <exception cref="System.ArgumentNullException">target or ref is null</exception>
        public void AddFormula(Formula target, IReference @ref)
        {
            ValidateNonNull(@ref, "ref");
            Reference selfRef = ReferencePool.GetReference((Reference)@ref);
            ValidateAddFormula(target, selfRef);

            SetFormulaDependencyReferences(target);
            target.SetSelfReference(selfRef);

            DependencyManager.AddFormula(target);
            _referenceFormulaMap.Add(selfRef, target);

            CheckFormulaCircularReference(target);
            NotifySelfReference(target);
        }

        private void ValidateAddFormula(Formula target, Reference selfRef)
        {
            if (!(selfRef is IFormulaSelfReference))
            {
                throw new ArgumentException("A formula can not be bound to this type of reference");
            }

            ValidateNonNull(target, "target");
            ValidateFormulaOwner(target);
            if (_referenceFormulaMap.Contains(selfRef))
            {
                throw new ArgumentException(String.Format("A formula already exists at reference {0}", selfRef.ToStringIReference()));
            }
        }

        internal static void ValidateNonNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        private void ValidateFormulaOwner(Formula target)
        {
            if (target != null && !ReferenceEquals(target.Engine, this))
            {
                throw new ArgumentException("The formula is not owned by this engine");
            }
        }

        /// <summary>
        ///     Removes a formula from the engine
        /// </summary>
        /// <param name="target">The formula instance to remove</param>
        /// <remarks>This method removes a formula from the engine and removes the formula's references from its dependency graph.</remarks>
        /// <exception cref="System.ArgumentException">
        ///     <list type="bullet">
        ///         <item>The given formula was not created by this engine</item>
        ///         <item>The formula is not contained in the engine</item>
        ///     </list>
        /// </exception>
        /// <exception cref="System.ArgumentNullException">The given formula is null</exception>
        public void RemoveFormula(Formula target)
        {
            ValidateRemoveFormula(target);
            Reference selfRef = target.SelfReference;

            DependencyManager.RemoveFormula(target);
            RemoveReferences(target.DependencyReferences);

            Debug.Assert(_referenceFormulaMap.Contains(target.SelfReference), "formula self ref not in map");
            _referenceFormulaMap.Remove(selfRef);
            ReferencePool.RemoveReference(selfRef);
            target.ClearSelfReference();
        }

        private void ValidateRemoveFormula(Formula target)
        {
            ValidateNonNull(target, "target");
            ValidateFormulaOwner(target);

            if (target.SelfReference == null)
            {
                throw new ArgumentException("The formula is contained in this formula engine");
            }
        }

        /// <summary>
        ///     Copies and adjusts a formula on a sheet
        /// </summary>
        /// <param name="source">The formula to copy</param>
        /// <param name="destRef">The destination of the copied formula</param>
        /// <remarks>
        ///     This method is used when wishing to implement copying of formulas similarly to Excel.  It makes a copy of the
        ///     source formula,
        ///     offsets its references by the difference between destRef and source's current location, and adds the copy to the
        ///     engine.
        ///     The engine will only adjust references marked as relative in the copied formula.
        /// </remarks>
        /// <exception cref="System.ArgumentException">The source formula is not bound to a sheet reference</exception>
        public void CopySheetFormula(Formula source, ISheetReference destRef)
        {
            ValidateNonNull(source, "source");
            ValidateNonNull(destRef, "destRef");
            if (!(source.SelfReference is SheetReference))
            {
                throw new ArgumentException("Source formula must be on a sheet");
            }
            var sourceRef = (SheetReference)source.SelfReference;
            int rowOffset = destRef.Row - sourceRef.Row;
            int colOffset = destRef.Column - sourceRef.Column;
            Formula clone = source.Clone();
            clone.OffsetReferencesForCopy(destRef.Sheet, rowOffset, colOffset);
            AddFormula(clone, destRef);
        }

        private void CheckFormulaCircularReference(Formula f)
        {
            if (DependencyManager.FormulaHasCircularReference(f))
            {
                OnFormulaCircularReference(f);
            }
        }

        /// <summary>
        ///     Set the dependency references of a formula to their pooled equivalents
        /// </summary>
        private void SetFormulaDependencyReferences(Formula target)
        {
            Reference[] depRefs = target.GetDependencyReferences();

            for (int i = 0; i <= depRefs.Length - 1; i++)
            {
                Reference @ref = depRefs[i];
                @ref = ReferencePool.GetReference(@ref);
                depRefs[i] = @ref;
            }

            target.SetDependencyReferences(depRefs);
        }

        internal void RemoveReferences(Reference[] refs)
        {
            foreach (Reference @ref in refs)
            {
                ReferencePool.RemoveReference(@ref);
            }
        }

        private void OnFormulaCircularReference(Formula target)
        {
            var list = new ArrayList { target.SelfReference };
            OnCircularReferenceDetected(list);
        }

        internal void OnCircularReferenceDetected(IList roots)
        {
            var arr = new IReference[roots.Count];
            roots.CopyTo(arr, 0);
            if (CircularReferenceDetected != null)
            {
                CircularReferenceDetected.Invoke(this, new CircularReferenceDetectedEventArgs(arr));
            }
        }

        /// <summary>
        ///     Recalculates all formulas that depend on a reference
        /// </summary>
        /// <param name="root">The reference whose dependents will be recalculated</param>
        /// <remarks>
        ///     This is the method that controls all recalculation in the formula engine.  Given a root reference, it will find
        ///     all formulas that depend on it and recalculate them in natural order.  If no formulas depend on the given reference
        ///     then the
        ///     method does nothing.
        /// </remarks>
        public void Recalculate(IReference root)
        {
            ValidateNonNull(root, "root");
            DoRecalculate(DependencyManager.GetReferenceCalculationList((Reference)root));
        }

        internal void DoRecalculate(Reference[] calculationList)
        {
            Formula[] formulas = GetFormulasFromReferences(calculationList);
            NotifyRecalculate(formulas);
        }

        internal Formula[] GetFormulasFromReferences(Reference[] references)
        {
            var arr = new Formula[references.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                Reference @ref = references[i];
                Debug.Assert(_referenceFormulaMap.Contains(@ref), "reference not mapped to formula");
                arr[i] = (Formula)_referenceFormulaMap[@ref];
            }

            return arr;
        }

        private void NotifyRecalculate(Formula[] calculationList)
        {
            for (int i = 0; i <= calculationList.Length - 1; i++)
            {
                Formula f = calculationList[i];
                NotifySelfReference(f);
            }
        }

        private void NotifySelfReference(Formula target)
        {
            ((IFormulaSelfReference)target.SelfReference).OnFormulaRecalculate(target);
        }

        /// <summary>
        ///     Determines if the engine has a formula bound to a particular reference
        /// </summary>
        /// <param name="ref">The reference to test</param>
        /// <returns>True is there is a formula bound to the reference; False if there is not</returns>
        /// <remarks>Use this formula to test if the engine has a formula bound to a given reference</remarks>
        public bool HasFormulaAt(IReference @ref)
        {
            return GetFormulaAt(@ref) != null;
        }

        /// <summary>
        ///     Gets the formula bound to a particular reference
        /// </summary>
        /// <param name="ref">The reference to find a formula for</param>
        /// <returns>The formula bound to the reference; null if no formula is bound to ref</returns>
        /// <remarks>Use this method when you have a reference and need to get the formula that is bound to it</remarks>
        public Formula GetFormulaAt(IReference @ref)
        {
            ValidateNonNull(@ref, "ref");
            Reference realRef = ReferencePool.GetPooledReference((Reference)@ref);
            if (realRef == null)
            {
                return null;
            }
            return (Formula)_referenceFormulaMap[realRef];
        }

        /// <summary>
        ///     Removes a formula bound to a particular reference
        /// </summary>
        /// <param name="ref">The reference whose formula to remove</param>
        /// <remarks>This method will remove the formula bound to a particular reference</remarks>
        /// <exception cref="System.ArgumentException">No formula is bound to ref</exception>
        public void RemoveFormulaAt(IReference @ref)
        {
            Formula f = GetFormulaAt(@ref);

            if (f == null)
            {
                throw new ArgumentException("No formula exists at given reference");
            }
            RemoveFormula(f);
        }

        /// <summary>
        ///     Removes all sheet formulas in a given range
        /// </summary>
        /// <param name="range">The range to clear formulas in</param>
        /// <remarks>
        ///     This method is used when you need to clear all sheet formulas in a given range.  All formulas bound to references
        ///     on the same sheet as range and that intersect its area will be removed from the engine.
        /// </remarks>
        public void RemoveFormulasInRange(ISheetReference range)
        {
            ValidateNonNull(range, "range");
            var realRange = (SheetReference)range;
            IList toRemove = new ArrayList();

            foreach (Reference selfRef in _referenceFormulaMap.Keys)
            {
                if (realRange.Intersects(selfRef))
                {
                    toRemove.Add(_referenceFormulaMap[selfRef]);
                }
            }

            RemoveFormulas(toRemove);
        }

        /// <summary>
        ///     Remove many formulas in a more efficient manner
        /// </summary>
        private void RemoveFormulas(IList formulas)
        {
            // Tell the dependency manager to suspend range link calculation
            DependencyManager.RemoveRangeLinks();
            DependencyManager.SetSuspendRangeLinks(true);

            // Remove all the formulas
            foreach (Formula f in formulas)
            {
                RemoveFormula(f);
            }

            // Resume range link calculation
            DependencyManager.SetSuspendRangeLinks(false);
            DependencyManager.AddRangeLinks();
        }

        /// <summary>
        ///     Resets the formula engine to an empty state
        /// </summary>
        /// <remarks>
        ///     Use this function when you need to reset the formula engine to an empty state.  This function will remove all:
        ///     formulas, dependencies,
        ///     references, and sheets from the engine.  It will <b>not</b> clear the function library.  You should call that
        ///     class'
        ///     <see cref="M:ciloci.FormulaEngine.FunctionLibrary.Clear(System.Boolean)" /> method if you also want to remove all
        ///     formulas.
        /// </remarks>
        public void Clear()
        {
            // Clear everything but functions
            DependencyManager.Clear();
            ReferencePool.Clear();
            _referenceFormulaMap.Clear();
            Sheets.Clear();
        }

        /// <summary>
        ///     Notifies the engine that columns have been inserted on the active sheet
        /// </summary>
        /// <param name="insertAt">The index of the first inserted column</param>
        /// <param name="count">The number of columns inserted</param>
        /// <remarks>
        ///     Use this method to notify the engine that columns have been inserted on the active sheet.  The engine will update
        ///     all references as necessary.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <list type="bullet">
        ///         <item>insertAt is less than 1</item>
        ///         <item>count is negative</item>
        ///     </list>
        /// </exception>
        public void OnColumnsInserted(int insertAt, int count)
        {
            ValidateHeaderOpArgs(insertAt, count);
            DoActiveSheetReferenceOperation(new ColumnsInsertedOperator(insertAt, count));
        }

        /// <summary>
        ///     Notifies the engine that columns have been removed on the active sheet
        /// </summary>
        /// <param name="removeAt">The index of the first removed column</param>
        /// <param name="count">The number of removed columns</param>
        /// <remarks>
        ///     Use this method to notify the engine that columns have been removed on the active sheet.  The engine will
        ///     invalidate
        ///     all references in the removed area, recalculate any formulas that depend on those references, and remove all
        ///     formulas in the area.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <list type="bullet">
        ///         <item>removeAt is less than 1</item>
        ///         <item>count is negative</item>
        ///     </list>
        /// </exception>
        public void OnColumnsRemoved(int removeAt, int count)
        {
            ValidateHeaderOpArgs(removeAt, count);
            DoActiveSheetReferenceOperation(new ColumnsRemovedOperator(removeAt, count));
        }

        /// <summary>
        ///     Notifies the engine that rows have been inserted on the active sheet
        /// </summary>
        /// <param name="insertAt">The index of the first inserted row</param>
        /// <param name="count">The number of rows inserted</param>
        /// <remarks>
        ///     Use this method to notify the engine that rows have been inserted on the active sheet.  The engine will update
        ///     all references as necessary.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <list type="bullet">
        ///         <item>insertAt is less than 1</item>
        ///         <item>count is negative</item>
        ///     </list>
        /// </exception>
        public void OnRowsInserted(int insertAt, int count)
        {
            ValidateHeaderOpArgs(insertAt, count);
            DoActiveSheetReferenceOperation(new RowsInsertedOperator(insertAt, count));
        }

        /// <summary>
        ///     Notifies the engine that rows have been removed on the active sheet
        /// </summary>
        /// <param name="removeAt">The index of the first removed row</param>
        /// <param name="count">The number of removed rows</param>
        /// <remarks>
        ///     Use this method to notify the engine that rows have been removed on the active sheet.  The engine will invalidate
        ///     all references in the removed area, recalculate any formulas that depend on those references, and remove all
        ///     formulas in the area.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <list type="bullet">
        ///         <item>removeAt is less than 1</item>
        ///         <item>count is negative</item>
        ///     </list>
        /// </exception>
        public void OnRowsRemoved(int removeAt, int count)
        {
            ValidateHeaderOpArgs(removeAt, count);
            DoActiveSheetReferenceOperation(new RowsRemovedOperator(removeAt, count));
        }

        /// <summary>
        ///     Notifies the engine that a range has moved
        /// </summary>
        /// <param name="range">The range that has moved</param>
        /// <param name="rowOffset">The number of rows range has moved.  Can be negative.</param>
        /// <param name="colOffset">The number of columns the range has moved.  Can be negative.</param>
        /// <remarks>
        ///     Use this method to notify the engine that a range on a sheet has moved.  The engine will update all references
        ///     in, or that depend on, the moved range accordingly.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The given range, when offset by the given offsets, is not within the bounds
        ///     of the active sheet
        /// </exception>
        public void OnRangeMoved(ISheetReference range, int rowOffset, int colOffset)
        {
            ValidateNonNull(range, "range");
            var sourceRef = (SheetReference)range;

            Rectangle destRect = range.Area;
            destRect.Offset(colOffset, rowOffset);
            var destRef = (SheetReference)ReferenceFactory.FromRectangle(destRect);

            ReferencePredicateBase pred;
            if (ReferenceEquals(sourceRef.Sheet, destRef.Sheet))
            {
                pred = new SheetReferencePredicate(Sheets.ActiveSheet);
            }
            else
            {
                pred = new CrossSheetReferencePredicate(sourceRef.Sheet, destRef.Sheet);
            }

            DoReferenceOperation(new RangeMovedOperator(this, sourceRef, destRef), pred);
        }

        internal void OnSheetRemoved(ISheet sheet)
        {
            DoSheetReferenceOperation(new SheetRemovedOperator(), sheet);
        }

        private void DoActiveSheetReferenceOperation(ReferenceOperator refOp)
        {
            DoSheetReferenceOperation(refOp, Sheets.ActiveSheet);
        }

        private void DoSheetReferenceOperation(ReferenceOperator refOp, ISheet sheet)
        {
            DoReferenceOperation(refOp, new SheetReferencePredicate(sheet));
        }

        private void DoReferenceOperation(ReferenceOperator refOp, ReferencePredicateBase predicate)
        {
            IList targets = ReferencePool.FindReferences(predicate);
            ReferencePool.DoReferenceOperation(targets, refOp);
        }

        internal void RecalculateAffectedReferences(IList affected)
        {
            IList sources = DependencyManager.GetSources(affected);

            Reference[] calcList = DependencyManager.GetCalculationList(sources);
            DoRecalculate(calcList);
        }

        internal void RemoveInvalidFormulas(IList invalidRefs)
        {
            foreach (Reference @ref in invalidRefs)
            {
                var f = (Formula)_referenceFormulaMap[@ref];
                if (f != null)
                {
                    RemoveFormula(f);
                }
            }
        }

        private void ValidateHeaderOpArgs(int location, int count)
        {
            if (location < 1)
            {
                throw new ArgumentOutOfRangeException("location", "Value must be greater than 0");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Value cannot be negative");
            }
        }

        /// <summary>
        ///     Creates an error wrapper around a specified error type
        /// </summary>
        /// <param name="errorType">The type of error to create a wrapper for</param>
        /// <returns>A wrapper around the error</returns>
        /// <remarks>This function lets you create an error wrapper around a specific error type</remarks>
        public static ErrorValueWrapper CreateError(ErrorValueType errorType)
        {
            return new ErrorValueWrapper(errorType);
        }

        /// <summary>
        ///     Gets all named references bound to formulas in the engine
        /// </summary>
        /// <returns>An array of named references</returns>
        /// <remarks>Use this function when you need to get all the named references bound to formulas in this engine.</remarks>
        public INamedReference[] GetNamedReferences()
        {
            IList found = new ArrayList();

            foreach (DictionaryEntry de in _referenceFormulaMap)
            {
                if (ReferenceEquals(de.Key.GetType(), typeof(NamedReference)))
                {
                    found.Add(de.Key);
                }
            }

            var arr = new INamedReference[found.Count];
            found.CopyTo(arr, 0);
            return arr;
        }

        /// <summary>
        ///     Defines a variable for use in formulas
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>A <see cref="T:ciloci.FormulaEngine.Variable" /> instance representing the newly defined variable</returns>
        /// <remarks>Use this method to define a new variable for use in formulas</remarks>
        public Variable DefineVariable(string name)
        {
            return new Variable(this, name);
        }

        internal IList GetReferenceProperties(Reference target)
        {
            IList found = new ArrayList();

            foreach (DictionaryEntry de in _referenceFormulaMap)
            {
                var f = (Formula)de.Value;
                f.GetReferenceProperties(target, found);
            }

            return found;
        }
    }
}
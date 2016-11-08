using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;
using FormulaEngineCore.Processors;
using FormulaEngineCore.Processors.Attributes;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Manages all functions that can be used in formulas
    /// </summary>
    /// <remarks>
    ///     This class is responsible for managing all functions used in formulas and for marshalling arguments during function
    ///     calls.
    ///     It has methods for defining and undefining formula functions in bulk or individually.  The library comes with many
    ///     of the most
    ///     common Excel functions already defined.  If a function is not defined in this class then an error will be generated
    ///     when
    ///     a formula tries to use it.
    ///     <para>
    ///         Adding your own function to the library requires the following steps:
    ///         <list type="bullet">
    ///             <item>
    ///                 Define a method that has the same signature as the
    ///                 <see cref="T:ciloci.FormulaEngine.FormulaFunctionCall">delegate</see>
    ///             </item>
    ///             <item>
    ///                 Tag the method with either the
    ///                 <see cref="T:ciloci.FormulaEngine.FixedArgumentFormulaFunctionAttribute" /> or
    ///                 <see cref="T:ciloci.FormulaEngine.VariableArgumentFormulaFunctionAttribute" /> attributes
    ///             </item>
    ///             <item>Add the function to the function library using one of the appropriate methods</item>
    ///         </list>
    ///     </para>
    ///     <note>
    ///         Function names are treated case-insensitively
    ///         <para>You cannot define/undefine functions while formulas are defined in the formula engine</para>
    ///     </note>
    /// </remarks>
    [Serializable]
    public class FunctionLibrary : ISerializable
    {
        /// <summary>The maximum number of arguments that any function can be called with</summary>
        /// <remarks>
        ///     This limit is arbitrary but is implemented to prevent functions being called with an unreasonable number of
        ///     arguments
        /// </remarks>
        public const int MAX_ARGUMENT_COUNT = 30;

        private const int VERSION = 1;
        private readonly BuiltinFunctions _builtinFunctions;
        private readonly IDictionary _functions;
        private readonly FormulaEngine _owner;

        internal FunctionLibrary(FormulaEngine owner)
        {
            _owner = owner;
            _builtinFunctions = new BuiltinFunctions();
            _functions = new Hashtable(StringComparer.OrdinalIgnoreCase);
            AddBuiltinFunctions();
        }

        private FunctionLibrary(SerializationInfo info, StreamingContext context)
        {
            _owner = (FormulaEngine)info.GetValue("Owner", typeof(FormulaEngine));
            _builtinFunctions = (BuiltinFunctions)info.GetValue("BuiltinFunctions", typeof(BuiltinFunctions));
            _functions = (IDictionary)info.GetValue("Functions", typeof(IDictionary));
        }

        /// <summary>
        ///     Gets the number of functions defined in the library
        /// </summary>
        /// <value>A count of the number of defined functions</value>
        /// <remarks>Use this property when you need to know the number of defined functions in the library</remarks>
        public int FunctionCount
        {
            get { return _functions.Keys.Count; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
            info.AddValue("Owner", _owner);
            info.AddValue("BuiltinFunctions", _builtinFunctions);
            info.AddValue("Functions", _functions);
        }

        /// <summary>
        ///     Adds all builtin functions to the library
        /// </summary>
        /// <remarks>
        ///     Use this method when you want to add all the builtin functions to the library.  Builtin functions are added by
        ///     default when the function library is created.
        /// </remarks>
        public void AddBuiltinFunctions()
        {
            AddInstanceFunctions(_builtinFunctions);
        }

        /// <summary>
        ///     Adds all instance methods of the given object that can be formula functions
        /// </summary>
        /// <param name="instance">The object whose methods you wish to add</param>
        /// <remarks>
        ///     Use this function when you want to add a large number of functions in bulk.  The method will search all instance
        ///     methods
        ///     of the type.  Methods that are tagged with a formula function attribute and have the correct signature will be
        ///     added to the library.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">instance is null</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     A method is tagged with a formula function attribute but does not have the
        ///     correct signature
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The function was called while formulas are defined in the formula engine
        ///     <para>A function with the same name is already defined</para>
        /// </exception>
        public void AddInstanceFunctions(object instance)
        {
            FormulaEngine.ValidateNonNull(instance, "instance");
            var creator = new InstanceDelegateCreator(instance);
            AddFormulaMethods(instance.GetType(), creator);
        }

        /// <summary>
        ///     Adds all static methods of the given type that can be formula functions
        /// </summary>
        /// <param name="target">The type to examine</param>
        /// <remarks>
        ///     This method works similarly to
        ///     <see cref="M:ciloci.FormulaEngine.FunctionLibrary.AddInstanceFunctions(System.Object)" /> except that it looks at
        ///     all static methods instead.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">target is null</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     A method is tagged with a formula function attribute but does not have the
        ///     correct signature
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The function was called while formulas are defined in the formula engine
        ///     <para>A function with the same name is already defined</para>
        /// </exception>
        public void AddStaticFunctions(Type target)
        {
            FormulaEngine.ValidateNonNull(target, "target");
            var creator = new StaticDelegateCreator(target);
            AddFormulaMethods(target, creator);
        }

        /// <summary>
        ///     Go through all methods of a type and try to add them as formula functions
        /// </summary>
        private void AddFormulaMethods(Type targetType, IDelegateCreator creator)
        {
            ValidateEngineStateForChangingFunctions();
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | creator.Flags;

            foreach (MethodInfo mi in targetType.GetMethods(flags))
            {
                var attr = (FormulaFunctionAttribute)Attribute.GetCustomAttribute(mi, typeof(FormulaFunctionAttribute));
                if (attr != null)
                {
                    FormulaFunctionCall d = creator.CreateDelegate(mi.Name);
                    if (d == null)
                    {
                        throw new ArgumentException(String.Format("The method {0} is marked as a formula function but does not have the correct signature", mi.Name));
                    }
                    var info = new FunctionInfo(d, attr);
                    AddFunctionInternal(info);
                }
            }
        }

        /// <summary>
        ///     Adds an individual formula function
        /// </summary>
        /// <param name="functionCall">A delegate pointing to the method you wish to add</param>
        /// <remarks>
        ///     This function lets you add an individual formula function by specifying a delegate pointing to it.  The method that
        ///     the delegate refers to must be tagged with the appropriate
        ///     <see cref="T:ciloci.FormulaEngine.FormulaFunctionAttribute">attribute</see>.
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">
        ///     The method that the delegate points to is not tagged with the required
        ///     attribute
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The function was called while formulas are defined in the formula engine
        ///     <para>A function with the same name is already defined</para>
        /// </exception>
        public void AddFunction(FormulaFunctionCall functionCall)
        {
            FormulaEngine.ValidateNonNull(functionCall, "functionCall");
            ValidateEngineStateForChangingFunctions();
            var attr =
                (FormulaFunctionAttribute)
                    Attribute.GetCustomAttribute(functionCall.Method, typeof(FormulaFunctionAttribute));

            if (attr == null)
            {
                throw new ArgumentException("The function does not have a FormulaFunctionAttribute defined on it");
            }

            var info = new FunctionInfo(functionCall, attr);
            AddFunctionInternal(info);
        }

        private void ValidateEngineStateForChangingFunctions()
        {
            if (_owner.FormulaCount > 0)
            {
                throw new InvalidOperationException("Cannot add or remove functions while formulas are defined");
            }
        }

        private void AddFunctionInternal(FunctionInfo info)
        {
            string methName = info.FunctionTarget.Method.Name;
            foreach (string name in info.Names)
            {
                if (_functions.Contains(name))
                {
                    throw new InvalidOperationException(String.Format("A function with the name {0} is already defined. Target method: {1}", name, methName));
                }

                _functions.Add(name, info);
            }
        }

        /// <summary>
        ///     Undefines an individual function
        /// </summary>
        /// <param name="functionName">The name of the function you wish to undefine</param>
        /// <remarks>This method removes a function from the library</remarks>
        /// <exception cref="System.ArgumentException">The given function name is not defined</exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The function was called while formulas are defined in the formula
        ///     engine
        /// </exception>
        public void RemoveFunction(string functionName)
        {
            FormulaEngine.ValidateNonNull(functionName, "functionName");
            ValidateEngineStateForChangingFunctions();
            if (_functions.Contains(functionName) == false)
            {
                throw new ArgumentException("That function is not defined");
            }
            _functions.Remove(functionName);
        }

        internal bool IsValidArgumentCount(string functionName, int argCount)
        {
            FunctionInfo info = GetFunctionInfo(functionName);
            FormulaFunctionAttribute attr = info.FunctionAttribute;
            return attr.IsValidMinArgCount(argCount) & attr.IsValidMaxArgCount(argCount);
        }

        internal bool IsFunctionDefined(string functionName)
        {
            return _functions.Contains(functionName);
        }

        /// <summary>
        ///     Perform a function call
        /// </summary>
        internal void InvokeFunction(string functionName, Stack state, int argumentCount)
        {
            var result = new FunctionResult();
            // Find the info for the function
            FunctionInfo info = GetFunctionInfo(functionName);
            // Get all the operands from teh stack
            IOperand[] ops = GetArguments(state, argumentCount);

            // Marshal the arguments
            IOperand resultOperand = MarshalArguments(info, ops);

            if (resultOperand == null)
            {
                // Marshaling succeeded so we can call the function
                Argument[] args = CreateArguments(ops);
                info.FunctionTarget(args, result, _owner);

                resultOperand = result.Operand;
                if (resultOperand == null)
                {
                    throw new InvalidOperationException(String.Format("Function {0} did not return a result", functionName));
                }
            }

            state.Push(resultOperand);
        }

        /// <summary>
        ///     Perform validation on operands passed to a function
        /// </summary>
        private ErrorValueOperand MarshalArguments(FunctionInfo info, IOperand[] ops)
        {
            FormulaFunctionAttribute attr = info.FunctionAttribute;

            for (int i = 0; i <= ops.Length - 1; i++)
            {
                IOperand op = ops[i];

                // We don't want invalid references
                if (IsInvalidReference(op))
                {
                    return new ErrorValueOperand(ErrorValueType.Ref);
                }

                ArgumentMarshalResult result = attr.MarshalArgument(i, op);

                if (result.Success == false)
                {
                    return (ErrorValueOperand)result.Result;
                }
                ops[i] = result.Result;
            }

            return null;
        }

        private bool IsInvalidReference(IOperand op)
        {
            var @ref = (Reference)op.Convert(OperandType.Reference);

            return @ref != null && @ref.Valid == false;
        }

        private Argument[] CreateArguments(IOperand[] ops)
        {
            var args = new Argument[ops.Length];

            for (int i = 0; i < ops.Length; i++)
            {
                args[i] = new Argument(ops[i]);
            }

            return args;
        }

        private IOperand[] GetArguments(Stack state, int argumentCount)
        {
            var arr = new IOperand[argumentCount];

            for (int i = 0; i < argumentCount; i++)
            {
                var op = (IOperand)state.Pop();
                arr[i] = op;
            }

            return arr;
        }

        private FunctionInfo GetFunctionInfo(string functionName)
        {
            var info = (FunctionInfo)_functions[functionName];
            Debug.Assert(info != null, "expected to find function");
            return info;
        }

        /// <summary>
        ///     Gets the names of all defined functions
        /// </summary>
        /// <returns>An array consisting of the names of all defined functions</returns>
        /// <remarks>Use this method when you need the names of all defined functions</remarks>
        public string[] GetFunctionNames()
        {
            var arr = new string[_functions.Keys.Count];
            _functions.Keys.CopyTo(arr, 0);
            return arr;
        }

        internal bool IsFunctionVolatile(string functionName)
        {
            FunctionInfo info = GetFunctionInfo(functionName);
            return info.Volatile;
        }

        /// <summary>
        ///     Undefines all functions
        /// </summary>
        /// <remarks>This method undefines all functions in the library</remarks>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The function was called while formulas are defined in the formula
        ///     engine
        /// </exception>
        public void Clear()
        {
            ValidateEngineStateForChangingFunctions();
            _functions.Clear();
        }

        /// <summary>
        ///     Stores all information about a function
        /// </summary>
        [Serializable]
        private class FunctionInfo
        {
            public readonly FormulaFunctionAttribute FunctionAttribute;
            public readonly FormulaFunctionCall FunctionTarget;
            public readonly string[] Names;
            public readonly bool Volatile;

            public FunctionInfo(FormulaFunctionCall target, FormulaFunctionAttribute attr)
            {
                FunctionTarget = target;
                FunctionAttribute = attr;
                string targetName = target.Method.Name;
                if (attr.Names != null && attr.Names.Length > 0)
                {
                    int addlNamesLen = attr.Names.Length;
                    Names = new string[addlNamesLen + 1];
                    Array.Copy(attr.Names, 0, Names, 1, addlNamesLen);
                }
                else
                {
                    Names = new string[1];
                }

                Names[0] = targetName;
                Volatile = attr.IsVolatile;
            }
        }

        private interface IDelegateCreator
        {
            BindingFlags Flags { get; }
            FormulaFunctionCall CreateDelegate(string methodName);
        }

        private class InstanceDelegateCreator : IDelegateCreator
        {
            private readonly object MyTarget;

            public InstanceDelegateCreator(object target)
            {
                MyTarget = target;
            }

            public FormulaFunctionCall CreateDelegate(string methodName)
            {
                return
                    (FormulaFunctionCall)
                        Delegate.CreateDelegate(typeof(FormulaFunctionCall), MyTarget, methodName, true, false);
            }

            public BindingFlags Flags
            {
                get { return BindingFlags.Instance; }
            }
        }

        private class StaticDelegateCreator : IDelegateCreator
        {
            private readonly Type MyTarget;

            public StaticDelegateCreator(Type target)
            {
                MyTarget = target;
            }

            public FormulaFunctionCall CreateDelegate(string methodName)
            {
                return
                    (FormulaFunctionCall)
                        Delegate.CreateDelegate(typeof(FormulaFunctionCall), MyTarget, methodName, true, false);
            }

            public BindingFlags Flags
            {
                get { return BindingFlags.Static; }
            }
        }
    }
}
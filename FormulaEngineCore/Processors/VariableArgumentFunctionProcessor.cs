// Various utilities that are required when implementing the builtin functions

using System;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    /// <summary>
    ///     Provides a framework for processing the arguments of a variable argument formula function
    /// </summary>
    /// <remarks>
    ///     This class provides a more reusable approach to processing the arguments of formula functions that
    ///     take a variable number of arguments.  Processing the arguments to such functions requires differentiating between
    ///     reference and primitive arguments, deciding how to handle error and null values, and even transforming values.
    ///     This class
    ///     handles the core processing of these tasks and lets derived classes handle the details specific to each function.
    /// </remarks>
    public abstract class VariableArgumentFunctionProcessor : IReferenceValueProcessor
    {
        private bool _processArgsFlag;

        protected VariableArgumentFunctionProcessor()
        {
            _processArgsFlag = true;
        }

        /// <summary>
        ///     Determines if processing stops upon encountering an error value
        /// </summary>
        /// <value>True if processing should stop when an error value is encountered; False to keep going</value>
        /// <remarks>
        ///     Some functions, like Sum, do not handle error values and need to stop at the first one they encounter.  Other
        ///     functions
        ///     like Count, simply ignore the value and keep going.  Derived classes must override this property so as to specify
        ///     their way
        ///     of handling error values.
        /// </remarks>
        protected abstract bool StopOnError { get; set; }

        /// <summary>
        ///     Gets the error that caused processing to fail
        /// </summary>
        /// <value>An error value</value>
        /// <remarks>
        ///     When processing of arguments fails, this property will indicate the specific error that is the cause.  The caller
        ///     can check
        ///     this property and set the result of the function accordingly.
        /// </remarks>
        public ErrorValueType ErrorValue { get; private set; }

        public bool ProcessValue(object value)
        {
            bool keepGoing;

            if (value == null)
            {
                ProcessEmptyValue();
                keepGoing = true;
            }
            else
            {
                keepGoing = ProcessNonEmptyValue(value);
            }

            KeepProcessingArguments(keepGoing);
            return keepGoing;
        }

        /// <summary>
        ///     Processes all arguments to a variable argument formula function
        /// </summary>
        /// <param name="args">The arguments to process</param>
        /// <returns>True if processing was sucessful; False otherwise</returns>
        /// <remarks>
        ///     This is the main method responsible for processing the function's arguments.  It handles the processing of
        ///     primitive arguments and the processing of each value of a reference argument.
        /// </remarks>
        public bool ProcessArguments(Argument[] args)
        {
            SortArguments(args);

            foreach (Argument arg in args)
            {
                ProcessArgument(arg);
                if (_processArgsFlag == false)
                {
                    return false;
                }
            }
            return true;
        }

        private void SortArguments(Argument[] args)
        {
            // Excel seems to process primitive arguments before reference arguments
            // Sort our arguments so that primitives come first
            var original = (Argument[]) args.Clone();
            Array.Sort(args, new ArgumentComparer(original));
        }

        private void ProcessArgument(Argument arg)
        {
            if (arg.IsReference == false)
            {
                bool keepGoing = ProcessPrimitiveArgumentInternal(arg);
                KeepProcessingArguments(keepGoing);
            }
            else
            {
                IReference @ref = arg.ValueAsReference;
                @ref.GetReferenceValues(this);
            }
        }

        private bool ProcessPrimitiveArgumentInternal(Argument arg)
        {
            if (StopOnError & arg.IsError)
            {
                SetError(arg.ValueAsError);
                return false;
            }
            return ProcessPrimitiveArgument(arg);
        }

        /// <summary>
        ///     Implemented by a derived class to handle processing of a primitive argument
        /// </summary>
        /// <param name="arg">The primitive argument to process</param>
        /// <returns>True if processing was successful; False otherwise</returns>
        /// <remarks>
        ///     This method will get called for each argument that is not a reference.  It is up to the derived class to decide
        ///     what
        ///     to do with each such argument.
        /// </remarks>
        protected abstract bool ProcessPrimitiveArgument(Argument arg);

        /// <summary>
        ///     Indicates how an empty reference value should be processed
        /// </summary>
        /// <remarks>
        ///     This method will get called for each value of a reference that is null.  Derived classes would override this method
        ///     if they need to handle null values in a special way.
        /// </remarks>
        protected virtual void ProcessEmptyValue()
        {
        }

        private bool ProcessNonEmptyValue(object value)
        {
            Type t = value.GetType();
            if (IsError(t))
            {
                return ProcessErrorValue((ErrorValueWrapper) value);
            }
            ProcessReferenceValue(value, t);
            return true;
        }

        private bool ProcessErrorValue(ErrorValueWrapper value)
        {
            OnErrorReferenceValue(value);

            if (StopOnError)
            {
                SetError(value.ErrorValue);
                return false;
            }
            // Ignore the error
            return true;
        }

        /// <summary>
        ///     Determines how a reference value that is an error should be handled
        /// </summary>
        /// <param name="value">The error value</param>
        /// <remarks>
        ///     This method will get called for each value of a reference that is an error value.  Derived classes can override
        ///     this method to provide custom handling for such values.
        /// </remarks>
        protected virtual void OnErrorReferenceValue(ErrorValueWrapper value)
        {
        }

        /// <summary>
        ///     Determines how a non-empty reference value will be processed
        /// </summary>
        /// <param name="value">The value to process</param>
        /// <param name="valueType">The value's type</param>
        /// <remarks>
        ///     Derived classes must override this method to provide customized handling of a reference's values.  This method only
        ///     deals
        ///     with non-empty values and thus the value parameter will never be null.
        /// </remarks>
        protected abstract void ProcessReferenceValue(object value, Type valueType);

        /// <summary>
        ///     Determines if a type represents an error
        /// </summary>
        /// <param name="t">The type to test</param>
        /// <returns>True is the type represents an error; False otherwise</returns>
        /// <remarks>This function is handy when you need to determine if the type of a value is an error.</remarks>
        protected bool IsError(Type t)
        {
            return ReferenceEquals(t, typeof (ErrorValueWrapper));
        }

        /// <summary>
        ///     Sets the error that will be reported at the end of processing
        /// </summary>
        /// <param name="errorType">The type of error to set</param>
        /// <remarks>
        ///     Derived classes can use this method when they encounter an error during argument processing.  When processing is
        ///     finished,
        ///     this value will be returned by the <see cref="P:FormulaEngineCore.Processors.VariableArgumentFunctionProcessor.ErrorValue" /> property to
        ///     callers who
        ///     need to know why processing failed.
        /// </remarks>
        protected void SetError(ErrorValueType errorType)
        {
            ErrorValue = errorType;
        }

        private void KeepProcessingArguments(bool process)
        {
            _processArgsFlag = process;
        }
    }
}
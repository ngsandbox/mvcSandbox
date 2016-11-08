using System;
using System.Collections;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    /// <summary>
    ///     An operator on two operands
    /// </summary>
    [Serializable]
    internal abstract class BinaryOperator : OperatorBase
    {
        protected BinaryOperator()
        {
        }

        protected BinaryOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected abstract OperandType ArgumentType { get; }

        public override void Evaluate(Stack state, FormulaEngine engine)
        {
            var rhs = (IOperand) state.Pop();
            var lhs = (IOperand) state.Pop();

            IOperand result;

            IOperand lhsConverted = lhs.Convert(ArgumentType);
            IOperand rhsConverted = rhs.Convert(ArgumentType);

            if (lhsConverted == null | rhsConverted == null)
            {
                result = GetConvertError(lhs, rhs);
            }
            else
            {
                result = ComputeValue(lhsConverted, rhsConverted, engine);
            }

            state.Push(result);
        }

        private IOperand GetConvertError(IOperand lhs, IOperand rhs)
        {
            IOperand errorOp = GetErrorOperand(lhs, rhs);

            if (errorOp != null)
            {
                return errorOp;
            }
            return new ErrorValueOperand(ErrorValueType.Value);
        }

        protected IOperand GetErrorOperand(IOperand lhs, IOperand rhs)
        {
            IOperand errorOp = lhs.Convert(OperandType.Error);

            if (errorOp != null)
            {
                return errorOp;
            }

            errorOp = rhs.Convert(OperandType.Error);

            if (errorOp != null)
            {
                return errorOp;
            }

            return null;
        }

        protected IOperand DoDoubleOperation(DoubleOperand lhs, DoubleOperand rhs, DoubleOperator @operator)
        {
            double result = @operator(lhs.ValueAsDouble, rhs.ValueAsDouble);

            if (IsInvalidDouble(result))
            {
                return new ErrorValueOperand(ErrorValueType.Num);
            }
            return new DoubleOperand(result);
        }

        protected abstract IOperand ComputeValue(IOperand lhs, IOperand rhs, FormulaEngine engine);

        protected delegate double DoubleOperator(double lhs, double rhs);
    }
}
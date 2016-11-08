using System;
using System.Collections;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    /// <summary>
    ///     Represents an operator on one operand
    /// </summary>
    [Serializable]
    internal abstract class UnaryOperator : OperatorBase
    {
        protected UnaryOperator()
        { 
        }

        protected UnaryOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void Evaluate(Stack state, FormulaEngine engine)
        {
            var op = (IOperand) state.Pop();

            IOperand result = ComputeValueInternal(op);
            state.Push(result);
        }

        private IOperand ComputeValueInternal(IOperand op)
        {
            OperandType knownType = GetKnownArgumentType();

            IOperand convertedOp = op.Convert(knownType);

            if (convertedOp == null)
            {
                return GetConvertError(op);
            }
            return ComputeValue(convertedOp);
        }

        private IOperand GetConvertError(IOperand op)
        {
            IOperand errorOp = op.Convert(OperandType.Error);

            if (errorOp != null)
            {
                return errorOp;
            }
            return new ErrorValueOperand(ErrorValueType.Value);
        }

        protected abstract OperandType GetKnownArgumentType();
        protected abstract IOperand ComputeValue(IOperand value);
    }
}
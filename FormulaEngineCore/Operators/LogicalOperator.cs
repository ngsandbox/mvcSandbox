using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class LogicalOperator : BinaryOperator
    {
        private readonly CompareType _compareType;

        public LogicalOperator(CompareType ct)
        {
            _compareType = ct;
        }

        private LogicalOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _compareType = (CompareType) info.GetInt32("CompareType");
        }

        protected override OperandType ArgumentType
        {
            get { return OperandType.Primitive; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CompareType", _compareType);
        }

        protected override IOperand ComputeValue(IOperand lhs, IOperand rhs, FormulaEngine engine)
        {
            IOperand errorOp = GetErrorOperand(lhs, rhs);

            if (errorOp != null)
            {
                return errorOp;
            }

            CompareResult cr = Compare(lhs, rhs);

            bool result = GetBooleanResult(_compareType, cr);
            return new BooleanOperand(result);
        }

        private CompareResult Compare(IOperand lhs, IOperand rhs)
        {
            OperandType[] convertTypes =
            {
                OperandType.Integer,
                OperandType.Double,
                OperandType.String,
                OperandType.Boolean,
                OperandType.DateTime
            };

            int commonTypeIndex = IndexOfCommonType((PrimitiveOperand) lhs, (PrimitiveOperand) rhs, convertTypes);

            if (commonTypeIndex == -1)
            {
                return CompareDifferentTypes((PrimitiveOperand) lhs, (PrimitiveOperand) rhs);
            }
            OperandType commonType = convertTypes[commonTypeIndex];
            var lhsPrim = (PrimitiveOperand) lhs.Convert(commonType);
            var rhsPrim = (PrimitiveOperand) rhs.Convert(commonType);
            int result = lhsPrim.Compare(rhsPrim);
            return Compare2CompareResult(result);
        }

        private int IndexOfCommonType(PrimitiveOperand lhs, PrimitiveOperand rhs, OperandType[] types)
        {
            for (int i = 0; i <= types.Length - 1; i++)
            {
                OperandType ot = types[i];
                if (lhs.CanConvertForCompare(ot) & rhs.CanConvertForCompare(ot))
                {
                    return i;
                }
            }
            return -1;
        }

        private CompareResult CompareDifferentTypes(PrimitiveOperand lhs, PrimitiveOperand rhs)
        {
            object lhsValue = lhs.Value;
            object rhsValue = rhs.Value;

            // Excel has weird rules when comparing values of different types.  Basically it assigns each type a rank and 
            // compares the rank of both values.
            int lhsRank = GetTypeRank(lhsValue.GetType());
            int rhsRank = GetTypeRank(rhsValue.GetType());
            return Compare2CompareResult(lhsRank.CompareTo(rhsRank));
        }

        private static int GetTypeRank(Type t)
        {
            if (ReferenceEquals(t, typeof (double)) | ReferenceEquals(t, typeof (int)))
            {
                return 1;
            }
            if (ReferenceEquals(t, typeof (string)))
            {
                return 2;
            }
            if (ReferenceEquals(t, typeof (bool)))
            {
                return 3;
            }
            if (ReferenceEquals(t, typeof (DateTime)))
            {
                return 4;
            }
            Debug.Assert(false, "unknown type");
            return 0;
        }

        public static bool GetBooleanResult(CompareType ct, CompareResult cr)
        {
            bool result = false;

            switch (ct)
            {
                case CompareType.Equal:
                    result = cr == CompareResult.Equal;
                    break;
                case CompareType.NotEqual:
                    result = cr != CompareResult.Equal;
                    break;
                case CompareType.LessThan:
                    result = cr == CompareResult.LessThan;
                    break;
                case CompareType.GreaterThan:
                    result = cr == CompareResult.GreaterThan;
                    break;
                case CompareType.LessThanOrEqual:
                    result = cr == CompareResult.LessThan | cr == CompareResult.Equal;
                    break;
                case CompareType.GreaterThanOrEqual:
                    result = cr == CompareResult.GreaterThan | cr == CompareResult.Equal;
                    break;
                default:
                    Debug.Assert(false, "unknown compare type");
                    break;
            }

            return result;
        }

        public static CompareResult Compare2CompareResult(int compare)
        {
            if (compare < 0)
            {
                return CompareResult.LessThan;
            }
            if (compare == 0)
            {
                return CompareResult.Equal;
            }
            return CompareResult.GreaterThan;
        }
    }
}
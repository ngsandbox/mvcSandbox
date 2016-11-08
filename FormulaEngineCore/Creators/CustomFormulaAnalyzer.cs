using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using FormulaEngineCore.Elements;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;
using FormulaEngineCore.Operators;
using PerCederberg.Grammatica.Runtime;

namespace FormulaEngineCore.Creators
{
    /// <summary>
    ///     Processes the parse tree and generates the information necessary to build a formula
    /// </summary>
    internal class CustomFormulaAnalyzer : FormulaAnalyzer
    {
        private readonly IList _referenceInfos;

        public CustomFormulaAnalyzer()
        {
            _referenceInfos = new ArrayList();
        }

        public ReferenceParseInfo[] ReferenceInfos
        {
            get
            {
                var arr = new ReferenceParseInfo[_referenceInfos.Count];
                _referenceInfos.CopyTo(arr, 0);
                return arr;
            }
        }

        public void ResetReferences()
        {
            _referenceInfos.Clear();
        }

        public override Node ExitFormula(Production node)
        {
            ArrayList childValues = GetChildValues(node);
            Debug.Assert(childValues.Count == 1 | childValues.Count == 2, "Should have 1 or 2 values");
            if (childValues.Count == 2)
            {
                // Remove leading EQ expression
                childValues.RemoveAt(0);
            }
            node.AddValues(childValues);
            return node;
        }

        public override Node ExitScalarFormula(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitPrimaryExpression(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitExpression(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitLogicalExpression(Production node)
        {
            AddBinaryOperation(node);
            return node;
        }

        public override Node ExitLogicalOp(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitConcatExpression(Production node)
        {
            AddBinaryOperation(node);
            return node;
        }

        public override Node ExitAdditiveExpression(Production node)
        {
            AddBinaryOperation(node);
            return node;
        }

        public override Node ExitAdditiveOp(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitMultiplicativeExpression(Production node)
        {
            AddBinaryOperation(node);
            return node;
        }

        public override Node ExitMultiplicativeOp(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitExponentiationExpression(Production node)
        {
            AddBinaryOperation(node);
            return node;
        }

        public override Node ExitPercentExpression(Production node)
        {
            IList values = GetChildValues(node);
            var first = (ParseTreeElement)values[0];

            if (values.Count > 1)
            {
                var pe = new PercentOperator(values.Count - 1);
                node.AddValue(new UnaryOperatorElement(pe, first));
            }
            else
            {
                node.AddValue(first);
            }

            return node;
        }

        public override Node ExitUnaryExpression(Production node)
        {
            IList values = GetChildValues(node);
            var last = (ParseTreeElement)values[values.Count - 1];

            int negCount = values.Cast<object>().Count(op => ReferenceEquals(op.GetType(), typeof(BinarySubOperator)));
            if ((negCount & 1) != 0)
            {
                var ne = new UnaryNegateOperator();
                node.AddValue(new UnaryOperatorElement(ne, last));
            }
            else
            {
                node.AddValue(last);
            }

            return node;
        }

        public override Node ExitBasicExpression(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitReference(Production node)
        {
            IList values = GetChildValues(node);
            var oe = new ContainerElement(values[0]);
            node.AddValue(oe);
            return node;
        }

        public override Node ExitExpressionGroup(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitFunctionCall(Production node)
        {
            var fe = new FunctionCallElement();
            fe.AcceptValues(GetChildValues(node));
            node.AddValue(fe);
            return node;
        }

        public override Node ExitArgumentList(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitPrimitive(Production node)
        {
            IList values = GetChildValues(node);
            var oe = new ContainerElement(values[0]);
            node.AddValue(oe);
            return node;
        }

        public override Node ExitAdd(Token node)
        {
            node.AddValue(new BinaryAddOperator());
            return node;
        }

        public override Node ExitSub(Token node)
        {
            node.AddValue(new BinarySubOperator());
            return node;
        }

        public override Node ExitMul(Token node)
        {
            node.AddValue(new BinaryMultiplyOperator());
            return node;
        }

        public override Node ExitDiv(Token node)
        {
            node.AddValue(new BinaryDivisionOperator());
            return node;
        }

        public override Node ExitConcat(Token node)
        {
            node.AddValue(new ConcatenationOperator());
            return node;
        }

        public override Node ExitExp(Token node)
        {
            node.AddValue(new BinaryPowerOperator());
            return node;
        }

        public override Node ExitEq(Token node)
        {
            node.AddValue(new LogicalOperator(CompareType.Equal));
            return node;
        }

        public override Node ExitNe(Token node)
        {
            node.AddValue(new LogicalOperator(CompareType.NotEqual));
            return node;
        }

        public override Node ExitLt(Token node)
        {
            node.AddValue(new LogicalOperator(CompareType.LessThan));
            return node;
        }

        public override Node ExitGt(Token node)
        {
            node.AddValue(new LogicalOperator(CompareType.GreaterThan));
            return node;
        }

        public override Node ExitLte(Token node)
        {
            node.AddValue(new LogicalOperator(CompareType.LessThanOrEqual));
            return node;
        }

        public override Node ExitGte(Token node)
        {
            node.AddValue(new LogicalOperator(CompareType.GreaterThanOrEqual));
            return node;
        }

        public override Node ExitPercent(Token node)
        {
            node.AddValue(node.Image);
            return node;
        }

        public override Node ExitNumber(Token node)
        {
            double value;
            PrimitiveOperand op;

            // Try to store the number as an integer if possible
            bool success = double.TryParse(node.Image, NumberStyles.Integer, null, out value);

            if (success)
            {
                if (value >= Int32.MinValue & value <= Int32.MaxValue)
                {
                    op = new IntegerOperand(Convert.ToInt32(value));
                }
                else
                {
                    op = new DoubleOperand(value);
                }
            }
            else
            {
                value = double.Parse(node.Image);
                op = new DoubleOperand(value);
            }

            node.AddValue(op);
            return node;
        }

        public override Node ExitStringLiteral(Token node)
        {
            string s = node.Image;
            // Remove first and last characters
            s = s.Substring(1, s.Length - 2);
            // Replace any double quotes with a single quote
            s = s.Replace("\"\"", "\"");
            node.AddValue(new StringOperand(s));
            return node;
        }

        public override Node ExitBoolean(Production node)
        {
            var b = (bool)node.GetChildAt(0).Values[0];
            node.AddValue(new BooleanOperand(b));
            return node;
        }

        public override Node ExitTrue(Token node)
        {
            node.AddValue(true);
            return node;
        }

        public override Node ExitFalse(Token node)
        {
            node.AddValue(false);
            return node;
        }

        public override Node ExitErrorExpression(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitDivError(Token node)
        {
            node.AddValue(new ErrorValueOperand(ErrorValueType.Div0));
            return node;
        }

        public override Node ExitNaError(Token node)
        {
            node.AddValue(new ErrorValueOperand(ErrorValueType.NA));
            return node;
        }

        public override Node ExitNameError(Token node)
        {
            node.AddValue(new ErrorValueOperand(ErrorValueType.Name));
            return node;
        }

        public override Node ExitNullError(Token node)
        {
            node.AddValue(new ErrorValueOperand(ErrorValueType.Null));
            return node;
        }

        public override Node ExitRefError(Token node)
        {
            node.AddValue(new ErrorValueOperand(ErrorValueType.Ref));
            return node;
        }

        public override Node ExitValueError(Token node)
        {
            node.AddValue(new ErrorValueOperand(ErrorValueType.Value));
            return node;
        }

        public override Node ExitNumError(Token node)
        {
            node.AddValue(new ErrorValueOperand(ErrorValueType.Num));
            return node;
        }

        public override Node ExitDefinedName(Token node)
        {
            node.AddValue(new NamedFormulaOperator(node.Image));
            return node;
        }

        public override Node ExitFunctionName(Token node)
        {
            string functionName = node.Image;
            // Remove trailing '('
            functionName = functionName.Remove(functionName.Length - 1, 1);
            node.AddValue(functionName);
            return node;
        }

        private void AddBinaryOperation(Production node)
        {
            if (node.GetChildCount() > 1)
            {
                var element = new BinaryOperatorElement();
                element.AcceptValues(GetChildValues(node));
                node.AddValue(element);
            }
            else
            {
                node.AddValues(GetChildValues(node));
            }
        }

        public override Node ExitCell(Token node)
        {
            var creator = new CellReferenceCreator();
            creator.Initialize(node.Image);
            node.AddValue(creator);
            return node;
        }

        public override Node ExitCellRange(Token node)
        {
            var creator = new CellRangeReferenceCreator();
            creator.Initialize(node.Image);
            node.AddValue(creator);
            return node;
        }

        public override Node ExitRowRange(Token node)
        {
            var creator = new RowReferenceCreator();
            creator.Initialize(node.Image);
            node.AddValue(creator);
            return node;
        }

        public override Node ExitColumnRange(Token node)
        {
            var creator = new ColumnReferenceCreator();
            creator.Initialize(node.Image);
            node.AddValue(creator);
            return node;
        }

        public override Node ExitGridReference(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        public override Node ExitGridReferenceExpression(Production node)
        {
            var cr = new CharacterRange();
            string sheetName = null;

            Node refNode = node.GetChildAt(node.GetChildCount() - 1);
            var creator = (ReferenceCreator)refNode.GetValue(0);
            cr.First = refNode.StartColumn - 1;
            cr.Length = refNode.EndColumn - refNode.StartColumn + 1;

            if (node.GetChildCount() == 2)
            {
                var sheetToken = (Token)node.GetChildAt(0);
                sheetName = sheetToken.Image.Substring(0, sheetToken.Image.Length - 1);
                cr.First = sheetToken.StartColumn - 1;
                cr.Length = refNode.EndColumn - sheetToken.StartColumn + 1;
            }

            Reference @ref = creator.CreateReference();
            var info = new ReferenceParseInfo
                       {
                           Target = @ref,
                           Location = cr,
                           Properties = creator.CreateProperties(node.GetChildCount() == 1),
                           ParseProperties = new ReferenceParseProperties { SheetName = sheetName }
                       };
            _referenceInfos.Add(info);

            node.AddValue(@ref);
            return node;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operators;
using FormulaEngineCore.Processors;
using FormulaEngineCore.Processors.Attributes;

namespace FormulaEngineCore
{
    /// <summary>
    ///     Implements all the functions that come standard with the formula engine
    /// </summary>
    [Serializable]
    internal class BuiltinFunctions
    {
        public const double TOLERANCE = 0.01;

        private const int MAX_FACTORIAL = 170;
        private readonly int[] _ractorialTable;

        private readonly Random _random;

        public BuiltinFunctions()
        {
            _ractorialTable = CreateFactorialTable();
            _random = new Random();
        }

        private int[] CreateFactorialTable()
        {
            var arr = new int[MAX_FACTORIAL];

            for (int i = 0; i < MAX_FACTORIAL; i++)
            {
                arr[i] = i + 1;
            }

            return arr;
        }

        #region "Statistical functions"

        [VariableArgumentFormulaFunction]
        public void Stdev(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeStandardDeviation(processor.Values, true, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void StdevA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new AverageAProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeStandardDeviation(processor.Values, true, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void StdevP(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeStandardDeviation(processor.Values, false, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void StdevPA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new AverageAProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeStandardDeviation(processor.Values, false, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void Var(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeVariance(processor.Values, true, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void VarA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new AverageAProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeVariance(processor.Values, true, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void VarP(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeVariance(processor.Values, false, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void VarPA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new AverageAProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeVariance(processor.Values, false, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void Max(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                double max__1 = ComputeMax(processor.Values);
                result.SetValue(max__1);
            }
        }

        [VariableArgumentFormulaFunction]
        public void MaxA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new AverageAProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                double max = ComputeMax(processor.Values);
                result.SetValue(max);
            }
        }

        [VariableArgumentFormulaFunction]
        public void Min(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                double min__1 = ComputeMin(processor.Values);
                result.SetValue(min__1);
            }
        }

        [VariableArgumentFormulaFunction]
        public void MinA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new AverageAProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                double min = ComputeMin(processor.Values);
                result.SetValue(min);
            }
        }

        [VariableArgumentFormulaFunction]
        public void Average(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            ComputeAverageFunction(args, result, new SumProcessor());
        }

        private void ComputeAverageFunction(Argument[] args, FunctionResult result,
            DoubleBasedReferenceValueProcessor processor)
        {
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                if (processor.Values.Count == 0)
                {
                    result.SetError(ErrorValueType.Div0);
                }
                else
                {
                    result.SetValue(ComputeAverage(processor.Values));
                }
            }
        }

        [VariableArgumentFormulaFunction]
        public void AverageA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            ComputeAverageFunction(args, result, new AverageAProcessor());
        }

        [VariableArgumentFormulaFunction]
        public void Count(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new CountProcessor();
            processor.ProcessArguments(args);
            result.SetValue(processor.Count);
        }

        [VariableArgumentFormulaFunction]
        public void CountA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new CountAProcessor();
            processor.ProcessArguments(args);
            result.SetValue(processor.Count);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Reference })]
        public void CountBlank(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new CountBlankProcessor();
            processor.ProcessArguments(args);
            result.SetValue(processor.Count);
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.SheetReference,
                                             OperandType.Primitive
                                         })]
        public void CountIf(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var range = (ISheetReference)args[0].ValueAsReference;
            object criteria = args[1].ValueAsPrimitive;

            SheetValuePredicate pred = SheetValuePredicate.Create(criteria);
            object[,] values = range.GetValuesTable();
            var processor = new CountIfConditionalSheetProcessor();
            DoConditionalTableOp(values, processor, pred);
            result.SetValue(processor.Count);
        }

        [VariableArgumentFormulaFunction]
        public void Mode(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeMode(processor.Values, result);
            }
        }

        [VariableArgumentFormulaFunction]
        public void Median(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                ComputeMedian(processor.Values, result);
            }
        }

        #endregion

        #region "Math functions"

        [VariableArgumentFormulaFunction]
        public void Sum(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                double sum__1 = ComputeSum(processor.Values);
                result.SetValue(sum__1);
            }
        }

        [VariableArgumentFormulaFunction]
        public void SumSq(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                double sum = ComputeSumOfSquares(processor.Values);
                result.SetValue(sum);
            }
        }

        [VariableArgumentFormulaFunction]
        public void Product(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new SumProcessor();
            bool success = processor.ProcessArguments(args);

            if (success == false)
            {
                result.SetError(processor.ErrorValue);
            }
            else
            {
                double product__1 = ComputeProduct(processor.Values);
                result.SetValue(product__1);
            }
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Double,
                                             OperandType.Integer
                                         })]
        public void Round(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            int decimals = args[1].ValueAsInteger;

            number = ComputeRound(number, decimals);

            result.SetValue(number);
        }

        private double ComputeRound(double number, int decimals)
        {
            if (decimals >= 0)
            {
                return Math.Round(number, decimals);
            }
            double scale = Math.Pow(10, Math.Abs(decimals));
            return Math.Round(number / scale) * scale;
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Sin(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(Math.Sin(args[0].ValueAsDouble));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Sinh(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(Math.Sinh(args[0].ValueAsDouble));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void ASin(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double d = args[0].ValueAsDouble;

            if (d < -1 | d > 1)
            {
                result.SetError(ErrorValueType.Num);
            }
            else
            {
                result.SetValue(Math.Asin(args[0].ValueAsDouble));
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Cos(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(Math.Cos(args[0].ValueAsDouble));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Cosh(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(Math.Cosh(args[0].ValueAsDouble));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void ACos(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double d = args[0].ValueAsDouble;

            if (d < -1 | d > 1)
            {
                result.SetError(ErrorValueType.Num);
            }
            else
            {
                result.SetValue(Math.Acos(args[0].ValueAsDouble));
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Tan(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(Math.Tan(args[0].ValueAsDouble));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Tanh(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(Math.Tanh(args[0].ValueAsDouble));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void ATan(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(Math.Atan(args[0].ValueAsDouble));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Abs(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double d = args[0].ValueAsDouble;
            result.SetValue(Math.Abs(d));
        }

        [FixedArgumentFormulaFunction(0, new OperandType[]
                                         {
                                         })]
        public void PI(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(Math.PI);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Degrees(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double rads = args[0].ValueAsDouble;
            double degs = rads * (180 / Math.PI);
            result.SetValue(degs);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Radians(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double degs = args[0].ValueAsDouble;
            double rads = degs * (Math.PI / 180);
            result.SetValue(rads);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Exp(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double d = args[0].ValueAsDouble;
            result.SetValue(Math.Exp(d));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Even(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            int sign = Math.Sign(number);

            number = Math.Abs(number);
            number = Math.Ceiling(number);

            if (Math.Abs(number % 2) > TOLERANCE)
            {
                number += 1;
            }

            result.SetValue(number * sign);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Odd(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            int sign = Math.Sign(number);

            number = Math.Abs(number);
            number = Math.Ceiling(number);

            if (Math.Abs(number % 2) < TOLERANCE)
            {
                number += 1;
            }

            result.SetValue(number * sign);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Fact(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double n = args[0].ValueAsDouble;

            if (n < 0 | n > MAX_FACTORIAL)
            {
                result.SetError(ErrorValueType.Num);
            }
            else if (Math.Abs(n) < TOLERANCE)
            {
                result.SetValue(1.0);
            }
            else
            {
                result.SetValue(ComputeFactorial(n));
            }
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Double,
                                             OperandType.Double
                                         })]
        public void Ceiling(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            double significance = args[1].ValueAsDouble;

            if (Math.Abs(number) < TOLERANCE | Math.Abs(significance) < TOLERANCE)
            {
                result.SetValue(0);
                return;
            }

            if (Math.Sign(number) != Math.Sign(significance))
            {
                result.SetError(ErrorValueType.Num);
            }
            else
            {
                double ceil = Math.Ceiling(number / significance) * significance;
                result.SetValue(ceil);
            }
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Double,
                                             OperandType.Double
                                         })]
        public void Floor(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            double significance = args[1].ValueAsDouble;

            if (Math.Abs(number) < TOLERANCE || Math.Abs(significance) < TOLERANCE)
            {
                result.SetValue(0);
                return;
            }

            if (Math.Sign(number) != Math.Sign(significance))
            {
                result.SetError(ErrorValueType.Num);
            }
            else
            {
                double floor__1 = Math.Floor(number / significance) * significance;
                result.SetValue(floor__1);
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Int(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            result.SetValue(Math.Floor(number));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Ln(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;

            if (number <= 0)
            {
                result.SetError(ErrorValueType.Num);
            }
            else
            {
                result.SetValue(Math.Log(number));
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Log10(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;

            if (number <= 0)
            {
                result.SetError(ErrorValueType.Num);
            }
            else
            {
                result.SetValue(Math.Log10(number));
            }
        }

        [FixedArgumentFormulaFunction(1, 2, new[]
                                            {
                                                OperandType.Double,
                                                OperandType.Double
                                            })]
        public void Log(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            double @base = 10;

            if (args.Length > 1)
            {
                @base = args[1].ValueAsDouble;
            }

            if (number <= 0 | @base <= 0)
            {
                result.SetError(ErrorValueType.Num);
            }
            else if (Math.Abs(@base - 1) < TOLERANCE)
            {
                result.SetError(ErrorValueType.Div0);
            }
            else
            {
                result.SetValue(Math.Log(number) / Math.Log(@base));
            }
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Double,
                                             OperandType.Double
                                         })]
        public void Mod(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            double divisor = args[1].ValueAsDouble;

            if (Math.Abs(divisor) < TOLERANCE)
            {
                result.SetError(ErrorValueType.Div0);
            }
            else
            {
                double value = number - divisor * Math.Floor(number / divisor);
                result.SetValue(value);
            }
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Double,
                                             OperandType.Double
                                         })]
        public void Power(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            double power__1 = args[1].ValueAsDouble;

            // Relying on result to handle NAN and infinity
            result.SetValue(Math.Pow(number, power__1));
        }

        [FixedArgumentFormulaFunction(0, new OperandType[]
                                         {
                                         }, IsVolatile = true)]
        public void Rand(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(_random.NextDouble());
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Integer,
                                             OperandType.Integer
                                         }, IsVolatile = true)]
        public void Randbetween(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(_random.Next(args[0].ValueAsInteger, args[1].ValueAsInteger));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Sign(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            result.SetValue(Math.Sign(number));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Double })]
        public void Sqrt(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;

            if (number < 0)
            {
                result.SetError(ErrorValueType.Num);
            }
            else
            {
                result.SetValue(Math.Sqrt(number));
            }
        }

        [FixedArgumentFormulaFunction(1, 2, new[]
                                            {
                                                OperandType.Double,
                                                OperandType.Integer
                                            })]
        public void Trunc(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            int decimals = 0;

            if (args.Length > 1)
            {
                decimals = args[1].ValueAsInteger;
            }

            double scale = Math.Pow(10, decimals);

            number = number * scale;
            number = Math.Truncate(number);
            number = number / scale;

            result.SetValue(number);
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Double,
                                             OperandType.Integer
                                         })]
        public void RoundDown(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            int decimals = args[1].ValueAsInteger;

            double half = 0.5 * Math.Pow(10, -decimals) * Math.Sign(number);
            number = number - half;
            number = ComputeRound(number, decimals);

            result.SetValue(number);
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Double,
                                             OperandType.Integer
                                         })]
        public void RoundUp(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;
            int decimals = args[1].ValueAsInteger;

            double half = 0.5 * Math.Pow(10, -decimals) * Math.Sign(number);
            number = number + half;
            number = ComputeRound(number, decimals);

            result.SetValue(number);
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Integer,
                                             OperandType.Integer
                                         })]
        public void Combin(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            int n = args[0].ValueAsInteger;
            int k = args[1].ValueAsInteger;

            if (n < 0 | k < 0 | n < k)
            {
                result.SetError(ErrorValueType.Num);
            }
            else
            {
                result.SetValue(ComputeCombinations(n, k));
            }
        }

        [FixedArgumentFormulaFunction(2, 3, new[]
                                            {
                                                OperandType.SheetReference,
                                                OperandType.Primitive,
                                                OperandType.SheetReference
                                            })]
        public void SumIf(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var sourceRange = (ISheetReference)args[0].ValueAsReference;
            object criteria = args[1].ValueAsPrimitive;
            ISheetReference sumRange = sourceRange;

            if (args.Length == 3)
            {
                sumRange = (ISheetReference)args[2].ValueAsReference;
                Rectangle rect = sourceRange.Area;
                rect.Offset(sumRange.Area.Left - sourceRange.Area.Left, sumRange.Area.Top - sourceRange.Area.Top);

                if (Utility.IsRectangleInSheet(rect, sumRange.Sheet) == false)
                {
                    result.SetError(ErrorValueType.Ref);
                    return;
                }
                sumRange = engine.ReferenceFactory.FromRectangle(rect);
            }

            SheetValuePredicate pred = SheetValuePredicate.Create(criteria);
            object[,] sourceValues = sourceRange.GetValuesTable();
            object[,] sumValues = sourceValues;

            if (!ReferenceEquals(sourceRange, sumRange))
            {
                sumValues = sumRange.GetValuesTable();
            }

            var processor = new SumIfConditionalSheetProcessor();
            processor.Initialize(sumValues);

            DoConditionalTableOp(sourceValues, processor, pred);

            double sum = ComputeSum(processor.Values);
            result.SetValue(sum);
        }

        #endregion

        #region "Information functions"

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsLogical(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];
            bool isValue = arg.IsPrimitive && ((arg.ValueAsPrimitive) is bool);
            result.SetValue(isValue);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsNumber(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];
            bool isValue = arg.IsPrimitive && ((arg.ValueAsPrimitive) is int | (arg.ValueAsPrimitive) is double);
            result.SetValue(isValue);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsText(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];
            bool isValue = arg.IsPrimitive && ((arg.ValueAsPrimitive) is string);
            result.SetValue(isValue);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsBlank(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];
            bool isValue = arg.IsPrimitive && (arg.ValueAsPrimitive == null);
            result.SetValue(isValue);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsError(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];
            // For whatever reason, IsError called with a non-cell range returns true....
            bool isValue = (arg.IsPrimitive == false) || ((arg.ValueAsPrimitive) is ErrorValueType);
            result.SetValue(isValue);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsErr(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];
            bool isValue;

            if (arg.IsPrimitive == false)
            {
                isValue = true;
            }
            else if (arg.IsError == false)
            {
                isValue = false;
            }
            else
            {
                isValue = arg.ValueAsError != ErrorValueType.NA;
            }

            result.SetValue(isValue);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsNA(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];
            bool isValue;

            if (arg.IsPrimitive == false)
            {
                isValue = false;
            }
            else if (arg.IsError == false)
            {
                isValue = false;
            }
            else
            {
                isValue = arg.ValueAsError == ErrorValueType.NA;
            }

            result.SetValue(isValue);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsRef(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(args[0].IsReference);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void IsNonText(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];
            bool isValue = (arg.IsPrimitive == false) || (!(arg.ValueAsPrimitive is string));
            result.SetValue(isValue);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void Type(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];

            if (arg.IsPrimitive == false)
            {
                result.SetValue(16);
                return;
            }

            object value = arg.ValueAsPrimitive;

            if (value == null)
            {
                result.SetValue(1);
                return;
            }

            Type t = value.GetType();
            int typeCode = 0;

            if (Utility.IsNumericType(t))
            {
                typeCode = 1;
            }
            else if (ReferenceEquals(t, typeof(string)))
            {
                typeCode = 2;
            }
            else if (ReferenceEquals(t, typeof(bool)))
            {
                typeCode = 4;
            }
            else if (ReferenceEquals(t, typeof(ErrorValueType)))
            {
                typeCode = 16;
            }
            else
            {
                Debug.Assert(false, "unknown type");
            }

            result.SetValue(typeCode);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Self })]
        public void Error_Type(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            Argument arg = args[0];

            ErrorValueType ev;

            if (arg.IsPrimitive == false)
            {
                ev = ErrorValueType.Value;
            }
            else if (arg.IsError)
            {
                ev = arg.ValueAsError;
            }
            else
            {
                result.SetError(ErrorValueType.NA);
                return;
            }

            ErrorValueType[] values =
            {
                ErrorValueType.Null,
                ErrorValueType.Div0,
                ErrorValueType.Value,
                ErrorValueType.Ref,
                ErrorValueType.Name,
                ErrorValueType.Num,
                ErrorValueType.NA
            };
            int index = Array.IndexOf(values, ev) + 1;
            Debug.Assert(index != -1, "unknown error value");
            result.SetValue(index);
        }

        #endregion

        #region "Text functions"

        [FixedArgumentFormulaFunction(1, 2, new[]
                                            {
                                                OperandType.String,
                                                OperandType.Integer
                                            })]
        public void Left(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;

            int count = args.Length == 1 ? 1 : args[1].ValueAsInteger;

            if (count < 0)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }

            count = Math.Min(count, text.Length);

            text = text.Substring(0, count);
            result.SetValue(text);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Integer })]
        public void Char(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            int code = args[0].ValueAsInteger;

            if (code <= 0 | code > 65535)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                char c = Convert.ToChar(code);
                result.SetValue(c.ToString(CultureInfo.InvariantCulture));
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void Clean(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string s = args[0].ValueAsString;
            s = Regex.Replace(s, "\\p{C}+", string.Empty);
            result.SetValue(s);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void Code(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string s = args[0].ValueAsString;

            if (s.Length < 1)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                char c = args[0].ValueAsString[0];
                result.SetValue(Convert.ToInt32(c));
            }
        }

        [FixedArgumentFormulaFunction(1, 2, new[]
                                            {
                                                OperandType.Double,
                                                OperandType.Integer
                                            })]
        public void Dollar(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;

            int decimals = args.Length == 2 ? args[1].ValueAsInteger : 2;

            number = ComputeRound(number, decimals);

            string format = "C";

            decimals = Math.Max(0, decimals);
            format = format + decimals;

            result.SetValue(number.ToString(format));
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.String,
                                             OperandType.String
                                         })]
        public void Exact(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string s1 = args[0].ValueAsString;
            string s2 = args[1].ValueAsString;

            bool equal = string.Equals(s1, s2, StringComparison.Ordinal);
            result.SetValue(equal);
        }

        [FixedArgumentFormulaFunction(2, 3, new[]
                                            {
                                                OperandType.String,
                                                OperandType.String,
                                                OperandType.Integer
                                            })]
        public void Find(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string findText = args[0].ValueAsString;
            string withinText = args[1].ValueAsString;

            int startIndex = args.Length == 3 ? args[2].ValueAsInteger : 1;

            if (startIndex <= 0 | startIndex > withinText.Length)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }

            startIndex -= 1;

            int index = withinText.IndexOf(findText, startIndex, StringComparison.CurrentCulture);

            if (index == -1)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                result.SetValue(index + 1);
            }
        }

        [FixedArgumentFormulaFunction(1, 3, new[]
                                            {
                                                OperandType.Double,
                                                OperandType.Integer,
                                                OperandType.Boolean
                                            })]
        public void Fixed(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double number = args[0].ValueAsDouble;

            int decimals = args.Length > 1 ? args[1].ValueAsInteger : 2;

            bool noCommas = args.Length > 2 && args[2].ValueAsBoolean;

            number = ComputeRound(number, decimals);

            string format = noCommas == false ? "N" : "F";

            decimals = Math.Max(0, decimals);
            format = format + decimals;

            result.SetValue(number.ToString(format));
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void Len(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string s = args[0].ValueAsString;
            result.SetValue(s.Length);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void Lower(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string s = args[0].ValueAsString;
            result.SetValue(s.ToLower());
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void Upper(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string s = args[0].ValueAsString;
            result.SetValue(s.ToUpper());
        }

        [FixedArgumentFormulaFunction(3, new[]
                                         {
                                             OperandType.String,
                                             OperandType.Integer,
                                             OperandType.Integer
                                         })]
        public void Mid(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;
            int startIndex = args[1].ValueAsInteger;
            int length = args[2].ValueAsInteger;

            if (startIndex < 1 | length < 0)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }

            startIndex -= 1;

            string value;

            if (startIndex >= text.Length)
            {
                value = string.Empty;
            }
            else
            {
                length = Math.Min(length, text.Length - startIndex);
                value = text.Substring(startIndex, length);
            }

            result.SetValue(value);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void Proper(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;
            var sb = new StringBuilder(text);
            bool firstLetter = false;

            for (int i = 0; i <= sb.Length - 1; i++)
            {
                char c = sb[i];
                if (char.IsLetter(c))
                {
                    if (firstLetter == false)
                    {
                        firstLetter = true;
                        c = char.ToUpper(c);
                    }
                    else if (char.IsLetter(sb[i - 1]) == false)
                    {
                        c = char.ToUpper(c);
                    }
                    else
                    {
                        c = char.ToLower(c);
                    }

                    sb[i] = c;
                }
            }

            text = sb.ToString();
            result.SetValue(text);
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.String,
                                             OperandType.Integer
                                         })]
        public void Rept(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;
            int count = args[1].ValueAsInteger;

            if (count < 0 | text.Length * count > short.MaxValue)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                var sb = new StringBuilder(text.Length * count);
                sb.Insert(0, text, count);
                result.SetValue(sb.ToString());
            }
        }

        [FixedArgumentFormulaFunction(4, new[]
                                         {
                                             OperandType.String,
                                             OperandType.Integer,
                                             OperandType.Integer,
                                             OperandType.String
                                         })]
        public void Replace(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;
            int startIndex = args[1].ValueAsInteger;
            int count = args[2].ValueAsInteger;
            string newText = args[3].ValueAsString;

            if (startIndex < 1 | count < 0)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                startIndex -= 1;
                count = Math.Min(count, text.Length);
                if (startIndex > text.Length)
                {
                    startIndex = text.Length;
                    count = 0;
                }

                var sb = new StringBuilder(text);
                sb.Remove(startIndex, count);
                sb.Insert(startIndex, newText);
                result.SetValue(sb.ToString());
            }
        }

        //SUBSTITUTE(text,old_text,new_text,instance_num)
        [FixedArgumentFormulaFunction(3, 4, new[]
                                            {
                                                OperandType.String,
                                                OperandType.String,
                                                OperandType.String,
                                                OperandType.Integer
                                            })]
        public void Substitute(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;
            string oldText = args[1].ValueAsString;
            string newText = args[2].ValueAsString;
            int instanceNum = -1;

            if (args.Length == 4)
            {
                instanceNum = args[3].ValueAsInteger;

                if (instanceNum < 1)
                {
                    result.SetError(ErrorValueType.Value);
                    return;
                }
                instanceNum -= 1;
            }

            if (oldText.Length == 0)
            {
                result.SetValue(text);
                return;
            }

            if (instanceNum == -1)
            {
                text = text.Replace(oldText, newText);
            }
            else
            {
                int[] indices = GetIndicesOf(text, oldText);

                if (instanceNum < indices.Length)
                {
                    int index = indices[instanceNum];
                    text = text.Remove(index, oldText.Length);
                    text = text.Insert(index, newText);
                }
            }

            result.SetValue(text);
        }

        private int[] GetIndicesOf(string text, string s)
        {
            var list = new List<int>();

            int index = 0;

            while (index < text.Length)
            {
                int tempIndex = text.IndexOf(s, index, StringComparison.Ordinal);
                if (tempIndex != -1)
                {
                    list.Add(tempIndex);
                    index = tempIndex + s.Length;
                }
                else
                {
                    break; // TODO: might not be correct. Was : Exit While
                }
            }

            var arr = new int[list.Count];
            list.CopyTo(arr, 0);
            return arr;
        }

        [FixedArgumentFormulaFunction(1, 2, new[]
                                            {
                                                OperandType.String,
                                                OperandType.Integer
                                            })]
        public void Right(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;
            int count = 1;

            if (args.Length > 1)
            {
                count = args[1].ValueAsInteger;
            }

            if (count < 0)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                count = Math.Min(count, text.Length);
                int startIndex = text.Length - count;
                text = text.Substring(startIndex, count);
                result.SetValue(text);
            }
        }

        [FixedArgumentFormulaFunction(2, new[]
                                         {
                                             OperandType.Double,
                                             OperandType.String
                                         })]
        public void Text(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            double value = args[0].ValueAsDouble;
            string format = args[1].ValueAsString;

            try
            {
                string text__1 = value.ToString(format);
                result.SetValue(text__1);
            }
            catch (FormatException)
            {
                result.SetError(ErrorValueType.Value);
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void Trim(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;
            // This is an inefficient but quick way to do this
            text = Regex.Replace(text, "^ +| +$", string.Empty);
            text = Regex.Replace(text, " +", " ");
            result.SetValue(text);
        }

        [VariableArgumentFormulaFunction]
        public void Concatenate(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= args.Length - 1; i++)
            {
                Argument arg = args[i];
                if (arg.IsError)
                {
                    result.SetError(arg.ValueAsError);
                    return;
                }
                if (arg.IsString)
                {
                    sb.Append(arg.ValueAsString);
                }
                else
                {
                    result.SetError(ErrorValueType.Value);
                    return;
                }
            }

            result.SetValue(sb.ToString());
        }

        [FixedArgumentFormulaFunction(2, 3, new[]
                                            {
                                                OperandType.String,
                                                OperandType.String,
                                                OperandType.Integer
                                            })]
        public void Search(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string findText = args[0].ValueAsString;
            string withinText = args[1].ValueAsString;

            int startIndex = args.Length == 3 ? args[2].ValueAsInteger : 1;

            if (startIndex <= 0 | startIndex > withinText.Length)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }

            startIndex -= 1;

            findText = Utility.Wildcard2Regex(findText);
            var re = new Regex(findText, RegexOptions.IgnoreCase);
            Match m = re.Match(withinText, startIndex);

            if (ReferenceEquals(m, System.Text.RegularExpressions.Match.Empty))
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                result.SetValue(m.Index + 1);
            }
        }

        #endregion

        #region "Logical functions"

        [FixedArgumentFormulaFunction(2, 3, new[]
                                            {
                                                OperandType.Boolean,
                                                OperandType.Self,
                                                OperandType.Self
                                            }, Names = new[] { "Если" })]
        public void If(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            bool condition = args[0].ValueAsBoolean;

            if (condition)
            {
                result.SetValue(args[1]);
            }
            else
            {
                if (args.Length == 2)
                {
                    result.SetValue(false);
                }
                else
                {
                    result.SetValue(args[2]);
                }
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.Boolean }, Names = new[] { "Нет" })]
        public void Not(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            bool b = args[0].ValueAsBoolean;
            result.SetValue(!b);
        }

        [VariableArgumentFormulaFunction(Names = new [] { "И" })]
        public void And(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new LogicalFunctionProcessor();
            if (processor.ProcessArguments(args) == false)
            {
                result.SetError(processor.ErrorValue);
                return;
            }

            if (processor.Values.Count == 0)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                bool b = ComputeAnd(processor.Values);
                result.SetValue(b);
            }
        }

        [VariableArgumentFormulaFunction(Names = new[] { "Или" })]
        public void Or(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var processor = new LogicalFunctionProcessor();
            if (processor.ProcessArguments(args) == false)
            {
                result.SetError(processor.ErrorValue);
                return;
            }

            if (processor.Values.Count == 0)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                bool b = ComputeOr(processor.Values);
                result.SetValue(b);
            }
        }

        #endregion

        #region "Lookup functions"

        [VariableArgumentFormulaFunction]
        public void Choose(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            if (args[0].IsInteger == false)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }

            int index = args[0].ValueAsInteger;

            if (index < 1 | index > args.Length - 1)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                result.SetValue(args[index]);
            }
        }

        [FixedArgumentFormulaFunction(2, 5, new[]
                                            {
                                                OperandType.Integer,
                                                OperandType.Integer,
                                                OperandType.Integer,
                                                OperandType.Boolean,
                                                OperandType.String
                                            })]
        public void Address(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            int rowIndex = args[0].ValueAsInteger;
            int columnIndex = args[1].ValueAsInteger;

            int refType = 1;

            if (args.Length > 2)
            {
                refType = args[2].ValueAsInteger;
            }

            if (refType < 1 | refType > 4 | SheetReference.IsValidColumnIndex(columnIndex) == false |
                SheetReference.IsValidRowIndex(rowIndex) == false)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }

            string sheetText = CreateSheetString(args);

            string rowDollar = string.Empty;
            string colDollar = string.Empty;

            if (refType == 1 | refType == 2)
            {
                rowDollar = "$";
            }

            if (refType == 1 | refType == 3)
            {
                colDollar = "$";
            }

            string address__1 = String.Concat(sheetText, colDollar, SheetReference.ColumnIndex2Label(columnIndex), rowDollar, rowIndex);
            result.SetValue(address__1);
        }

        private string CreateSheetString(Argument[] args)
        {
            if (args.Length < 5)
            {
                return string.Empty;
            }

            string s = args[4].ValueAsString;

            if (s.IndexOf(' ') != -1)
            {
                return String.Concat("'", s, "'");
            }
            return String.Concat(s, "!");
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.SheetReference })]
        public void Column(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var @ref = (ISheetReference)args[0].ValueAsReference;
            result.SetValue(@ref.Column);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.SheetReference })]
        public void Row(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var @ref = (ISheetReference)args[0].ValueAsReference;
            result.SetValue(@ref.Row);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.SheetReference })]
        public void Columns(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var @ref = (ISheetReference)args[0].ValueAsReference;
            result.SetValue(@ref.Width);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.SheetReference })]
        public void Rows(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var @ref = (ISheetReference)args[0].ValueAsReference;
            result.SetValue(@ref.Height);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.SheetReference })]
        public void Areas(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(1);
        }

        [FixedArgumentFormulaFunction(3, 4, new[]
                                            {
                                                OperandType.SheetReference,
                                                OperandType.Integer,
                                                OperandType.Integer,
                                                OperandType.Integer
                                            })]
        public void Index(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var @ref = (ISheetReference)args[0].ValueAsReference;
            int rowOffset = args[1].ValueAsInteger;
            int colOffset = args[2].ValueAsInteger;
            Rectangle refRect = @ref.Area;

            if (rowOffset < 0 | colOffset < 0)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }

            int height;
            int width;
            int x;
            int y;

            if (rowOffset == 0)
            {
                height = refRect.Height;
                y = 0;
            }
            else
            {
                height = 1;
                y = rowOffset - 1;
            }

            if (colOffset == 0)
            {
                width = refRect.Width;
                x = 0;
            }
            else
            {
                width = 1;
                x = colOffset - 1;
            }

            var rect = new Rectangle(refRect.Left + x, refRect.Top + y, width, height);

            if (refRect.Contains(rect) == false)
            {
                result.SetError(ErrorValueType.Ref);
            }
            else
            {
                result.SetValue(engine.ReferenceFactory.FromRectangle(rect));
            }
        }

        [FixedArgumentFormulaFunction(3, 5, new[]
                                            {
                                                OperandType.SheetReference,
                                                OperandType.Integer,
                                                OperandType.Integer,
                                                OperandType.Integer,
                                                OperandType.Integer
                                            }, IsVolatile = true)]
        public void Offset(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            var @ref = (ISheetReference)args[0].ValueAsReference;
            int rowOffset = args[1].ValueAsInteger;
            int colOffset = args[2].ValueAsInteger;
            Rectangle refRect = @ref.Area;
            int height = refRect.Height;
            int width = refRect.Width;

            if (args.Length > 3)
            {
                height = args[3].ValueAsInteger;
            }

            if (args.Length > 4)
            {
                width = args[4].ValueAsInteger;
            }

            if (width == 0 | height == 0)
            {
                result.SetError(ErrorValueType.Ref);
                return;
            }
            if (width < 0 | height < 0)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }

            refRect.Offset(colOffset, rowOffset);
            var newRect = new Rectangle(refRect.Left, refRect.Top, width, height);
            if (SheetReference.IsRectangleInSheet(newRect, @ref.Sheet) == false)
            {
                result.SetError(ErrorValueType.Ref);
            }
            else
            {
                result.SetValue(engine.ReferenceFactory.FromRectangle(newRect));
            }
        }

        [FixedArgumentFormulaFunction(1, 2, new[]
                                            {
                                                OperandType.String,
                                                OperandType.Boolean
                                            }, IsVolatile = true)]
        public void Indirect(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string text = args[0].ValueAsString;

            try
            {
                ISheetReference @ref = engine.ReferenceFactory.Parse(text);
                result.SetValue(@ref);
            }
            catch (Exception)
            {
                result.SetError(ErrorValueType.Ref);
            }
        }

        [FixedArgumentFormulaFunction(3, 4, new[]
                                            {
                                                OperandType.Primitive,
                                                OperandType.SheetReference,
                                                OperandType.Integer,
                                                OperandType.Boolean
                                            })]
        public void HLookup(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DoHVLookup(args, result, new HLookupProcessor());
        }

        [FixedArgumentFormulaFunction(3, 4, new[]
                                            {
                                                OperandType.Primitive,
                                                OperandType.SheetReference,
                                                OperandType.Integer,
                                                OperandType.Boolean
                                            })]
        public void VLookup(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DoHVLookup(args, result, new VLookupProcessor());
        }

        [FixedArgumentFormulaFunction(3, new[]
                                         {
                                             OperandType.Primitive,
                                             OperandType.SheetReference,
                                             OperandType.SheetReference
                                         })]
        public void Lookup(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            object lookupValue = args[0].ValueAsPrimitive;
            var lookupRef = (ISheetReference)args[1].ValueAsReference;
            var resultRef = (ISheetReference)args[2].ValueAsReference;

            if (lookupRef.Area.Size != resultRef.Area.Size)
            {
                result.SetError(ErrorValueType.Ref);
                return;
            }

            object[,] lookupTable = lookupRef.GetValuesTable();
            object[,] resultTable = resultRef.GetValuesTable();

            object[] lookupVector;
            object[] resultVector;

            if (lookupTable.GetLength(0) == 1)
            {
                lookupVector = Utility.GetTableRow(lookupTable, 0);
                resultVector = Utility.GetTableRow(resultTable, 0);
            }
            else
            {
                lookupVector = Utility.GetTableColumn(lookupTable, 0);
                resultVector = Utility.GetTableColumn(resultTable, 0);
            }

            var processor = new PlainLookupProcessor();
            processor.Initialize(lookupVector, resultVector);

            DoGenericLookup(processor, lookupValue, false, result);
        }

        [FixedArgumentFormulaFunction(2, 3, new[]
                                            {
                                                OperandType.Primitive,
                                                OperandType.SheetReference,
                                                OperandType.Integer
                                            })]
        public void Match(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            object lookupValue = args[0].ValueAsPrimitive;
            var lookupRef = (ISheetReference)args[1].ValueAsReference;
            int matchType = 1;

            if (args.Length == 3)
            {
                matchType = args[2].ValueAsInteger;
            }

            object[,] lookupTable = lookupRef.GetValuesTable();
            object[] lookupVector = Utility.GetTableColumn(lookupTable, 0);

            var processor = new PlainLookupProcessor();
            processor.Initialize(lookupVector, lookupVector);

            int matchIndex;
            SheetValuePredicate predicate = CreateLookupPredicate(lookupValue);

            if (matchType > 0)
            {
                predicate.SetCompareType(CompareType.LessThanOrEqual);
                matchIndex = IndexOfMatch(lookupVector, predicate, true);
            }
            else if (matchType == 0)
            {
                predicate.SetCompareType(CompareType.Equal);
                matchIndex = IndexOfMatch(lookupVector, predicate, true);
            }
            else
            {
                predicate.SetCompareType(CompareType.GreaterThanOrEqual);
                matchIndex = IndexOfMatch(lookupVector, predicate, false);
            }

            if (matchIndex == -1)
            {
                result.SetError(ErrorValueType.NA);
            }
            else
            {
                result.SetValue(matchIndex + 1);
            }
        }

        private void DoHVLookup(Argument[] args, FunctionResult result, HVLookupProcessor processor)
        {
            object lookupValue = args[0].ValueAsPrimitive;
            var tableRef = (ISheetReference)args[1].ValueAsReference;
            int index = args[2].ValueAsInteger;
            bool exactMatch = false;

            if (lookupValue == null)
            {
                result.SetError(ErrorValueType.NA);
                return;
            }
            if (args[0].IsError)
            {
                result.SetValue(args[0]);
                return;
            }

            if (args.Length == 4)
            {
                exactMatch = !args[3].ValueAsBoolean;
            }

            Rectangle refRect = tableRef.Area;

            if (index < 1)
            {
                result.SetError(ErrorValueType.Value);
                return;
            }
            if (processor.IsValidIndex(refRect, index) == false)
            {
                result.SetError(ErrorValueType.Ref);
                return;
            }

            object[,] table = tableRef.GetValuesTable();
            processor.Initialize(table, index);

            DoGenericLookup(processor, lookupValue, exactMatch, result);
        }

        private void DoGenericLookup(LookupProcessor processor, object lookupValue, bool exactMatch,
            FunctionResult result)
        {
            object[] lookupVector = processor.GetLookupVector();
            SheetValuePredicate predicate = CreateLookupPredicate(lookupValue);

            predicate.SetCompareType(CompareType.Equal);
            int matchIndex = IndexOfMatch(lookupVector, predicate, true);

            if (matchIndex == -1 & exactMatch == false)
            {
                predicate.SetCompareType(CompareType.LessThan);
                matchIndex = IndexOfMatch(lookupVector, predicate, true);
            }

            if (matchIndex == -1)
            {
                result.SetError(ErrorValueType.NA);
            }
            else
            {
                object[] resultVector = processor.GetResultVector();
                result.SetValueFromSheet(resultVector[matchIndex]);
            }
        }

        private int IndexOfMatch(object[] line, SheetValuePredicate predicate, bool useGreatest)
        {
            IList matches = new ArrayList();

            for (int i = 0; i <= line.Length - 1; i++)
            {
                object value = line[i];
                value = Utility.NormalizeIfNumericValue(value);
                line[i] = value;
                if (predicate.IsMatch(value))
                {
                    matches.Add(value);
                }
            }

            if (matches.Count == 0)
            {
                return -1;
            }

            var arr = new object[matches.Count];
            matches.CopyTo(arr, 0);
            Array.Sort(arr);

            int nextIndex;

            if (useGreatest)
            {
                nextIndex = arr.Length - 1;
            }
            else
            {
                nextIndex = 0;
            }

            object nextValue = arr[nextIndex];
            return Array.IndexOf(line, nextValue);
        }

        private SheetValuePredicate CreateLookupPredicate(object targetValue)
        {
            var s = targetValue as string;
            SheetValuePredicate pred;

            if (s == null)
            {
                pred = new ComparerBasedPredicate(targetValue, new Comparer(CultureInfo.CurrentCulture));
            }
            else if (s.IndexOfAny(new[] { '?', '*' }) != -1)
            {
                pred = new WildcardPredicate(s);
            }
            else
            {
                pred = new ComparerBasedPredicate(s, StringComparer.OrdinalIgnoreCase);
            }

            pred.SetCompareType(CompareType.Equal);
            return pred;
        }

        #endregion

        #region "Date and time functions"

        [FixedArgumentFormulaFunction(3, new[]
                                         {
                                             OperandType.Integer,
                                             OperandType.Integer,
                                             OperandType.Integer
                                         })]
        public void Date(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            int year = args[0].ValueAsInteger;
            int month = args[1].ValueAsInteger;
            int day = args[2].ValueAsInteger;

            if (year < 1 | year > 9999)
            {
                result.SetError(ErrorValueType.Value);
            }
            else
            {
                DateTime? dt = CreateDate(year, month, day);
                if (dt.HasValue)
                {
                    result.SetValue(dt.Value);
                }
                else
                {
                    result.SetError(ErrorValueType.Value);
                }
            }
        }

        private DateTime? CreateDate(int year, int month, int day)
        {
            try
            {
                var dt = new DateTime(year, 1, 1);
                dt = dt.AddMonths(month - 1);
                dt = dt.AddDays(day - 1);
                return dt;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void DateValue(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string s = args[0].ValueAsString;
            DateTime dt;

            if (DateTime.TryParseExact(s, new[] { "d", "D" }, null, DateTimeStyles.AllowWhiteSpaces, out dt))
            {
                result.SetValue(dt);
            }
            else
            {
                result.SetError(ErrorValueType.Value);
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.DateTime })]
        public void Day(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DateTime dt = args[0].ValueAsDateTime;
            result.SetValue(dt.Day);
        }

        [FixedArgumentFormulaFunction(2, 3, new[]
                                            {
                                                OperandType.DateTime,
                                                OperandType.DateTime,
                                                OperandType.Boolean
                                            })]
        public void Days360(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DateTime dateStart = args[0].ValueAsDateTime;
            DateTime dateEnd = args[1].ValueAsDateTime;
            bool useEuropeanMethod = false;

            if (args.Length == 3)
            {
                useEuropeanMethod = args[2].ValueAsBoolean;
            }

            int daysInStartMonth = DateTime.DaysInMonth(dateStart.Year, dateStart.Month);
            int daysInEndMonth = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
            int daysStart = dateStart.Day;
            int daysEnd = dateEnd.Day;

            if (useEuropeanMethod)
            {
                daysEnd = Math.Min(30, dateEnd.Day);
            }
            else
            {
                if (dateStart.Month == 2 & dateStart.Day == daysInStartMonth)
                {
                    daysStart = 30;
                    if (dateEnd.Month == 2 & dateEnd.Day == daysInEndMonth)
                    {
                        daysEnd = 30;
                    }
                }

                if (daysEnd == 31 & daysStart >= 30)
                {
                    daysEnd = 30;
                }
            }

            daysStart = Math.Min(30, daysStart);

            result.SetValue(ComputeDays360(dateStart, dateEnd, daysStart, daysEnd));
        }

        private int ComputeDays360(DateTime dateStart, DateTime dateEnd, int daysStart, int daysEnd)
        {
            int sign = 1;

            if (DateTime.Compare(dateStart, dateEnd) == 1)
            {
                DateTime dt = dateStart;
                dateStart = dateEnd;
                dateEnd = dt;
                int days = daysStart;
                daysStart = daysEnd;
                daysEnd = days;
                sign = -1;
            }

            daysStart = dateStart.Year * 360 + dateStart.Month * 30 + daysStart;
            daysEnd = dateEnd.Year * 360 + dateEnd.Month * 30 + daysEnd;

            int result = (daysEnd - daysStart) * sign;
            return result;
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.DateTime })]
        public void Hour(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DateTime dt = args[0].ValueAsDateTime;
            result.SetValue(dt.Hour);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.DateTime })]
        public void Minute(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DateTime dt = args[0].ValueAsDateTime;
            result.SetValue(dt.Minute);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.DateTime }, Names = new[] { "Месяц" })]
        public void Month(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DateTime dt = args[0].ValueAsDateTime;
            result.SetValue(dt.Month);
        }

        [FixedArgumentFormulaFunction(0, new OperandType[]
                                         {
                                         }, IsVolatile = true)]
        public void Now(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(DateTime.Now);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.DateTime })]
        public void Second(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DateTime dt = args[0].ValueAsDateTime;
            result.SetValue(dt.Second);
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.DateTime }, Names = new[] { "Год" })]
        public void Year(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DateTime dt = args[0].ValueAsDateTime;
            result.SetValue(dt.Year);
        }

        [FixedArgumentFormulaFunction(3, new[]
                                         {
                                             OperandType.Integer,
                                             OperandType.Integer,
                                             OperandType.Integer
                                         })]
        public void Time(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            int hours = args[0].ValueAsInteger;
            int minutes = args[1].ValueAsInteger;
            int seconds = args[2].ValueAsInteger;

            try
            {
                var ts = new TimeSpan(hours, minutes, seconds);
                DateTime dt = DateTime.Today;
                dt = dt.Add(ts);
                result.SetValue(dt);
            }
            catch (ArgumentOutOfRangeException)
            {
                result.SetError(ErrorValueType.Value);
            }
        }

        [FixedArgumentFormulaFunction(1, new[] { OperandType.String })]
        public void TimeValue(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            string s = args[0].ValueAsString;
            DateTime dt;

            if (DateTime.TryParseExact(s, new[] { "t", "T" }, null, DateTimeStyles.AllowWhiteSpaces, out dt))
            {
                result.SetValue(dt);
            }
            else
            {
                result.SetError(ErrorValueType.Value);
            }
        }

        [FixedArgumentFormulaFunction(0, new OperandType[]
                                         {
                                         }, Names = new[] { "Сегодня" }, IsVolatile = true)]
        public void Today(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            result.SetValue(DateTime.Today);
        }

        [FixedArgumentFormulaFunction(1, 2, new[]
                                            {
                                                OperandType.DateTime,
                                                OperandType.Integer
                                            })]
        public void Weekday(Argument[] args, FunctionResult result, FormulaEngine engine)
        {
            DateTime dt = args[0].ValueAsDateTime;
            int returnType = 1;

            if (args.Length == 2)
            {
                returnType = args[1].ValueAsInteger;
            }

            if (returnType < 1 | returnType > 3)
            {
                result.SetError(ErrorValueType.Num);
                return;
            }

            DayOfWeek[] arr1 =
            {
                DayOfWeek.Sunday,
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday
            };
            DayOfWeek[] arr2 =
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };
            int index = 0;

            if (returnType == 1)
            {
                index = Array.IndexOf(arr1, dt.DayOfWeek);
                index += 1;
            }
            else if (returnType == 2)
            {
                index = Array.IndexOf(arr2, dt.DayOfWeek);
                index += 1;
            }
            else if (returnType == 3)
            {
                index = Array.IndexOf(arr2, dt.DayOfWeek);
            }

            result.SetValue(index);
        }

        #endregion

        #region "Utility"

        private double ComputeCombinations(int n, int k)
        {
            double C = 1;
            for (int i = 0; i <= k - 1; )
            {
                return (n - i) / (double)(k - i);
            }

            return C;
        }

        private double ComputeSum(IList<double> values)
        {
            double sum = 0.0;

            for (int i = 0; i <= values.Count - 1; i++)
            {
                double d = values[i];
                sum += d;
            }

            return sum;
        }

        private double ComputeSumOfSquares(IList<double> values)
        {
            double sum = 0.0;

            for (int i = 0; i <= values.Count - 1; i++)
            {
                double d = values[i];
                sum += Math.Pow(d, 2);
            }

            return sum;
        }

        private double? ComputeVariance(IList<double> values, bool isSample, FunctionResult result)
        {
            int count = values.Count;

            if (count == 0 | (count == 1 & isSample))
            {
                result.SetError(ErrorValueType.Div0);
                return null;
            }

            double mean = ComputeAverage(values);
            double sum = 0;

            for (int i = 0; i <= count - 1; i++)
            {
                double x = values[i];
                sum += Math.Pow((x - mean), 2);
            }

            if (isSample)
            {
                count -= 1;
            }
            double var = sum / count;
            result.SetValue(var);
            return var;
        }

        private void ComputeStandardDeviation(IList<double> values, bool isSample, FunctionResult result)
        {
            double? variance = ComputeVariance(values, isSample, result);
            if (variance.HasValue)
            {
                result.SetValue(Math.Sqrt(variance.Value));
            }
        }

        private void ComputeMode(IList<double> values, FunctionResult result)
        {
            var frequencies = new Dictionary<double, int>();

            for (int i = 0; i <= values.Count - 1; i++)
            {
                double value = values[i];
                if (frequencies.ContainsKey(value) == false)
                {
                    frequencies.Add(value, 1);
                }
                else
                {
                    frequencies[value] += 1;
                }
            }

            var modePair = new KeyValuePair<double, int>();
            int max = 1;

            foreach (var pair in frequencies)
            {
                if (pair.Value > max)
                {
                    modePair = pair;
                    max = pair.Value;
                }
            }

            if (max == 1)
            {
                result.SetError(ErrorValueType.NA);
            }
            else
            {
                result.SetValue(modePair.Key);
            }
        }

        private void ComputeMedian(IList<double> values, FunctionResult result)
        {
            if (values.Count == 0)
            {
                result.SetError(ErrorValueType.Num);
                return;
            }

            var arr = new double[values.Count];
            values.CopyTo(arr, 0);
            Array.Sort(arr);

            double median;

            if ((arr.Length & 1) == 1)
            {
                median = arr[(arr.Length - 1) / 2];
            }
            else
            {
                double d1 = arr[arr.Length / 2];
                double d2 = arr[(arr.Length / 2) - 1];
                median = (d1 + d2) / 2;
            }

            result.SetValue(median);
        }

        private double ComputeProduct(IList<double> values)
        {
            if (values.Count == 0)
            {
                return 0.0;
            }

            double product = 1;

            for (int i = 0; i <= values.Count - 1; i++)
            {
                double d = values[i];
                product *= d;
            }

            return product;
        }

        private double ComputeAverage(IList<double> values)
        {
            double sum = ComputeSum(values);
            return sum / values.Count;
        }

        private double ComputeMax(IList<double> values)
        {
            if (values.Count == 0)
            {
                return 0.0;
            }

            double max = double.MinValue;

            for (int i = 0; i <= values.Count - 1; i++)
            {
                double d = values[i];
                if (d > max)
                {
                    max = d;
                }
            }

            return max;
        }

        private double ComputeMin(IList<double> values)
        {
            if (values.Count == 0)
            {
                return 0.0;
            }

            double min = double.MaxValue;

            for (int i = 0; i <= values.Count - 1; i++)
            {
                double d = values[i];
                if (d < min)
                {
                    min = d;
                }
            }

            return min;
        }

        private double ComputeFactorial(double n)
        {
            double product = 1;
            n = Math.Truncate(n);

            for (int i = 0; i <= n - 1; i++)
            {
                product = product * _ractorialTable[i];
            }

            return product;
        }

        private bool ComputeAnd(IList<bool> values)
        {
            for (int i = 0; i <= values.Count - 1; i++)
            {
                bool b = values[i];
                if (b == false)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ComputeOr(IList<bool> values)
        {
            for (int i = 0; i <= values.Count - 1; i++)
            {
                bool b = values[i];
                if (b)
                {
                    return true;
                }
            }
            return false;
        }

        private void DoConditionalTableOp(object[,] compareValues, ConditionalSheetProcessor processor,
            SheetValuePredicate predicate)
        {
            for (int row = 0; row <= compareValues.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= compareValues.GetUpperBound(1); col++)
                {
                    object compareValue = compareValues[row, col];
                    if (predicate.IsMatch(compareValue))
                    {
                        processor.OnMatch(row, col);
                    }
                }
            }
        }

        #endregion
    }
}
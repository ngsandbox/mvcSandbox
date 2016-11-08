using System.IO;
using PerCederberg.Grammatica.Runtime;

namespace FormulaEngineCore
{
    /// <remarks>A token stream parser.</remarks>
    public class FormulaParser : RecursiveDescentParser
    {
        /// <summary>Creates a new parser.</summary>
        /// <param name='input'>the input stream to read from</param>
        /// <param name='settings'>Formula additional settings like ListSeparator and NumberDecimalSeparator</param>
        /// <param name='analyzer'>the analyzer to parse with</param>
        /// <exception cref='ParserCreationException'>if the parser couldn't be initialized correctly</exception>
        public FormulaParser(TextReader input, FormulaParserSettings settings, Analyzer analyzer) :
            base(new FormulaTokenizer(input, settings), analyzer)
        {
            CreatePatterns();
        }

        /// <summary>
        ///     Initializes the parser by creating all the productionpatterns.
        /// </summary>
        /// <exception cref='ParserCreationException'>if the parser couldn't be initialized correctly</exception>
        private void CreatePatterns()
        {
            AddPattern(GetFormulaPattern());
            AddPattern(GetScalarFormulaPattern());
            AddPattern(GetPrimaryExpressionPattern());
            AddPattern(GetExpressionPattern());
            AddPattern(GetLogicalConcatExpPattern());
            AddPattern(GetLogicalOptionsPattern());
            AddPattern(GetConcatPattern());
            AddPattern(GetAddPattern());
            AddPattern(GetAddSubOptionPattern());
            AddPattern(GetMultExpresPattern());
            AddPattern(GetMultDivOptionPattern());
            AddPattern(GetExponentiationExprPattern());
            AddPattern(GetPercentExpressionPattern());
            AddPattern(GetUnaryExpressionPattern());
            AddPattern(GetBasicExpressionPattern());
            AddPattern(GetReferencePattern());
            AddPattern(GetGridReferenceExprPattern());
            AddPattern(GetGridReferencePattern());
            AddPattern(GetFunctionCallPattern());
            AddPattern(GetArgumentListPattern());
            AddPattern(GetExpressionGroupPattern());
            AddPattern(GetPrimitivePattern());
            AddPattern(GetBooleanPattern());
            AddPattern(GetErrorsPattern());
            AddPattern(GetSubProdForLogicPattern());
            AddPattern(GetProdForConcatPattern());
            AddPattern(GetSubProdForAddPattern());
            AddPattern(GeSubProdForMultPattern());
            AddPattern(GetSubProdForExpressionPattern());
            AddPattern(GetSubProdForAddAndSubPattern());
            AddPattern(GetArgsSeparatorPattern());
        }

        private static ProductionPattern GetFormulaPattern()
        {
            var pattern = new ProductionPattern((int) FormulaConstants.FORMULA, "Formula");
            var alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.EQ, 0, 1);
            alt.AddProduction((int) FormulaConstants.SCALAR_FORMULA, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetScalarFormulaPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.SCALAR_FORMULA, "ScalarFormula");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.PRIMARY_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetPrimaryExpressionPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.PRIMARY_EXPRESSION, "PrimaryExpression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetExpressionPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.EXPRESSION, "Expression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.LOGICAL_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetExponentiationExprPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.EXPONENTIATION_EXPRESSION, "ExponentiationExpression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.PERCENT_EXPRESSION, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_5, 0, -1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetPercentExpressionPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.PERCENT_EXPRESSION, "PercentExpression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.UNARY_EXPRESSION, 1, 1);
            alt.AddToken((int) FormulaConstants.PERCENT, 0, -1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetUnaryExpressionPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.UNARY_EXPRESSION, "UnaryExpression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_6, 0, -1);
            alt.AddProduction((int) FormulaConstants.BASIC_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetBasicExpressionPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.BASIC_EXPRESSION, "BasicExpression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.PRIMITIVE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.FUNCTION_CALL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.REFERENCE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.EXPRESSION_GROUP, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetReferencePattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.REFERENCE, "Reference");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.DEFINED_NAME, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.GRID_REFERENCE_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetGridReferenceExprPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.GRID_REFERENCE_EXPRESSION, "GridReferenceExpression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.SHEET_NAME, 0, 1);
            alt.AddProduction((int) FormulaConstants.GRID_REFERENCE, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetGridReferencePattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.GRID_REFERENCE, "GridReference");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.CELL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.CELL_RANGE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.ROW_RANGE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.COLUMN_RANGE, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetFunctionCallPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.FUNCTION_CALL, "FunctionCall");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.FUNCTION_NAME, 1, 1);
            alt.AddProduction((int) FormulaConstants.ARGUMENT_LIST, 0, 1);
            alt.AddToken((int) FormulaConstants.RIGHT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetArgumentListPattern()
        {
            var pattern = new ProductionPattern((int) FormulaConstants.ARGUMENT_LIST, "ArgumentList");
            var alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.EXPRESSION, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_7, 0, -1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetExpressionGroupPattern()
        {
            var pattern = new ProductionPattern((int) FormulaConstants.EXPRESSION_GROUP, "ExpressionGroup");
            var alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.LEFT_PAREN, 1, 1);
            alt.AddProduction((int) FormulaConstants.EXPRESSION, 1, 1);
            alt.AddToken((int) FormulaConstants.RIGHT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetMultDivOptionPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.MULTIPLICATIVE_OP, "MultiplicativeOp");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.MUL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.DIV, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetMultExpresPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.MULTIPLICATIVE_EXPRESSION, "MultiplicativeExpression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.EXPONENTIATION_EXPRESSION, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_4, 0, -1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetAddSubOptionPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.ADDITIVE_OP, "AdditiveOp");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.ADD, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.SUB, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetLogicalConcatExpPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.LOGICAL_EXPRESSION, "LogicalExpression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.CONCAT_EXPRESSION, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_1, 0, -1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetLogicalOptionsPattern()
        {
            var pattern = new ProductionPattern((int) FormulaConstants.LOGICAL_OP, "LogicalOp");
            var alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.EQ, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.GT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.LT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.GTE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.LTE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.NE, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetPrimitivePattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) FormulaConstants.PRIMITIVE, "Primitive");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.NUMBER, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.BOOLEAN, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.STRING_LITERAL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.ERROR_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetSubProdForLogicPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_1, "Subproduction1") {Synthetic = true};
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.LOGICAL_OP, 1, 1);
            alt.AddProduction((int) FormulaConstants.CONCAT_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetProdForConcatPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_2, "Subproduction2") {Synthetic = true};
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.CONCAT, 1, 1);
            alt.AddProduction((int) FormulaConstants.ADDITIVE_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetSubProdForAddPattern()
        {
            ProductionPattern pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_3, "Subproduction3") {Synthetic = true};
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.ADDITIVE_OP, 1, 1);
            alt.AddProduction((int) FormulaConstants.MULTIPLICATIVE_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GeSubProdForMultPattern()
        {
            var pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_4, "Subproduction4")
            {
                Synthetic = true
            };
            var alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.MULTIPLICATIVE_OP, 1, 1);
            alt.AddProduction((int) FormulaConstants.EXPONENTIATION_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetSubProdForExpressionPattern()
        {
            var pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_5, "Subproduction5")
            {
                Synthetic = true
            };
            var alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.EXP, 1, 1);
            alt.AddProduction((int) FormulaConstants.PERCENT_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetSubProdForAddAndSubPattern()
        {
            var pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_6, "Subproduction6")
            {
                Synthetic = true
            };
            var alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.ADD, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.SUB, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetArgsSeparatorPattern()
        {
            var pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_7, "Subproduction7")
            {
                Synthetic = true
            };
            var alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.ARG_SEPARATOR, 1, 1);
            alt.AddProduction((int) FormulaConstants.EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetBooleanPattern()
        {
            var pattern = new ProductionPattern((int) FormulaConstants.BOOLEAN, "Boolean");
            var alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.TRUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.FALSE, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetErrorsPattern()
        {
            var pattern = new ProductionPattern((int) FormulaConstants.ERROR_EXPRESSION, "ErrorExpression");
            var alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.DIV_ERROR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.NA_ERROR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.NAME_ERROR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.NULL_ERROR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.REF_ERROR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.VALUE_ERROR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) FormulaConstants.NUM_ERROR, 1, 1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private static ProductionPattern GetAddPattern()
        {
            var pattern = new ProductionPattern((int) FormulaConstants.ADDITIVE_EXPRESSION, "AdditiveExpression");
            var alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.MULTIPLICATIVE_EXPRESSION, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_3, 0, -1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        private ProductionPattern GetConcatPattern()
        {
            var pattern = new ProductionPattern((int) FormulaConstants.CONCAT_EXPRESSION, "ConcatExpression");
            var alt = new ProductionPatternAlternative();
            alt.AddProduction((int) FormulaConstants.ADDITIVE_EXPRESSION, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_2, 0, -1);
            pattern.AddAlternative(alt);
            return pattern;
        }

        /// <summary>
        ///     An enumeration with the generated production node
        ///     identity constants.
        /// </summary>
        private enum SynteticPatterns
        {
            SUBPRODUCTION_1 = 3001,
            SUBPRODUCTION_2 = 3002,
            SUBPRODUCTION_3 = 3003,
            SUBPRODUCTION_4 = 3004,
            SUBPRODUCTION_5 = 3005,
            SUBPRODUCTION_6 = 3006,
            SUBPRODUCTION_7 = 3007
        }
    }
}
using System.IO;
using PerCederberg.Grammatica.Runtime;

namespace FormulaEngineCore
{
    /// <remarks>A character stream tokenizer.</remarks>
    internal class FormulaTokenizer : Tokenizer
    {
        /// <summary>Creates a new tokenizer for the specified input stream.</summary>
        /// <param name='input'>the input stream to read</param>
        /// <exception cref='ParserCreationException'>if the tokenizer couldn't be initialized correctly</exception>
        public FormulaTokenizer(TextReader input) : this(input, new FormulaParserSettings())
        {
        }

        /// <summary>Creates a new tokenizer for the specified input stream.</summary>
        /// <param name='input'>the input stream to read</param>
        /// <param name='settings'>Formula additional settings like ListSeparator and NumberDecimalSeparator</param>
        /// <exception cref='ParserCreationException'>if the tokenizer couldn't be initialized correctly</exception>
        public FormulaTokenizer(TextReader input, FormulaParserSettings settings)
            : base(input, true)
        {
            // Use the arg separator from the current culture
            Settings = settings;
            CreatePatterns();
        }

        public FormulaParserSettings Settings { get; private set; }

        /// <summary>
        /// Initializes the tokenizer by creating all the token patterns.
        /// </summary>
        /// <exception cref='ParserCreationException'>if the tokenizer couldn't be initialized correctly</exception>
        private void CreatePatterns()
        {
            // NOTE: Необходимо проверить на обработку русских символов в формулах
            AddPattern(new TokenPattern((int)FormulaConstants.ADD, "ADD", TokenPattern.PatternType.STRING, "+"));
            AddPattern(new TokenPattern((int)FormulaConstants.SUB, "SUB", TokenPattern.PatternType.STRING, "-"));
            AddPattern(new TokenPattern((int)FormulaConstants.MUL, "MUL", TokenPattern.PatternType.STRING, "*"));
            AddPattern(new TokenPattern((int)FormulaConstants.DIV, "DIV", TokenPattern.PatternType.STRING, "/"));
            AddPattern(new TokenPattern((int)FormulaConstants.EXP, "EXP", TokenPattern.PatternType.STRING, "^"));
            AddPattern(new TokenPattern((int)FormulaConstants.CONCAT, "CONCAT", TokenPattern.PatternType.STRING, "&"));
            AddPattern(new TokenPattern((int)FormulaConstants.LEFT_PAREN, "LEFT_PAREN", TokenPattern.PatternType.STRING,
                "("));
            AddPattern(new TokenPattern((int)FormulaConstants.RIGHT_PAREN, "RIGHT_PAREN",
                TokenPattern.PatternType.STRING, ")"));
            AddPattern(new TokenPattern((int)FormulaConstants.PERCENT, "PERCENT", TokenPattern.PatternType.STRING, "%"));

            AddPattern(new TokenPattern((int)FormulaConstants.WHITESPACE, "WHITESPACE", TokenPattern.PatternType.REGEXP,
                "\\s+")
            { Ignore = true });
            AddPattern(new TokenPattern((int)FormulaConstants.ARG_SEPARATOR, "ARG_SEPARATOR",
                TokenPattern.PatternType.STRING, Settings.ListSeparator));
            AddPattern(new TokenPattern((int)FormulaConstants.EQ, "EQ", TokenPattern.PatternType.STRING, "="));
            AddPattern(new TokenPattern((int)FormulaConstants.LT, "LT", TokenPattern.PatternType.STRING, "<"));
            AddPattern(new TokenPattern((int)FormulaConstants.GT, "GT", TokenPattern.PatternType.STRING, ">"));
            AddPattern(new TokenPattern((int)FormulaConstants.LTE, "LTE", TokenPattern.PatternType.STRING, "<="));
            AddPattern(new TokenPattern((int)FormulaConstants.GTE, "GTE", TokenPattern.PatternType.STRING, ">="));
            AddPattern(new TokenPattern((int)FormulaConstants.NE, "NE", TokenPattern.PatternType.STRING, "<>"));
            AddPattern(new TokenPattern((int)FormulaConstants.STRING_LITERAL, "STRING_LITERAL",
                TokenPattern.PatternType.REGEXP, "\"(\"\"|[^\"])*\""));

            var numberPattern = string.Concat("\\d+(\\", Settings.DecimalSeparator, "\\d+)?([e][+-]\\d{1,3})?");
            AddPattern(new TokenPattern((int)FormulaConstants.NUMBER, "NUMBER", TokenPattern.PatternType.REGEXP,
                numberPattern));
            AddPattern(new TokenPattern((int)FormulaConstants.TRUE, "TRUE", TokenPattern.PatternType.STRING, "True"));
            AddPattern(new TokenPattern((int)FormulaConstants.FALSE, "FALSE", TokenPattern.PatternType.STRING, "False"));
//            AddPattern(new TokenPattern((int)FormulaConstants.FUNCTION_NAME, "FUNCTION_NAME",
//                TokenPattern.PatternType.REGEXP, "[a-z][\\w]*\\("));
            AddPattern(new TokenPattern((int)FormulaConstants.FUNCTION_NAME, "FUNCTION_NAME",
                TokenPattern.PatternType.REGEXP, "[a-zа-яA-ZА-Я][a-zа-яA-ZА-Я]*\\("));
            AddPattern(new TokenPattern((int)FormulaConstants.DIV_ERROR, "DIV_ERROR", TokenPattern.PatternType.STRING,
                "#DIV/0!"));
            AddPattern(new TokenPattern((int)FormulaConstants.NA_ERROR, "NA_ERROR", TokenPattern.PatternType.STRING,
                "#N/A"));
            AddPattern(new TokenPattern((int)FormulaConstants.NAME_ERROR, "NAME_ERROR", TokenPattern.PatternType.STRING,
                "#NAME?"));
            AddPattern(new TokenPattern((int)FormulaConstants.NULL_ERROR, "NULL_ERROR", TokenPattern.PatternType.STRING,
                "#NULL!"));
            AddPattern(new TokenPattern((int)FormulaConstants.REF_ERROR, "REF_ERROR", TokenPattern.PatternType.STRING,
                "#REF!"));
            AddPattern(new TokenPattern((int)FormulaConstants.VALUE_ERROR, "VALUE_ERROR",
                TokenPattern.PatternType.STRING, "#VALUE!"));
            AddPattern(new TokenPattern((int)FormulaConstants.NUM_ERROR, "NUM_ERROR", TokenPattern.PatternType.STRING,
                "#NUM!"));
//            AddPattern(new TokenPattern((int)FormulaConstants.CELL, "CELL", TokenPattern.PatternType.REGEXP,
//                "\\$?[a-z]{1,2}\\$?\\d{1,5}"));
            AddPattern(new TokenPattern((int)FormulaConstants.CELL, "CELL", TokenPattern.PatternType.REGEXP,
                "\\$?[a-zа-яA-ZА-Я]{1,2}\\$?\\d{1,5}"));
            //AddPattern(new TokenPattern((int)FormulaConstants.CELL_RANGE, "CELL_RANGE", TokenPattern.PatternType.REGEXP,
            //    "\\$?[a-z]{1,2}\\$?\\d{1,5}:\\$?[a-z]{1,2}\\$?\\d{1,5}"));
            AddPattern(new TokenPattern((int)FormulaConstants.CELL_RANGE, "CELL_RANGE", TokenPattern.PatternType.REGEXP,
                "\\$?[a-zа-яA-ZА-Я]{1,2}\\$?\\d{1,5}:\\$?[a-zа-яA-ZА-Я]{1,2}\\$?\\d{1,5}"));
            AddPattern(new TokenPattern((int)FormulaConstants.ROW_RANGE, "ROW_RANGE", TokenPattern.PatternType.REGEXP,
                "\\$?\\d{1,5}:\\$?\\d{1,5}"));
            AddPattern(new TokenPattern((int)FormulaConstants.COLUMN_RANGE, "COLUMN_RANGE",
                TokenPattern.PatternType.REGEXP, "\\$?[a-zа-яA-ZА-Я]{1,2}:\\$?[a-zа-яA-ZА-Я]{1,2}"));
            //AddPattern(new TokenPattern((int)FormulaConstants.COLUMN_RANGE, "COLUMN_RANGE",
            //    TokenPattern.PatternType.REGEXP, "\\$?[a-z]{1,2}:\\$?[a-z]{1,2}"));
            AddPattern(new TokenPattern((int)FormulaConstants.SHEET_NAME, "SHEET_NAME", TokenPattern.PatternType.REGEXP,
                "[_a-zа-яA-ZА-Я][a-zа-яA-ZА-Я]*!"));
            AddPattern(new TokenPattern((int)FormulaConstants.DEFINED_NAME, "DEFINED_NAME",
                TokenPattern.PatternType.REGEXP, "[_a-zа-яA-ZА-Я][a-zа-яA-ZА-Я]*"));
        }
    }
}
using PerCederberg.Grammatica.Runtime;

namespace FormulaEngineCore.Creators
{
    ///<remarks>A class providing callback methods for theparser.</remarks>
    internal abstract class FormulaAnalyzer : Analyzer
    {

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public override void Enter(Node node)
        {
            switch ((FormulaConstants)node.Id)
            {
                case FormulaConstants.ADD:
                    EnterAdd((Token)node);
                    break;
                case FormulaConstants.SUB:

                    EnterSub((Token)node);
                    break;
                case FormulaConstants.MUL:

                    EnterMul((Token)node);
                    break;
                case FormulaConstants.DIV:

                    EnterDiv((Token)node);
                    break;
                case FormulaConstants.EXP:

                    EnterExp((Token)node);
                    break;
                case FormulaConstants.CONCAT:

                    EnterConcat((Token)node);
                    break;
                case FormulaConstants.LEFT_PAREN:

                    EnterLeftParen((Token)node);
                    break;
                case FormulaConstants.RIGHT_PAREN:

                    EnterRightParen((Token)node);
                    break;
                case FormulaConstants.PERCENT:

                    EnterPercent((Token)node);
                    break;
                case FormulaConstants.ARG_SEPARATOR:

                    EnterArgSeparator((Token)node);
                    break;
                case FormulaConstants.EQ:

                    EnterEq((Token)node);
                    break;
                case FormulaConstants.LT:

                    EnterLt((Token)node);
                    break;
                case FormulaConstants.GT:

                    EnterGt((Token)node);
                    break;
                case FormulaConstants.LTE:

                    EnterLte((Token)node);
                    break;
                case FormulaConstants.GTE:

                    EnterGte((Token)node);
                    break;
                case FormulaConstants.NE:

                    EnterNe((Token)node);
                    break;
                case FormulaConstants.STRING_LITERAL:

                    EnterStringLiteral((Token)node);
                    break;
                case FormulaConstants.NUMBER:

                    EnterNumber((Token)node);
                    break;
                case FormulaConstants.TRUE:

                    EnterTrue((Token)node);
                    break;
                case FormulaConstants.FALSE:

                    EnterFalse((Token)node);
                    break;
                case FormulaConstants.FUNCTION_NAME:

                    EnterFunctionName((Token)node);
                    break;
                case FormulaConstants.DIV_ERROR:

                    EnterDivError((Token)node);
                    break;
                case FormulaConstants.NA_ERROR:

                    EnterNaError((Token)node);
                    break;
                case FormulaConstants.NAME_ERROR:

                    EnterNameError((Token)node);
                    break;
                case FormulaConstants.NULL_ERROR:

                    EnterNullError((Token)node);
                    break;
                case FormulaConstants.REF_ERROR:

                    EnterRefError((Token)node);
                    break;
                case FormulaConstants.VALUE_ERROR:

                    EnterValueError((Token)node);
                    break;
                case FormulaConstants.NUM_ERROR:

                    EnterNumError((Token)node);
                    break;
                case FormulaConstants.CELL:

                    EnterCell((Token)node);
                    break;
                case FormulaConstants.CELL_RANGE:

                    EnterCellRange((Token)node);
                    break;
                case FormulaConstants.ROW_RANGE:

                    EnterRowRange((Token)node);
                    break;
                case FormulaConstants.COLUMN_RANGE:

                    EnterColumnRange((Token)node);
                    break;
                case FormulaConstants.SHEET_NAME:

                    EnterSheetName((Token)node);
                    break;
                case FormulaConstants.DEFINED_NAME:

                    EnterDefinedName((Token)node);
                    break;
                case FormulaConstants.FORMULA:

                    EnterFormula((Production)node);
                    break;
                case FormulaConstants.SCALAR_FORMULA:

                    EnterScalarFormula((Production)node);
                    break;
                case FormulaConstants.PRIMARY_EXPRESSION:

                    EnterPrimaryExpression((Production)node);
                    break;
                case FormulaConstants.EXPRESSION:

                    EnterExpression((Production)node);
                    break;
                case FormulaConstants.LOGICAL_EXPRESSION:

                    EnterLogicalExpression((Production)node);
                    break;
                case FormulaConstants.LOGICAL_OP:

                    EnterLogicalOp((Production)node);
                    break;
                case FormulaConstants.CONCAT_EXPRESSION:

                    EnterConcatExpression((Production)node);
                    break;
                case FormulaConstants.ADDITIVE_EXPRESSION:

                    EnterAdditiveExpression((Production)node);
                    break;
                case FormulaConstants.ADDITIVE_OP:

                    EnterAdditiveOp((Production)node);
                    break;
                case FormulaConstants.MULTIPLICATIVE_EXPRESSION:

                    EnterMultiplicativeExpression((Production)node);
                    break;
                case FormulaConstants.MULTIPLICATIVE_OP:

                    EnterMultiplicativeOp((Production)node);
                    break;
                case FormulaConstants.EXPONENTIATION_EXPRESSION:

                    EnterExponentiationExpression((Production)node);
                    break;
                case FormulaConstants.PERCENT_EXPRESSION:

                    EnterPercentExpression((Production)node);
                    break;
                case FormulaConstants.UNARY_EXPRESSION:

                    EnterUnaryExpression((Production)node);
                    break;
                case FormulaConstants.BASIC_EXPRESSION:

                    EnterBasicExpression((Production)node);
                    break;
                case FormulaConstants.REFERENCE:

                    EnterReference((Production)node);
                    break;
                case FormulaConstants.GRID_REFERENCE_EXPRESSION:

                    EnterGridReferenceExpression((Production)node);
                    break;
                case FormulaConstants.GRID_REFERENCE:

                    EnterGridReference((Production)node);
                    break;
                case FormulaConstants.FUNCTION_CALL:

                    EnterFunctionCall((Production)node);
                    break;
                case FormulaConstants.ARGUMENT_LIST:

                    EnterArgumentList((Production)node);
                    break;
                case FormulaConstants.EXPRESSION_GROUP:

                    EnterExpressionGroup((Production)node);
                    break;
                case FormulaConstants.PRIMITIVE:

                    EnterPrimitive((Production)node);
                    break;
                case FormulaConstants.BOOLEAN:

                    EnterBoolean((Production)node);
                    break;
                case FormulaConstants.ERROR_EXPRESSION:

                    EnterErrorExpression((Production)node);
                    break;
            }
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public override Node Exit(Node node)
        {
            switch ((FormulaConstants)node.Id)
            {
                case FormulaConstants.ADD:

                    return ExitAdd((Token)node);
                case FormulaConstants.SUB:

                    return ExitSub((Token)node);
                case FormulaConstants.MUL:

                    return ExitMul((Token)node);
                case FormulaConstants.DIV:

                    return ExitDiv((Token)node);
                case FormulaConstants.EXP:

                    return ExitExp((Token)node);
                case FormulaConstants.CONCAT:

                    return ExitConcat((Token)node);
                case FormulaConstants.LEFT_PAREN:

                    return ExitLeftParen((Token)node);
                case FormulaConstants.RIGHT_PAREN:

                    return ExitRightParen((Token)node);
                case FormulaConstants.PERCENT:

                    return ExitPercent((Token)node);
                case FormulaConstants.ARG_SEPARATOR:

                    return ExitArgSeparator((Token)node);
                case FormulaConstants.EQ:

                    return ExitEq((Token)node);
                case FormulaConstants.LT:

                    return ExitLt((Token)node);
                case FormulaConstants.GT:

                    return ExitGt((Token)node);
                case FormulaConstants.LTE:

                    return ExitLte((Token)node);
                case FormulaConstants.GTE:

                    return ExitGte((Token)node);
                case FormulaConstants.NE:

                    return ExitNe((Token)node);
                case FormulaConstants.STRING_LITERAL:

                    return ExitStringLiteral((Token)node);
                case FormulaConstants.NUMBER:

                    return ExitNumber((Token)node);
                case FormulaConstants.TRUE:

                    return ExitTrue((Token)node);
                case FormulaConstants.FALSE:

                    return ExitFalse((Token)node);
                case FormulaConstants.FUNCTION_NAME:

                    return ExitFunctionName((Token)node);
                case FormulaConstants.DIV_ERROR:

                    return ExitDivError((Token)node);
                case FormulaConstants.NA_ERROR:

                    return ExitNaError((Token)node);
                case FormulaConstants.NAME_ERROR:

                    return ExitNameError((Token)node);
                case FormulaConstants.NULL_ERROR:

                    return ExitNullError((Token)node);
                case FormulaConstants.REF_ERROR:

                    return ExitRefError((Token)node);
                case FormulaConstants.VALUE_ERROR:

                    return ExitValueError((Token)node);
                case FormulaConstants.NUM_ERROR:

                    return ExitNumError((Token)node);
                case FormulaConstants.CELL:

                    return ExitCell((Token)node);
                case FormulaConstants.CELL_RANGE:

                    return ExitCellRange((Token)node);
                case FormulaConstants.ROW_RANGE:

                    return ExitRowRange((Token)node);
                case FormulaConstants.COLUMN_RANGE:

                    return ExitColumnRange((Token)node);
                case FormulaConstants.SHEET_NAME:

                    return ExitSheetName((Token)node);
                case FormulaConstants.DEFINED_NAME:

                    return ExitDefinedName((Token)node);
                case FormulaConstants.FORMULA:

                    return ExitFormula((Production)node);
                case FormulaConstants.SCALAR_FORMULA:

                    return ExitScalarFormula((Production)node);
                case FormulaConstants.PRIMARY_EXPRESSION:

                    return ExitPrimaryExpression((Production)node);
                case FormulaConstants.EXPRESSION:

                    return ExitExpression((Production)node);
                case FormulaConstants.LOGICAL_EXPRESSION:

                    return ExitLogicalExpression((Production)node);
                case FormulaConstants.LOGICAL_OP:

                    return ExitLogicalOp((Production)node);
                case FormulaConstants.CONCAT_EXPRESSION:

                    return ExitConcatExpression((Production)node);
                case FormulaConstants.ADDITIVE_EXPRESSION:

                    return ExitAdditiveExpression((Production)node);
                case FormulaConstants.ADDITIVE_OP:

                    return ExitAdditiveOp((Production)node);
                case FormulaConstants.MULTIPLICATIVE_EXPRESSION:

                    return ExitMultiplicativeExpression((Production)node);
                case FormulaConstants.MULTIPLICATIVE_OP:

                    return ExitMultiplicativeOp((Production)node);
                case FormulaConstants.EXPONENTIATION_EXPRESSION:

                    return ExitExponentiationExpression((Production)node);
                case FormulaConstants.PERCENT_EXPRESSION:

                    return ExitPercentExpression((Production)node);
                case FormulaConstants.UNARY_EXPRESSION:

                    return ExitUnaryExpression((Production)node);
                case FormulaConstants.BASIC_EXPRESSION:

                    return ExitBasicExpression((Production)node);
                case FormulaConstants.REFERENCE:

                    return ExitReference((Production)node);
                case FormulaConstants.GRID_REFERENCE_EXPRESSION:

                    return ExitGridReferenceExpression((Production)node);
                case FormulaConstants.GRID_REFERENCE:

                    return ExitGridReference((Production)node);
                case FormulaConstants.FUNCTION_CALL:

                    return ExitFunctionCall((Production)node);
                case FormulaConstants.ARGUMENT_LIST:

                    return ExitArgumentList((Production)node);
                case FormulaConstants.EXPRESSION_GROUP:

                    return ExitExpressionGroup((Production)node);
                case FormulaConstants.PRIMITIVE:

                    return ExitPrimitive((Production)node);
                case FormulaConstants.BOOLEAN:

                    return ExitBoolean((Production)node);
                case FormulaConstants.ERROR_EXPRESSION:

                    return ExitErrorExpression((Production)node);
            }
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child__1'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public override void Child(Production node, Node child__1)
        {
            switch ((FormulaConstants)node.Id)
            {
                case FormulaConstants.FORMULA:

                    ChildFormula(node, child__1);
                    break;
                case FormulaConstants.SCALAR_FORMULA:

                    ChildScalarFormula(node, child__1);
                    break;
                case FormulaConstants.PRIMARY_EXPRESSION:

                    ChildPrimaryExpression(node, child__1);
                    break;
                case FormulaConstants.EXPRESSION:

                    ChildExpression(node, child__1);
                    break;
                case FormulaConstants.LOGICAL_EXPRESSION:

                    ChildLogicalExpression(node, child__1);
                    break;
                case FormulaConstants.LOGICAL_OP:

                    ChildLogicalOp(node, child__1);
                    break;
                case FormulaConstants.CONCAT_EXPRESSION:

                    ChildConcatExpression(node, child__1);
                    break;
                case FormulaConstants.ADDITIVE_EXPRESSION:

                    ChildAdditiveExpression(node, child__1);
                    break;
                case FormulaConstants.ADDITIVE_OP:

                    ChildAdditiveOp(node, child__1);
                    break;
                case FormulaConstants.MULTIPLICATIVE_EXPRESSION:

                    ChildMultiplicativeExpression(node, child__1);
                    break;
                case FormulaConstants.MULTIPLICATIVE_OP:

                    ChildMultiplicativeOp(node, child__1);
                    break;
                case FormulaConstants.EXPONENTIATION_EXPRESSION:

                    ChildExponentiationExpression(node, child__1);
                    break;
                case FormulaConstants.PERCENT_EXPRESSION:

                    ChildPercentExpression(node, child__1);
                    break;
                case FormulaConstants.UNARY_EXPRESSION:

                    ChildUnaryExpression(node, child__1);
                    break;
                case FormulaConstants.BASIC_EXPRESSION:

                    ChildBasicExpression(node, child__1);
                    break;
                case FormulaConstants.REFERENCE:

                    ChildReference(node, child__1);
                    break;
                case FormulaConstants.GRID_REFERENCE_EXPRESSION:

                    ChildGridReferenceExpression(node, child__1);
                    break;
                case FormulaConstants.GRID_REFERENCE:

                    ChildGridReference(node, child__1);
                    break;
                case FormulaConstants.FUNCTION_CALL:

                    ChildFunctionCall(node, child__1);
                    break;
                case FormulaConstants.ARGUMENT_LIST:

                    ChildArgumentList(node, child__1);
                    break;
                case FormulaConstants.EXPRESSION_GROUP:

                    ChildExpressionGroup(node, child__1);
                    break;
                case FormulaConstants.PRIMITIVE:

                    ChildPrimitive(node, child__1);
                    break;
                case FormulaConstants.BOOLEAN:

                    ChildBoolean(node, child__1);
                    break;
                case FormulaConstants.ERROR_EXPRESSION:

                    ChildErrorExpression(node, child__1);
                    break;
            }
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterAdd(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitAdd(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterSub(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitSub(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterMul(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitMul(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterDiv(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitDiv(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterExp(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitExp(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterConcat(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitConcat(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterLeftParen(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitLeftParen(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterRightParen(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitRightParen(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterPercent(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitPercent(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterArgSeparator(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitArgSeparator(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterEq(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitEq(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterLt(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitLt(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterGt(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitGt(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterLte(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitLte(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterGte(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitGte(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterNe(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitNe(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterStringLiteral(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitStringLiteral(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterNumber(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitNumber(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterTrue(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitTrue(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterFalse(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitFalse(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterFunctionName(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitFunctionName(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterDivError(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitDivError(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterNaError(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitNaError(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterNameError(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitNameError(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterNullError(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitNullError(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterRefError(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitRefError(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterValueError(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitValueError(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterNumError(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitNumError(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterCell(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitCell(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterCellRange(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitCellRange(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterRowRange(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitRowRange(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterColumnRange(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitColumnRange(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterSheetName(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitSheetName(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterDefinedName(Token node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitDefinedName(Token node)
        {
            return node;
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterFormula(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitFormula(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildFormula(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterScalarFormula(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitScalarFormula(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildScalarFormula(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterPrimaryExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitPrimaryExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildPrimaryExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterLogicalExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitLogicalExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildLogicalExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterLogicalOp(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitLogicalOp(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildLogicalOp(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterConcatExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitConcatExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildConcatExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterAdditiveExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitAdditiveExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildAdditiveExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterAdditiveOp(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitAdditiveOp(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildAdditiveOp(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterMultiplicativeExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitMultiplicativeExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildMultiplicativeExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterMultiplicativeOp(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitMultiplicativeOp(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildMultiplicativeOp(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterExponentiationExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitExponentiationExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildExponentiationExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterPercentExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitPercentExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildPercentExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterUnaryExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitUnaryExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildUnaryExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterBasicExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitBasicExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildBasicExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterReference(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitReference(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildReference(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterGridReferenceExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitGridReferenceExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildGridReferenceExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterGridReference(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitGridReference(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildGridReference(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterFunctionCall(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitFunctionCall(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildFunctionCall(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterArgumentList(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitArgumentList(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildArgumentList(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterExpressionGroup(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitExpressionGroup(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildExpressionGroup(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterPrimitive(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitPrimitive(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildPrimitive(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterBoolean(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitBoolean(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildBoolean(Production node, Node child)
        {
            node.AddChild(child);
        }

        ///<summary>Called when entering a parse tree node.</summary>
        ///
        ///<param name='node'>the node being entered</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void EnterErrorExpression(Production node)
        {
        }

        ///<summary>Called when exiting a parse tree node.</summary>
        ///
        ///<param name='node'>the node being exited</param>
        ///
        ///<returns>the node to add to the parse tree, or
        ///         null if no parse tree should be created</returns>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual Node ExitErrorExpression(Production node)
        {
            return node;
        }

        ///<summary>Called when adding a child to a parse tree
        ///node.</summary>
        ///
        ///<param name='node'>the parent node</param>
        ///<param name='child'>the child node, or null</param>
        ///
        ///<exception cref='ParseException'>if the node analysis
        ///discovered errors</exception>
        public virtual void ChildErrorExpression(Production node, Node child)
        {
            node.AddChild(child);
        }
    }
}

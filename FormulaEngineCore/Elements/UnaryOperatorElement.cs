// Classes for converting a parse tree into a postfix expression

using System.Collections;
using FormulaEngineCore.Operators;

namespace FormulaEngineCore.Elements
{
    internal class UnaryOperatorElement : ParseTreeElement
    {
        private readonly ParseTreeElement _argument;
        private readonly UnaryOperator _operator; 

        public UnaryOperatorElement(UnaryOperator @operator, ParseTreeElement argument)
        {
            _operator = @operator;
            _argument = argument;
        }

        public override void AddAsRPN(IList dest)
        {
            _argument.AddAsRPN(dest);
            dest.Add(_operator);
        }
    }
}
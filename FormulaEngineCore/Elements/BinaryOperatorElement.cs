using System.Collections;
using System.Diagnostics;
using FormulaEngineCore.Operators;

namespace FormulaEngineCore.Elements
{
    internal class BinaryOperatorElement : ParseTreeElement
    {
        private ParseTreeElement[] _args;

        private BinaryOperator[] _operations;

        public void AcceptValues(IList values)
        {
            Debug.Assert(values.Count >= 2, "must have at least 2 values");

            _args = new ParseTreeElement[(values.Count + 1) / 2];
            _operations = new BinaryOperator[(values.Count - 1) / 2];

            int index = 0;

            for (int i = 0; i <= values.Count - 1; i += 2)
            {
                _args[index] = values[i] as ParseTreeElement;
                index += 1;
            }

            index = 0;

            for (int i = 1; i <= values.Count - 1; i += 2)
            {
                _operations[index] = values[i] as BinaryOperator;
                index += 1;
            }
        }

        public override void AddAsRPN(IList dest)
        {
            ParseTreeElement element = _args[0];
            element.AddAsRPN(dest);
            element = _args[1];
            element.AddAsRPN(dest);
            dest.Add(_operations[0]);

            int opIndex = 1;

            for (int i = 2; i <= _args.Length - 1; i++)
            {
                element = _args[i];
                element.AddAsRPN(dest);
                dest.Add(_operations[opIndex]);
                opIndex += 1;
            }
        }
    }
}
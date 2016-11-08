using System;
using System.Collections;
using FormulaEngineCore.Operators;

namespace FormulaEngineCore.Elements
{
    internal class FunctionCallElement : ParseTreeElement
    {
        private ParseTreeElement[] _args;
        private string _functionName;

        public void AcceptValues(IList values)
        {
            var functionName = (string)values[0];

            values.RemoveAt(0);

            var arr = new ParseTreeElement[values.Count];
            values.CopyTo(arr, 0);
            Array.Reverse(arr);

            _args = arr;
            _functionName = functionName;
        }

        public override void AddAsRPN(IList dest)
        {
            foreach (ParseTreeElement element in _args)
            {
                element.AddAsRPN(dest);
            }

            var funcCall = new FunctionCallOperator(_functionName, _args.Length);
            dest.Add(funcCall);
        }
    }
}
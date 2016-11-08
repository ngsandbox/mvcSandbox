using System.Collections;

namespace FormulaEngineCore.Elements
{
    internal class ContainerElement : ParseTreeElement
    {
        private readonly object _value;

        public ContainerElement(object value)
        {
            _value = value;
        }

        public override void AddAsRPN(IList dest)
        {
            dest.Add(_value);
        }
    }
}
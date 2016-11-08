using System.Collections;

namespace FormulaEngineCore.Elements
{
    internal abstract class ParseTreeElement
    {
        public abstract void AddAsRPN(IList dest);
    }
}
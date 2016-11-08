namespace FormulaEngineCore.Processors
{
    internal abstract class LookupProcessor
    {
        public abstract object[] GetLookupVector();
        public abstract object[] GetResultVector();
    }
}
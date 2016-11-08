namespace FormulaEngineCore.Processors
{
    internal abstract class ConditionalSheetProcessor
    {
        public abstract void OnMatch(int row, int col);
    }
}
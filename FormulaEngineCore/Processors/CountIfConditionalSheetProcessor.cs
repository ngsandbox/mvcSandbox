namespace FormulaEngineCore.Processors
{
    internal class CountIfConditionalSheetProcessor : ConditionalSheetProcessor
    {
        public int Count { get; private set; }

        public override void OnMatch(int row, int col)
        {
            Count += 1;
        }
    }
}
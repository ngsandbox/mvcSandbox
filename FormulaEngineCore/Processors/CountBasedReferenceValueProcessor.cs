namespace FormulaEngineCore.Processors
{
    /// <summary>
    ///     Processor that keeps a count of values
    /// </summary>
    internal abstract class CountBasedReferenceValueProcessor : VariableArgumentFunctionProcessor
    {
        public int Count { get; private set; }

        protected override bool StopOnError
        {
            get { return false; }
            set { }
        }

        protected void IncrementCount()
        {
            Count += 1;
        }
    }
}
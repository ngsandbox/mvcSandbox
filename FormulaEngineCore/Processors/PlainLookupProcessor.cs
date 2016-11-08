namespace FormulaEngineCore.Processors
{
    internal class PlainLookupProcessor : LookupProcessor
    {
        private object[] _lookupVector;

        private object[] _resultVector;

        public void Initialize(object[] lookupVector, object[] resultVector)
        {
            _lookupVector = lookupVector;
            _resultVector = resultVector;
        }

        public override object[] GetLookupVector()
        {
            return _lookupVector;
        }

        public override object[] GetResultVector()
        {
            return _resultVector;
        }
    }
}
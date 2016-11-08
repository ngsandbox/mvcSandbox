namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Implemented by references that can have a formula bound to them
    /// </summary>
    internal interface IFormulaSelfReference
    {
        void OnFormulaRecalculate(Formula target);
    }
}
using System;
using System.Collections;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Implemented by objects that compute a formula's result
    /// </summary>
    internal interface IFormulaComponent : ICloneable
    {
        void Evaluate(Stack state, FormulaEngine engine);
        void EvaluateForDependencyReference(IList references, FormulaEngine engine);
        void Validate(FormulaEngine engine);
    }
}
using System;
using FormulaEngineCore.FormulaRererences;

namespace FormulaEngineCore.FormulaTypes
{
    [Serializable]
    internal struct ReferencePoolInfo
    {
        public int Count;
        public Reference Target;

        public ReferencePoolInfo(Reference target)
        {
            Target = target;
            Count = 1;
        }
    }
}
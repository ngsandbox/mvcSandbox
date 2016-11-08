using System;
using System.Collections.Generic;
using FormulaEngineCore.FormulaRererences;

namespace FormulaEngineCore.Miscellaneous
{
    [Serializable]
    internal class ReferenceEqualityComparer : IEqualityComparer<Reference>
    {
        public bool Equals(Reference x, Reference y)
        {
            return x.EqualsReference(y);
        }

        public int GetHashCode(Reference obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals1(Reference x, Reference y)
        {
            return x.EqualsReference(y);
        }

        public int GetHashCode1(Reference obj)
        {
            return obj.GetReferenceHashCode();
        }
    }
}
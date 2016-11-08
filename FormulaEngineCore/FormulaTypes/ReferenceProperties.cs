using System;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Base class for properties that are specific to a reference
    /// </summary>
    [Serializable]
    public abstract class ReferenceProperties : ICloneable
    {
        public object Clone()
        {
            object copy = MemberwiseClone();
            InitializeClone(copy as ReferenceProperties);
            return copy;
        }


        protected virtual void InitializeClone(ReferenceProperties clone)
        {
        }
    }
}
using System;
using System.Collections;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    /// <summary>
    ///     Base class for all operators
    /// </summary>
    [Serializable]
    internal abstract class OperatorBase : IFormulaComponent, ISerializable
    {
        private const int VERSION = 1;

        protected OperatorBase()
        {
        }


        protected OperatorBase(SerializationInfo info, StreamingContext context)
        {
        }

        public abstract void Evaluate(Stack state, FormulaEngine engine);

        public object Clone()
        {
            return MemberwiseClone();
        }


        public virtual void EvaluateForDependencyReference(IList references, FormulaEngine engine)
        {
        }


        public virtual void Validate(FormulaEngine engine)
        {
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
        }

        public static bool IsInvalidDouble(double d)
        {
            return double.IsInfinity(d) || double.IsNaN(d);
        }
    }
}
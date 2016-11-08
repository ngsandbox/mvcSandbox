// Various classes that don't deserve an invididual source file

using System.Collections;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal abstract class ReferenceOperator
    {
        public virtual void PreOperate(IList references)
        {
        }


        public virtual void PostOperate(IList references)
        {
        }

        public abstract ReferenceOperationResultType Operate(Reference @ref);
    }
}
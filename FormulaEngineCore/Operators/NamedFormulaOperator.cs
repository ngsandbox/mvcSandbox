using System;
using System.Collections;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class NamedFormulaOperator : OperatorBase
    {
        private readonly string _name;

        public NamedFormulaOperator(string name)
        {
            _name = name;
        }

        private NamedFormulaOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _name = info.GetString("Name");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Name", _name);
        }

        public override void Evaluate(Stack state, FormulaEngine engine)
        {
            var namedRef = (Reference) engine.ReferenceFactory.Named(_name);
            Formula target = engine.GetFormulaAt(namedRef);

            if (target != null)
            {
                var selfRef = (NamedReference) target.SelfReference;
                IOperand op = selfRef.ValueOperand;
                state.Push(op);
            }
            else
            {
                state.Push(new ErrorValueOperand(ErrorValueType.Name));
            }
        }

        public override void EvaluateForDependencyReference(IList references, FormulaEngine engine)
        {
            var namedRef = (Reference) engine.ReferenceFactory.Named(_name);
            references.Add(namedRef);
        }
    }
}
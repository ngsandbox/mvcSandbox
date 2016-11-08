using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.FormulaRererences
{
    [Serializable]
    internal class NamedReference : NonGridReference, IFormulaSelfReference, INamedReference
    {
        private static readonly Regex _regex = new Regex("^[_a-zа-яA-ZА-Я][_0-9a-zа-яA-ZА-Я]*$");

        public NamedReference(string name)
        {
            Name = name;
            ValueOperand = new NullValueOperand();
        }

        private NamedReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Name = info.GetString("Name");
            ValueOperand = (IOperand)info.GetValue("ValueOperand", typeof(IOperand));
            ComputeHashCode();
        }

        public object OperandValue
        {
            get { return ValueOperand.Value; }
            set { ValueOperand = OperandFactory.CreateDynamic(value); }
        }

        public IOperand ValueOperand { get; private set; }

        public void OnFormulaRecalculate(Formula target)
        {
            ValueOperand = target.EvaluateToOperand();
        }

        public string Name { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Name", Name);
            info.AddValue("ValueOperand", ValueOperand);
        }

        public static bool IsValidName(string name)
        {
            return _regex.IsMatch(name);
        }

        protected override bool EqualsReferenceInternal(Reference @ref)
        {
            var realRef = (NamedReference)@ref;
            return String.Equals(Name, realRef.Name, StringComparison.OrdinalIgnoreCase);
        }

        protected override string Format()
        {
            return Name;
        }

        protected override byte[] GetHashData()
        {
            string nameLower = Name.ToLowerInvariant();
            byte[] nameBytes = Encoding.ASCII.GetBytes(nameLower);

            var bytes = new byte[REFERENCE_HASH_SIZE + nameBytes.Length];
            GetBaseHashData(bytes);
            Array.Copy(nameBytes, 0, bytes, REFERENCE_HASH_SIZE, nameBytes.Length);
            return bytes;
        }
    }
}
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Creators
{
    internal abstract class ReferenceCreator
    {
        protected string _image;

        public void Initialize(string image)
        {
            _image = image;
        }

        public abstract Reference CreateReference();
        public abstract ReferenceProperties CreateProperties(bool implicitSheet);
    }
}
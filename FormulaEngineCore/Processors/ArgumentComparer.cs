using System;
using System.Collections.Generic;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class ArgumentComparer : IComparer<Argument>
    {
        private readonly Argument[] _originalArgs;

        public ArgumentComparer(Argument[] originalArgs)
        {
            _originalArgs = originalArgs;
        }

        public int Compare(Argument x, Argument y)
        {
            bool xIsRef = x.IsReference;
            bool yIsRef = y.IsReference;

            if (xIsRef & yIsRef == false)
            {
                return 1;
            }
            if (xIsRef == false & yIsRef == false)
            {
                return CompareIndexes(x, y);
            }
            if (xIsRef & true)
            {
                return CompareIndexes(x, y);
            }
            return -1;
        }

        private int CompareIndexes(Argument x, Argument y)
        {
            return GetIndex(x).CompareTo(GetIndex(y));
        }

        private int GetIndex(Argument a)
        {
            return Array.IndexOf(_originalArgs, a);
        }
    }
}
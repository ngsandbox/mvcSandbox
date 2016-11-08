using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operators;

namespace FormulaEngineCore
{
    /// <summary>
    ///     Maintains a pool of all references.  This allows formulas to re-use instances of a reference so that when a
    ///     reference changes, all formulas
    ///     that use it will see the change.  It uses simple reference counting to ensure that unused references are released.
    /// </summary>
    [Serializable]
    internal class ReferencePool : ISerializable
    {
        private const int VERSION = 1;
        private readonly FormulaEngine _owner;
        private readonly IDictionary<Reference, ReferencePoolInfo> _referenceMap;

        public ReferencePool(FormulaEngine owner)
        {
            _referenceMap = new Dictionary<Reference, ReferencePoolInfo>(new ReferenceEqualityComparer());
            _owner = owner;
        }

        private ReferencePool(SerializationInfo info, StreamingContext context)
        {
            _owner = (FormulaEngine) info.GetValue("Engine", typeof (FormulaEngine));
            _referenceMap =
                (IDictionary<Reference, ReferencePoolInfo>)
                    info.GetValue("Map", typeof (IDictionary<Reference, ReferencePoolInfo>));
        }

        public int ReferenceCount
        {
            get { return _referenceMap.Count; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
            info.AddValue("Engine", _owner);
            info.AddValue("Map", _referenceMap);
        }

        public Reference GetReference(Reference target)
        {
            Debug.Assert(target.Valid, "expecting reference to be valid");
            Reference @ref = GetPooledReference(target);
            if (@ref == null)
            {
                AddReference(target);
                return target;
            }
            IncrementReferenceCount(@ref);
            return @ref;
        }

        private void IncrementReferenceCount(Reference @ref)
        {
            ReferencePoolInfo info = _referenceMap[@ref];
            info.Count += 1;
            _referenceMap[@ref] = info;
        }

        private void AddReference(Reference @ref)
        {
            var info = new ReferencePoolInfo(@ref);
            _referenceMap.Add(@ref, info);
            @ref.SetEngine(_owner);
        }

        public void RemoveReference(Reference @ref)
        {
            if (@ref.Valid == false)
            {
                return;
            }
            if (_referenceMap.ContainsKey(@ref) == false)
            {
                throw new ArgumentException("Reference is not in pool");
            }

            ReferencePoolInfo info = _referenceMap[@ref];
            info.Count -= 1;

            if (info.Count == 0)
            {
                _referenceMap.Remove(@ref);
            }
            else
            {
                _referenceMap[@ref] = info;
            }
        }

        public Reference GetPooledReference(Reference target)
        {
            if (_referenceMap.ContainsKey(target))
            {
                ReferencePoolInfo info = _referenceMap[target];
                return info.Target;
            }
            return null;
        }

        public void Clear()
        {
            _referenceMap.Clear();
        }

        /// <summary>
        ///     Perform an operation on a set of references
        /// </summary>
        public void DoReferenceOperation(IList targets, ReferenceOperator refOp)
        {
            // Keep track of references in various states
            IList affectedValid = new ArrayList();
            IList invalid = new ArrayList();
            IList affected = new ArrayList();
            refOp.PreOperate(targets);

            foreach (Reference @ref in targets)
            {
                ReferenceOperationResultType result = refOp.Operate(@ref);

                if (result != ReferenceOperationResultType.NotAffected)
                {
                    // Note references that are affected (valid or not)
                    affected.Add(@ref);
                }

                if (result == ReferenceOperationResultType.Invalidated)
                {
                    // Note invalid references
                    invalid.Add(@ref);
                }
                else if (result == ReferenceOperationResultType.Affected)
                {
                    // Note affected but valid references
                    affectedValid.Add(@ref);
                }
            }

            ProcessInvalidReferences(invalid);
            ProcessAffectedValidReferences(affectedValid);
            _owner.RemoveInvalidFormulas(invalid);
            _owner.RecalculateAffectedReferences(affected);
            refOp.PostOperate(affectedValid);
        }

        private void ProcessInvalidReferences(IList invalid)
        {
            foreach (Reference @ref in invalid)
            {
                _referenceMap.Remove(@ref);
                @ref.MarkAsInvalid();
            }
        }

// Go through all references that were affected by an operation and rehash them
        private void ProcessAffectedValidReferences(IList affectedValid)
        {
            foreach (Reference @ref in affectedValid)
            {
                RehashReference(@ref);
            }
        }

        private void RehashReference(Reference @ref)
        {
            ReferencePoolInfo info = _referenceMap[@ref];
            _referenceMap.Remove(@ref);
            @ref.ComputeHashCode();
            _referenceMap.Add(@ref, info);
        }

        public IList FindReferences(ReferencePredicateBase predicate)
        {
            IList found = new ArrayList();

            foreach (Reference @ref in _referenceMap.Keys)
            {
                // Ignore invalid references
                if (@ref.Valid && predicate.IsMatch(@ref))
                {
                    found.Add(@ref);
                }
            }

            return found;
        }
    }
}
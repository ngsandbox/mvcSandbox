using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore
{
    /// <summary>
    ///     Manages formula dependencies
    /// </summary>
    [Serializable]
    internal class DependencyManager : ISerializable
    {
        private const int VERSION = 1;
        private readonly DependencyMap _dependents;
        private readonly FormulaEngine _owner;
        private readonly DependencyMap _precedents;
        private bool _suspendRangeLinks;

        public DependencyManager(FormulaEngine owner)
        {
            _owner = owner;
            _dependents = new DependencyMap();
            _precedents = new DependencyMap();
        }

        private DependencyManager(SerializationInfo info, StreamingContext context)
        {
            _dependents = (DependencyMap)info.GetValue("Dependents", typeof(DependencyMap));
            _precedents = (DependencyMap)info.GetValue("Precedents", typeof(DependencyMap));
            _owner = (FormulaEngine)info.GetValue("Engine", typeof(FormulaEngine));
        }

        public int DependentsCount { get { return _dependents.DependencyCount; } }

        public int PrecedentsCount { get { return _precedents.DependencyCount; } }

        public string DependencyDump { get { return _dependents.ToString(); } }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
            info.AddValue("Dependents", _dependents);
            info.AddValue("Precedents", _precedents);
            info.AddValue("Engine", _owner);
        }

        /// <summary>
        ///     Add all dependencies of a formula
        /// </summary>
        public void AddFormula(Formula f)
        {
            RemoveRangeLinks();

            // Add dependencies from the formula's self reference to each of its references
            Reference selfRef = f.SelfReference;
            Reference[] refs = f.DependencyReferences;

            for (int i = 0; i <= refs.Length - 1; i++)
            {
                Reference @ref = refs[i];
                _dependents.AddDependency(@ref, selfRef);
                _precedents.AddDependency(selfRef, @ref);
            }

            AddRangeLinks();
        }

        /// <summary>
        ///     Remove all dependencies of a formula
        /// </summary>
        public void RemoveFormula(Formula f)
        {
            Reference selfRef = f.SelfReference;
            RemoveRangeLinks();

            foreach (Reference @ref in f.DependencyReferences)
            {
                RemoveDependency(@ref, selfRef, _dependents, _precedents);
            }

            AddRangeLinks();
        }

        public bool FormulaHasCircularReference(Formula f)
        {
            return IsCircularReference(f.SelfReference);
        }

        public bool IsCircularReference(Reference @ref)
        {
            return _dependents.IsCircularReference(@ref);
        }

        public void AddRangeLinks()
        {
            if (_suspendRangeLinks == false)
            {
                ProcessRangeLinks(DoAddIntersectingDependency);
            }
        }

        public void RemoveRangeLinks()
        {
            if (_suspendRangeLinks == false)
            {
                ProcessRangeLinks(DoRemoveIntersectingDependency);
            }
        }

        /// <summary>
        ///     Creates or removes range links
        /// </summary>
        /// <remarks>
        ///     A range link is an optimization for dependencies involving ranges.  Given a reference to a range, we find
        ///     all non-range references that intersect it.  We then create dependencies (or links) between those references and
        ///     all depedendents of the target range.  If we didn't do this, then when walking a dependency chain, we'd have
        ///     to find all intersecting references for each reference along the chain.
        /// </remarks>
        private void ProcessRangeLinks(IntersectingDependantProcessor processor)
        {
            IList linkers = FindReferences(new RangeLinkPredicate());

            foreach (Reference linker in linkers)
            {
                IList intersecting = FindReferences(new LinkerIntersectPredicate(linker));
                ProcessLinkerIntersects(linker, intersecting, processor);
            }
        }

        private void ProcessLinkerIntersects(Reference linker, IList intersecting,
            IntersectingDependantProcessor processor)
        {
            IList linkerDependents = new ArrayList();
            _dependents.GetDirectDependents(linker, linkerDependents);

            foreach (Reference intersect in intersecting)
            {
                ProcessLinkerDependents(intersect, linkerDependents, processor);
            }
        }

        private void ProcessLinkerDependents(Reference intersect, IList linkerDependents,
            IntersectingDependantProcessor processor)
        {
            foreach (Reference linkerDependant in linkerDependents)
            {
                processor(intersect, linkerDependant);
            }
        }

        private void DoAddIntersectingDependency(Reference tail, Reference dependant)
        {
            _dependents.AddDependency(tail, dependant);
            _precedents.AddDependency(dependant, tail);
        }

        private void DoRemoveIntersectingDependency(Reference tail, Reference dependant)
        {
            _dependents.RemoveDependency(tail, dependant);
            _precedents.RemoveDependency(dependant, tail);
        }

        private IList FindReferences(DependencyReferencePredicateBase predicate)
        {
            predicate.SetOwner(this);
            return _owner.ReferencePool.FindReferences(predicate);
        }

        private void RemoveDependency(Reference tail, Reference head, DependencyMap map, DependencyMap inverseMap)
        {
            map.RemoveDependency(tail, head);
            inverseMap.RemoveDependency(head, tail);
        }

        internal void Clear()
        {
            _dependents.Clear();
            _precedents.Clear();
        }

        public IList GetSources(IList refs)
        {
            IList sources = refs.OfType<Reference>().Where(IsSource).ToList();
            return sources;
        }

        public Reference[] GetReferenceCalculationList(Reference root)
        {
            IList roots = FindReferences(new IntersectingPredicate(root));
            return GetCalculationList(roots);
        }

        public Reference[] GetAllCalculationList()
        {
            IList roots = FindReferences(new SourcePredicate());
            return GetCalculationList(roots);
        }

        /// <summary>
        ///     Gets a sorted list of all references that depend on a given set of references
        /// </summary>
        public Reference[] GetCalculationList(IList roots)
        {
            // We need to get rid of all circular references from the roots list
            RemoveCircularReferences(roots);
            // Add any volatile functions to the list
            AddVolatileReference(roots);
            // Create temporary maps for calculating the list
            var tempDependents = new DependencyMap();
            var tempPrecedents = new DependencyMap();

            // Clone the portion of our dependency map that contains the roots
            _dependents.CloneSourceDependents(roots, tempDependents, tempPrecedents);

            // We don't care about non-sources
            RemoveNonSourcesFromRoots(roots, tempPrecedents);

            // Sort the dependencies
            IList targets = TopologicalSort(tempDependents, tempPrecedents, roots);
            // Sources can't be recalculated since they have no formula so get rid of them
            RemoveSources(roots, targets);

            var arr = new Reference[targets.Count];
            targets.CopyTo(arr, 0);
            return arr;
        }

        private void RemoveCircularReferences(IList roots)
        {
            var arr = new Reference[roots.Count];
            roots.CopyTo(arr, 0);

            foreach (Reference @ref in arr)
            {
                if (IsCircularReference(@ref))
                {
                    roots.Remove(@ref);
                }
            }
        }

        private void RemoveNonSourcesFromRoots(IList roots, DependencyMap precedents)
        {
            var arr = new Reference[roots.Count];
            roots.CopyTo(arr, 0);

            for (int i = 0; i <= arr.Length - 1; i++)
            {
                Reference root = arr[i];
                if (precedents.ContainsTail(root))
                {
                    roots.Remove(root);
                }
            }
        }

        private void RemoveSources(IList sources, IList targets)
        {
            foreach (Reference source in sources)
            {
                targets.Remove(source);
            }
        }

        /// <summary>
        ///     Add any volatile functions to the roots of a calculation list
        /// </summary>
        private void AddVolatileReference(IList roots)
        {
            var @volatile = new VolatileFunctionReference();
            // Are there any formulas that have volatile functions?
            @volatile = _owner.ReferencePool.GetPooledReference(@volatile) as VolatileFunctionReference;
            if (@volatile != null)
            {
                roots.Add(@volatile);
            }
        }

        /// <summary>
        ///     Perform the topological sort algorithm on our graph
        /// </summary>
        private IList TopologicalSort(DependencyMap dependents, DependencyMap precedents, IList sources)
        {
            var q = new Queue(sources);
            IList output = new ArrayList();
            IList directDependents = new ArrayList();

            while (q.Count > 0)
            {
                var n = (Reference)q.Dequeue();
                output.Add(n);

                directDependents.Clear();
                dependents.GetDirectDependents(n, directDependents);

                foreach (Reference m in directDependents)
                {
                    dependents.RemoveDependency(n, m);
                    precedents.RemoveDependency(m, n);
                    if (precedents.ContainsTail(m) == false)
                    {
                        q.Enqueue(m);
                    }
                }
            }

            // We assume no circular references
            return output;
        }

        /// <summary>
        ///     Determines if a reference is a source.  A source is a reference that only has dependents and no precedents
        /// </summary>
        private bool IsSource(Reference @ref)
        {
            return _precedents.ContainsTail(@ref) == false;
        }

        public int GetDirectDependentsCount(Reference @ref)
        {
            return GetDirectDependentsInternal(@ref, _dependents);
        }

        public int GetDirectPrecedentsCount(Reference @ref)
        {
            return GetDirectDependentsInternal(@ref, _precedents);
        }

        private int GetDirectDependentsInternal(Reference @ref, DependencyMap map)
        {
            IList dependents = new ArrayList();
            map.GetDirectDependents(@ref, dependents);
            return dependents.Count;
        }

        public void SetSuspendRangeLinks(bool suspend)
        {
            _suspendRangeLinks = suspend;
        }

        /// <summary>
        ///     A data structure to store a reference and a list of its dependents
        /// </summary>
        [Serializable]
        private class DependencyMap
        {
            private readonly IDictionary _map;

            public DependencyMap()
            {
                _map = new Hashtable();
            }

            public int DependencyCount { get { return _map.Values.Cast<IList>().Sum(l => l.Count); } }

            public void AddDependency(Reference tail, Reference head)
            {
                IList dependents = GetDependentsList(tail);
                Debug.Assert(dependents.Contains(head) == false, "head already in list");
                dependents.Add(head);
            }

            public void RemoveDependency(Reference tail, Reference head)
            {
                var list = (IList)_map[tail];
                Debug.Assert(list.Contains(head), "reference not in list");
                list.Remove(head);

                if (list.Count == 0)
                {
                    _map.Remove(tail);
                }
            }

            private IList GetDependentsList(Reference tail)
            {
                var list = (IList)_map[tail];

                if (list == null)
                {
                    list = new ArrayList();
                    _map.Add(tail, list);
                }

                return list;
            }

            public bool ContainsTail(Reference tail)
            {
                return _map.Contains(tail);
            }

            public void Clear()
            {
                _map.Clear();
            }

            public bool IsCircularReference(Reference tail)
            {
                return IsCircularReferenceInternal(tail, tail);
            }

            private bool IsCircularReferenceInternal(Reference target, object currentReference)
            {
                var directDependents = (IList)_map[currentReference];

                if (directDependents == null)
                {
                    return false;
                }

                return directDependents.Cast<Reference>()
                    .Any(dependant => target.IsReferenceEqualForCircularReference(dependant) ||
                    IsCircularReferenceInternal(target, dependant));
            }

            public void GetDirectDependents(Reference tail, IList dest)
            {
                var list = (IList)_map[tail];
                if (list != null)
                {
                    foreach (object o in list)
                    {
                        dest.Add(o);
                    }
                }
            }

            /// <summary>
            ///     Copy a portion of our dependencies into another DependencyMap
            /// </summary>
            public void CloneSourceDependents(IList sources, DependencyMap dependents, DependencyMap precedents)
            {
                IDictionary seenNodes = new Hashtable();
                foreach (Reference source in sources)
                {
                    CloneDependentsInternal(source, dependents, precedents, seenNodes);
                }
            }

            private void CloneDependentsInternal(Reference tail, DependencyMap dependents, DependencyMap precedents,
                IDictionary seenNodes)
            {
                // Have we seen this reference already?
                if (seenNodes.Contains(tail))
                {
                    // Yes so just return
                    return;
                }
                // Mark it as seen
                seenNodes.Add(tail, null);

                var list = (IList)_map[tail];

                if (list == null)
                {
                    // No dependents so just return
                    return;
                }

                foreach (Reference dependant in list)
                {
                    dependents.AddDependency(tail, dependant);
                    precedents.AddDependency(dependant, tail);
                    CloneDependentsInternal(dependant, dependents, precedents, seenNodes);
                }
            }

            public override string ToString()
            {
                var lines = new string[_map.Count];
                int index = 0;

                foreach (DictionaryEntry de in _map)
                {
                    var key = (Reference)de.Key;
                    var list = (IList)de.Value;
                    lines[index] = String.Concat(key, " -> ", Reference.References2String(list));
                    index += 1;
                }

                return String.Join(Environment.NewLine, lines);
            }
        }

        private abstract class DependencyReferencePredicateBase : ReferencePredicateBase
        {
            private DependencyManager _owner;

            protected DependencyManager Owner { get { return _owner; } }

            public void SetOwner(DependencyManager owner)
            {
                _owner = owner;
            }
        }

        private delegate void IntersectingDependantProcessor(Reference tail, Reference dependant);

        private class IntersectingPredicate : DependencyReferencePredicateBase
        {
            private readonly Reference _target;

            public IntersectingPredicate(Reference target)
            {
                _target = target;
            }

            public override bool IsMatch(Reference @ref)
            {
                return _target.Intersects(@ref);
            }
        }

        private class LinkerIntersectPredicate : DependencyReferencePredicateBase
        {
            private readonly Reference _target;

            public LinkerIntersectPredicate(Reference target)
            {
                _target = target;
            }

            public override bool IsMatch(Reference @ref)
            {
                return @ref.CanRangeLink == false && _target.Intersects(@ref) && Owner.IsSource(@ref) == false;
            }
        }

        private class RangeLinkPredicate : DependencyReferencePredicateBase
        {
            public override bool IsMatch(Reference @ref)
            {
                return @ref.CanRangeLink;
            }
        }

        private class SourcePredicate : DependencyReferencePredicateBase
        {
            public override bool IsMatch(Reference @ref)
            {
                return Owner.IsSource(@ref);
            }
        }
    }
}
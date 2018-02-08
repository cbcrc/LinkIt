using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Core
{
    /// <summary>
    /// Responsible for gathering the lookup ids of a loading level.
    /// </summary>
    internal class LookupContext
    {
        private readonly Dictionary<Type, HashSet<object>> _lookupIdsByReferenceType = new Dictionary<Type, HashSet<object>>();

        public IReadOnlyDictionary<Type, IReadOnlyList<object>> LookupIds
            => _lookupIdsByReferenceType.ToDictionary(p => p.Key, p => (IReadOnlyList<object>) p.Value.ToList());

        public void AddLookupId<TReference>(object lookupId)
        {
            if (lookupId == null) return;

            AddLookupIds(typeof(TReference), new[] { lookupId });
        }

        public void AddLookupIds<TReference, TId>(IEnumerable<TId> lookupIds)
        {
            if (lookupIds == null)
            {
                throw new ArgumentNullException(nameof(lookupIds));
            }

            AddLookupIds(typeof(TReference), lookupIds);
        }

        private void AddLookupIds<TId>(Type referenceType, IEnumerable<TId> lookupIds)
        {
            var nonNullIds = lookupIds.Where(id => id != null).ToList();
            if (!nonNullIds.Any())
            {
                return;
            }

            var currentLookupIds = GetOrAddLookupIdsFor(referenceType);
            foreach (var id in nonNullIds)
            {
                currentLookupIds.Add(id);
            }
        }

        private HashSet<object> GetOrAddLookupIdsFor(Type referenceType)
        {
            if (_lookupIdsByReferenceType.ContainsKey(referenceType))
            {
                return _lookupIdsByReferenceType[referenceType];
            }

            var emptySet = new HashSet<object>();
            _lookupIdsByReferenceType.Add(referenceType, emptySet);
            return emptySet;
        }
    }
}

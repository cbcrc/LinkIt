#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    /// <summary>
    /// In addition to the responsiblies of ILookupIdContext,
    /// responsible for gathering the lookup ids of a loading level.
    /// </summary>I
    internal class LookupIdContext : ILookupIdContext
    {
        private readonly Dictionary<Type, IEnumerable> _lookupIdsByReferenceType = new Dictionary<Type, IEnumerable>();

        public IReadOnlyList<Type> GetReferenceTypes()
        {
            return _lookupIdsByReferenceType
                .Keys
                .ToList();
        }

        public IReadOnlyList<TId> GetReferenceIds<TReference, TId>()
        {
            return GetReferenceIds<TId>(typeof(TReference));
        }

        public IReadOnlyList<TId> GetReferenceIds<TId>(Type referenceType)
        {
            return (IReadOnlyList<TId>) GetLookupIds<TId>(referenceType)?.ToList() ?? new TId[0];
        }

        public IDictionary<Type, IEnumerable> GetReferenceIds()
        {
            return new Dictionary<Type, IEnumerable>(_lookupIdsByReferenceType);
        }

        private HashSet<TId> GetLookupIds<TId>(Type referenceType)
        {
            return _lookupIdsByReferenceType.ContainsKey(referenceType)
                ? (HashSet<TId>) _lookupIdsByReferenceType[referenceType]
                : null;
        }

        public void AddSingle<TReference, TId>(TId lookupId)
        {
            AddSingle(typeof(TReference), lookupId);
        }

        private void AddSingle<TId>(Type referenceType, TId lookupId)
        {
            if (lookupId == null) return;

            var currentLookupIds = GetOrAddLookupIdsFor<TId>(referenceType);
            currentLookupIds.Add(lookupId);
        }

        private HashSet<TId> GetOrAddLookupIdsFor<TId>(Type referenceType)
        {
            var lookupIds = GetLookupIds<TId>(referenceType);
            if (lookupIds != null)
            {
                return lookupIds;
            }

            var set = new HashSet<TId>();
            _lookupIdsByReferenceType.Add(referenceType, set);
            return set;
        }

        public void AddMulti<TReference, TId>(IEnumerable<TId> lookupIds)
        {
            AddMulti(typeof(TReference), lookupIds);
        }

        private void AddMulti<TId>(Type referenceType, IEnumerable<TId> lookupIds)
        {
            if (lookupIds == null)
            {
                throw new ArgumentNullException(nameof(lookupIds));
            }

            var nonNullIds = lookupIds.Where(id => id != null).ToList();
            if (!nonNullIds.Any())
            {
                return;
            }

            var currentLookupIds = GetOrAddLookupIdsFor<TId>(referenceType);
            foreach (var id in nonNullIds)
            {
                currentLookupIds.Add(id);
            }
        }
    }
}
#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
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
        private readonly Dictionary<Type, object> _lookupIdsByReferenceType = new Dictionary<Type, object>();

        public IReadOnlyList<Type> GetReferenceTypes()
        {
            return _lookupIdsByReferenceType
                .Keys
                .ToList();
        }

        public IReadOnlyList<TId> GetReferenceIds<TReference, TId>()
        {
            var tReference = typeof(TReference);
            if (!_lookupIdsByReferenceType.ContainsKey(tReference))
                throw new InvalidOperationException(
                    $"There are no reference ids for this the type {tReference.Name}."
                );

            var casted = (List<TId>) _lookupIdsByReferenceType[tReference];

            return casted
                .Distinct()
                .ToList();
        }

        public void AddSingle<TReference, TId>(TId lookupId)
        {
            if (lookupId == null) return;

            var tReference = typeof(TReference);
            if (!_lookupIdsByReferenceType.ContainsKey(tReference)) _lookupIdsByReferenceType.Add(tReference, new List<TId>());

            var currentLookupIds = (List<TId>) _lookupIdsByReferenceType[tReference];
            currentLookupIds.Add(lookupId);
        }

        public void AddMulti<TReference, TId>(IEnumerable<TId> lookupIds)
        {
            if (lookupIds == null) throw new ArgumentNullException(nameof(lookupIds));

            foreach (var lookupId in lookupIds) AddSingle<TReference, TId>(lookupId);
        }
    }
}
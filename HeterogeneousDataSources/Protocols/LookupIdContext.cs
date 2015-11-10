using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public class LookupIdContext
    {
        private readonly Dictionary<Type, object> _lookupIdsByReferenceType = new Dictionary<Type, object>();

        public void AddSingle<TReference, TId>(TId lookupId) {
            if (lookupId == null) { return; }

            var tReference = typeof(TReference);
            if (!_lookupIdsByReferenceType.ContainsKey(tReference)) {
                _lookupIdsByReferenceType.Add(tReference, new List<TId>());
            }

            var currentLookupIds = (List<TId>)_lookupIdsByReferenceType[tReference];
            currentLookupIds.Add(lookupId);
        }

        public void AddMulti<TReference, TId>(List<TId> lookupIds) {
            foreach (var lookupId in lookupIds) {
                AddSingle<TReference, TId>(lookupId);
            }
        }

        public List<Type> GetReferenceTypes() {
            return _lookupIdsByReferenceType
                .Keys
                .ToList();
        }

        public List<TId> GetReferenceIds<TReference, TId>()
        {
            var tReference = typeof (TReference);
            if (!_lookupIdsByReferenceType.ContainsKey(tReference)) {
                throw new InvalidOperationException(
                    string.Format(
                        "There are no reference ids for this the type {0}.",
                        tReference.Name)
                );
            }

            var casted = (List<TId>) _lookupIdsByReferenceType[tReference];

            return casted
                .Distinct()
                .ToList();
        }
    }
}
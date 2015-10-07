using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    //stle: this may become a builder
    public class LookupIdContext
    {
        private readonly Dictionary<Type, List<object>> _lookupIdsByReferenceType = new Dictionary<Type, List<object>>();

        public void AddSingle<TReference>(object lookupId) {
            var tReference = typeof(TReference);
            if (!_lookupIdsByReferenceType.ContainsKey(tReference)) {
                _lookupIdsByReferenceType.Add(tReference, new List<object>());
            }

            //stle: think of how we can
            //  avoid depending on reference loader to optimize for empty ids
            //  and
            //  to have a simple list initilization mechanism in linking (especially in poly)
            if (lookupId == null) { return; }

            var currentLookupIds = _lookupIdsByReferenceType[tReference];
            currentLookupIds.Add(lookupId);
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

            return _lookupIdsByReferenceType[tReference]
                .Cast<TId>()
                .Distinct()
                .ToList();
        }
    }
}
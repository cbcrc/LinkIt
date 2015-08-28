using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    //stle: this may become a builder
    public class LookupIdContext
    {
        private readonly Dictionary<Type, object> _lookupIdsByReferenceType = new Dictionary<Type, object>();

        public void Add<TReference, TId>(List<TId> lookupIds)
        {
            //Assume that that lookupIds cannot contains null

            var tReference = typeof(TReference);

            if (!_lookupIdsByReferenceType.ContainsKey(tReference)) {
                _lookupIdsByReferenceType.Add(tReference, new List<TId>());
            }

            var currentLookupIds = (List<TId>)_lookupIdsByReferenceType[tReference];
            currentLookupIds.AddRange(lookupIds);
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
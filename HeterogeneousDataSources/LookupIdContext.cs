using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public class LookupIdContext
    {
        private readonly Dictionary<Type, object> _lookupIdsByReferenceType = new Dictionary<Type, object>();

        public void Add<TReference, TId>(List<TId> lookupIds)
        {
            var tReference = typeof(TReference);

            if (_lookupIdsByReferenceType.ContainsKey(tReference)) {
                throw new InvalidOperationException(
                    string.Format(
                        "All lookup ids for the reference type ({0}) must be added at the same time.",
                        tReference.Name)
                );
            }

            _lookupIdsByReferenceType.Add(tReference, GetCleanedIds(lookupIds));
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
            return casted.ToList();
        }

        private static List<TId> GetCleanedIds<TId>(List<TId> lookupIds) {
            return lookupIds
                .Where(id => id != null)
                .Distinct()
                .ToList();
        }
    }
}
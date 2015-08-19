using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public class ReferenceTypeConfig<TReference,TId>
    {
        private readonly Func<List<TId>, List<TReference>> _loadReferencesFunc;

        public ReferenceTypeConfig(
            Func<TReference, TId> getReferenceIdFunc, 
            Func<List<TId>, List<TReference>> loadReferencesFunc)
        {
            GetReferenceIdFunc = getReferenceIdFunc;
            _loadReferencesFunc = loadReferencesFunc;
        }

        public Func<TReference, TId> GetReferenceIdFunc { get; private set; }

        public Func<List<TId>, List<TReference>> LoadReferencesFunc
        {
            get { return LoadReferencesWithCleanedIds; } 
        }

        private List<TReference> LoadReferencesWithCleanedIds(List<TId> ids)
        {
            var cleanedIds = ids
                .Where(id => id != null)
                .Distinct()
                .ToList();

            return _loadReferencesFunc(cleanedIds);
        }
    }
}
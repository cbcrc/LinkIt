using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public class ReferenceTypeConfig<TReference,TId>
    {
        public ReferenceTypeConfig(
            Func<TReference, TId> getReferenceIdFunc, 
            Func<List<TId>, List<TReference>> loadReferencesFunc)
        {
            LoadReferencesFunc = loadReferencesFunc;
            GetReferenceIdFunc = getReferenceIdFunc;
        }

        public Func<TReference, TId> GetReferenceIdFunc { get; private set; }

        public Func<List<TId>, List<TReference>> LoadReferencesFunc { get; private set; }
    }
}
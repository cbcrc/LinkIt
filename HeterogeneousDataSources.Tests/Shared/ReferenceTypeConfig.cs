using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.Tests.Shared
{
    public class ReferenceTypeConfig<TReference, TId> : IReferenceTypeConfig {
        private readonly Func<List<TId>, List<TReference>> _loadReferencesFunc;
        //the necessity of this function could be generalized
        private readonly Func<TReference, object> _getReferenceIdFunc;

        public ReferenceTypeConfig(Func<List<TId>, List<TReference>> loadReferencesFunc, Func<TReference, object> getReferenceIdFunc, string requiredConnection = null)
        {
            _loadReferencesFunc = loadReferencesFunc;
            _getReferenceIdFunc = getReferenceIdFunc;
            RequiredConnection = requiredConnection;
        }

        public string RequiredConnection { get; private set; }

        public Type ReferenceType
        {
            get { return typeof (TReference); }
        }

        public void Load(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext) {
            var lookupIds = lookupIdContext.GetReferenceIds<TReference, TId>();
            var references = _loadReferencesFunc(lookupIds);
            loadedReferenceContext.AddReferences(references, _getReferenceIdFunc);
        }
    }
}
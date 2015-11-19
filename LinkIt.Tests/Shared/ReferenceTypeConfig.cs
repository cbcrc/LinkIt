using System;
using System.Collections.Generic;
using LinkIt.PublicApi;

namespace LinkIt.Tests.Shared
{
    public class ReferenceTypeConfig<TReference, TId> : IReferenceTypeConfig {
        private readonly Func<List<TId>, List<TReference>> _loadReferences;
        //the necessity of this function could be generalized
        private readonly Func<TReference, TId> _getReferenceId;

        public ReferenceTypeConfig(Func<List<TId>, List<TReference>> loadReferences, Func<TReference, TId> getReferenceId, string requiredConnection = null)
        {
            _loadReferences = loadReferences;
            _getReferenceId = getReferenceId;
            RequiredConnection = requiredConnection;
        }

        public string RequiredConnection { get; private set; }

        public Type ReferenceType
        {
            get { return typeof (TReference); }
        }

        public void Load(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext) {
            var lookupIds = lookupIdContext.GetReferenceIds<TReference, TId>();
            var references = _loadReferences(lookupIds);
            loadedReferenceContext.AddReferences(references, reference => _getReferenceId(reference));
        }
    }
}
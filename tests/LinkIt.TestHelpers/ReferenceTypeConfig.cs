// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using LinkIt.PublicApi;

namespace LinkIt.TestHelpers
{
    public class ReferenceTypeConfig<TReference, TId> : IReferenceTypeConfig
    {
        private readonly Func<IEnumerable<TId>, IEnumerable<TReference>> _loadReferences;
        //the necessity of this function could be generalized
        private readonly Func<TReference, TId> _getReferenceId;

        public ReferenceTypeConfig(Func<IEnumerable<TId>, IEnumerable<TReference>> loadReferences, Func<TReference, TId> getReferenceId)
        {
            _loadReferences = loadReferences;
            _getReferenceId = getReferenceId;
        }

        public Type ReferenceType => typeof (TReference);

        public void Load(ILoadingContext loadingContext)
        {
            var lookupIds = loadingContext.GetReferenceIds<TReference, TId>();
            var references = _loadReferences(lookupIds);
            loadingContext.AddReferences(references, reference => _getReferenceId(reference));
        }
    }
}
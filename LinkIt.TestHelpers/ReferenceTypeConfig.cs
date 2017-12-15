#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using LinkIt.PublicApi;

namespace LinkIt.TestHelpers
{
    public class ReferenceTypeConfig<TReference, TId> : IReferenceTypeConfig {
        private readonly Func<List<TId>, List<TReference>> _loadReferences;
        //the necessity of this function could be generalized
        private readonly Func<TReference, TId> _getReferenceId;

        public ReferenceTypeConfig(Func<List<TId>, List<TReference>> loadReferences, Func<TReference, TId> getReferenceId)
        {
            _loadReferences = loadReferences;
            _getReferenceId = getReferenceId;
        }

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
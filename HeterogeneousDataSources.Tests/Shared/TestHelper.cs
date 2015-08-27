using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources.Tests.Shared {
    public static class TestHelper {
        public static LoadLinkProtocol CreateLoadLinkProtocol<TReference, TId>(List<ILoadLinkExpression> loadLinkExpressions, TReference fixedValue, Func<TReference, TId> getReferenceIdFunc, List<Type>[] fakeReferenceTypeForLoadingLevel) 
        {
            var customConfig = CreateCustomReferenceTypeConfig(
                fixedValue,
                getReferenceIdFunc
            );
            var referenceLoader = new FakeReferenceLoader(customConfig);

            return new LoadLinkProtocol(
                referenceLoader,
                new LoadLinkConfig(
                    loadLinkExpressions,
                    fakeReferenceTypeForLoadingLevel 
                )
            );
        }

        public static IReferenceTypeConfig CreateCustomReferenceTypeConfig<TReference, TId>(TReference fixedValue, Func<TReference, TId> getReferenceIdFunc) {
            return new ReferenceTypeConfig<TReference, TId>(
                ids => ids.Select(id => fixedValue).ToList(),
                getReferenceIdFunc
            );
        }
    }
}

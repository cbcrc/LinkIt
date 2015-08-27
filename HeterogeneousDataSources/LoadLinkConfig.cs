using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions, List<Type>[] fakeReferenceTypeForLoadingLevel)
        {
            FakeReferenceTypeForLoadingLevel = fakeReferenceTypeForLoadingLevel;

            AllLoadLinkExpressions = loadLinkExpressions;
            ReferenceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => !loadLinkExpression.IsNestedLinkedSourceLoadLinkExpression)
                .ToList();

            NestedLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.IsNestedLinkedSourceLoadLinkExpression)
                .ToList();
        }

        public List<Type>[] FakeReferenceTypeForLoadingLevel { get; private set; }

        public int GetNumberOfLoadingLevel<TLinkedSource>()
        {
            return FakeReferenceTypeForLoadingLevel.Length;
        }

        public List<Type> GetReferenceTypeForLoadingLevel<TLinkedSource>(int loadingLevel)
        {
            return FakeReferenceTypeForLoadingLevel[loadingLevel];
        }


        public List<ILoadLinkExpression> AllLoadLinkExpressions { get; private set; }
        public List<ILoadLinkExpression> ReferenceLoadLinkExpressions { get; private set; }
        public List<ILoadLinkExpression> NestedLinkedSourceLoadLinkExpressions { get; private set; }
    }
}

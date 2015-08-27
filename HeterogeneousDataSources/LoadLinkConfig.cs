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

        public int GetNumberOfLoadingLevel<TRootLinkedSource>()
        {
            return FakeReferenceTypeForLoadingLevel.Length;
        }

        public List<ILoadLinkExpression> GetLoadExpressions<TRootLinkedSource>(object linkedSource, int loadingLevel)
        {
            return GetLoadLinkExpressions<TRootLinkedSource>(linkedSource, loadingLevel, AllLoadLinkExpressions);
        }

        public List<ILoadLinkExpression> GetLinkNestedLinkedSourceExpressions<TRootLinkedSource>(object linkedSource, int loadingLevel) {
            return GetLoadLinkExpressions<TRootLinkedSource>(linkedSource, loadingLevel, NestedLinkedSourceLoadLinkExpressions);
        }

        public List<ILoadLinkExpression> GetLinkReferenceExpressions(object linkedSource) {
            var linkedSourceType = linkedSource.GetType();
            return ReferenceLoadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType.Equals(linkedSourceType))
                .ToList();
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions<TRootLinkedSource>(object linkedSource, int loadingLevel, List<ILoadLinkExpression> loadLinkExpressions) {
            var referenceTypesToBeLoaded = GetReferenceTypeForLoadingLevel<TRootLinkedSource>(loadingLevel);

            var linkedSourceType = linkedSource.GetType();
            return loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType.Equals(linkedSourceType))
                .Where(loadLinkExpression => referenceTypesToBeLoaded.Contains(loadLinkExpression.ReferenceType))
                .ToList();
        }



        private List<Type> GetReferenceTypeForLoadingLevel<TRootLinkedSource>(int loadingLevel)
        {
            return FakeReferenceTypeForLoadingLevel[loadingLevel];
        }


        private List<ILoadLinkExpression> AllLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> ReferenceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> NestedLinkedSourceLoadLinkExpressions { get; set; }
    }
}

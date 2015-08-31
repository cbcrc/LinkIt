using System;
using System.Collections;
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
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Reference)
                .ToList();

            NestedLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.NestedLinkedSource)
                .ToList();

            SubLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.SubLinkedSource)
                .ToList();
        }

        public List<Type>[] FakeReferenceTypeForLoadingLevel { get; private set; }

        public int GetNumberOfLoadingLevel<TRootLinkedSource>()
        {
            return FakeReferenceTypeForLoadingLevel.Length;
        }

        public List<ILoadLinkExpression> GetLoadExpressions<TRootLinkedSource>(object linkedSource, int loadingLevel)
        {
            return GetLoadLinkExpressions<TRootLinkedSource>(linkedSource, AllLoadLinkExpressions, loadingLevel);
        }

        public List<ILoadLinkExpression> GetLinkNestedLinkedSourceExpressions<TRootLinkedSource>(object linkedSource, int loadingLevel) {
            return GetLoadLinkExpressions<TRootLinkedSource>(linkedSource, NestedLinkedSourceLoadLinkExpressions, loadingLevel);
        }

        public List<ILoadLinkExpression> GetLinkReferenceExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, ReferenceLoadLinkExpressions)
                .ToList();
        }

        public List<ILoadLinkExpression> GetSubLinkedSourceExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, SubLinkedSourceLoadLinkExpressions)
                .ToList();
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions<TRootLinkedSource>(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions, int loadingLevel) {
            var referenceTypesToBeLoaded = GetReferenceTypeForLoadingLevel<TRootLinkedSource>(loadingLevel);

            return GetLoadLinkExpressions(linkedSource, loadLinkExpressions)
                .Where(loadLinkExpression => referenceTypesToBeLoaded.Contains(loadLinkExpression.ReferenceType))
                .ToList();
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions) {
            var linkedSourceType = linkedSource.GetType();
            return loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType.Equals(linkedSourceType))
                .ToList();
        }


        private List<Type> GetReferenceTypeForLoadingLevel<TRootLinkedSource>(int loadingLevel)
        {
            return FakeReferenceTypeForLoadingLevel[loadingLevel];
        }

        private List<ILoadLinkExpression> AllLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> ReferenceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> NestedLinkedSourceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> SubLinkedSourceLoadLinkExpressions { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        //stle: remove fakeReferenceType
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions, List<Type>[] fakeReferenceTypeForLoadingLevel)
        {
            AllLoadLinkExpressions = loadLinkExpressions;
            ReferenceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Reference)
                .ToList();

            NestedLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => 
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.NestedLinkedSource||
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Root)
                .ToList();

            SubLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.SubLinkedSource)
                .ToList();
        }

        //stle: should go in protocol
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

        public int GetNumberOfLoadingLevel<TRootLinkedSource>()
        {
            return InferReferenceTypeForEachLoadingLevel<TRootLinkedSource>().Count;
        }

        public List<Type> GetReferenceTypeForLoadingLevel<TRootLinkedSource>(int loadingLevel)
        {
            return InferReferenceTypeForEachLoadingLevel<TRootLinkedSource>()[0];
        }

        //stle: init for performance
        public List<List<Type>> InferReferenceTypeForEachLoadingLevel<TRootLinkedSource>()
        {
            var firstLevel = AllLoadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression is IRootLoadLinkExpression)
                .Cast<IRootLoadLinkExpression>()
                .Where(rootLoadLinkExpression => 
                    rootLoadLinkExpression.RootLinkedSourceType == typeof (TRootLinkedSource)
                )
                .Select(loadLinkExpression => loadLinkExpression.ReferenceType)
                .ToList();

            //var nextLevel = AllLoadLinkExpressions
            //    .Where(loadLinkExpression =>
            //        loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.NestedLinkedSource ||
            //        loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Reference)
            //    .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType == typeof(TRootLinkedSource))
            //    .Select(loadLinkExpression => loadLinkExpression.ReferenceType)
            //.ToList();

            return new List<List<Type>>{firstLevel};
        }

        private List<ILoadLinkExpression> AllLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> ReferenceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> NestedLinkedSourceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> SubLinkedSourceLoadLinkExpressions { get; set; }
    }
}

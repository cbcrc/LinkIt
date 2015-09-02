using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        private LoadLinkExpressionTreeFactory _linkExpressionTreeFactory;
        private readonly Dictionary<Type, Dictionary<int, List<Type>>> _referenceTypeByLoadingLevelByRootLinkedSourceType;

        //stle: remove fakeReferenceType
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions, List<Type>[] fakeReferenceTypeForLoadingLevel = null)
        {
            EnsureChildLinkedSourceTypeIsUniqueInRootExpression(loadLinkExpressions);

            _linkExpressionTreeFactory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);

            _referenceTypeByLoadingLevelByRootLinkedSourceType = 
                GetReferenceTypeByLoadingLevelByRootLinkedSourceType(
                    loadLinkExpressions, 
                    _linkExpressionTreeFactory
                );

            AllLoadLinkExpressions = loadLinkExpressions;
            ReferenceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Reference)
                .ToList();

            NestedLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => 
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.NestedLinkedSource ||
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Root
                )
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

        //stle: is this internal?
        public int GetNumberOfLoadingLevel<TRootLinkedSource>(){
            var rootLinkedSourceType = typeof (TRootLinkedSource);

            return _referenceTypeByLoadingLevelByRootLinkedSourceType[rootLinkedSourceType].Count;
        }

        //stle: is this internal?
        public List<Type> GetReferenceTypeForLoadingLevel<TRootLinkedSource>(int loadingLevel){
            var rootLinkedSourceType = typeof(TRootLinkedSource);

            return _referenceTypeByLoadingLevelByRootLinkedSourceType[rootLinkedSourceType][loadingLevel];
        }

        private Dictionary<Type, Dictionary<int, List<Type>>> GetReferenceTypeByLoadingLevelByRootLinkedSourceType(List<ILoadLinkExpression> loadLinkExpressions, LoadLinkExpressionTreeFactory linkExpressionTreeFactory)
        {
            var parser = new ReferenceTypeByLoadingLevelParser(linkExpressionTreeFactory);

            return GetRootLoadLinkExpressions(loadLinkExpressions)
                .Select(rootExpression =>
                    new {
                        RootLinkedSourceType = rootExpression.ChildLinkedSourceType,
                        ReferenceTypeByLoadingLevel = parser.ParseReferenceTypeByLoadingLevel(rootExpression)
                    }
                )
                .ToDictionary(
                    p => p.RootLinkedSourceType,
                    p => p.ReferenceTypeByLoadingLevel
                );
        }

        private void EnsureChildLinkedSourceTypeIsUniqueInRootExpression(List<ILoadLinkExpression> loadLinkExpressions) {
            var childLinkedSourceTypeWithDuplicates = GetRootLoadLinkExpressions(loadLinkExpressions)
                .GroupBy(rootExpression => rootExpression.ChildLinkedSourceType)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (childLinkedSourceTypeWithDuplicates.Any()) {
                throw new ArgumentException(
                    string.Format(
                        "Can only have one root expression per child linked source, but there are many for : {0}.",
                        String.Join(",", childLinkedSourceTypeWithDuplicates)
                    )
                );
            }
        }

        private List<INestedLoadLinkExpression> GetRootLoadLinkExpressions(List<ILoadLinkExpression> loadLinkExpressions)
        {
            return loadLinkExpressions
                .Where(loadLinkExpression =>
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Root)
                .Cast<INestedLoadLinkExpression>()
                .ToList();
        }

        private List<ILoadLinkExpression> AllLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> ReferenceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> NestedLinkedSourceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> SubLinkedSourceLoadLinkExpressions { get; set; }
    }
}

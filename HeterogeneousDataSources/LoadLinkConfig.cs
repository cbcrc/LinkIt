using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        private readonly List<ILoadLinkExpression> _allLoadLinkExpressions;
        private readonly ReferenceTypeByLoadingLevelParser _referenceTypeByLoadingLevelParser;

        #region Initialization
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions)
        {
            EnsureLoadLinkExpressionLinkTargetIdsAreUnique(loadLinkExpressions);

            var linkExpressionTreeFactory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);
            _referenceTypeByLoadingLevelParser = new ReferenceTypeByLoadingLevelParser(linkExpressionTreeFactory);
            
            //EnsureNoCyclesInRootLoadLinkExpressions(loadLinkExpressions, linkExpressionTreeFactory);

            _allLoadLinkExpressions = loadLinkExpressions;
        }

        private void EnsureLoadLinkExpressionLinkTargetIdsAreUnique(List<ILoadLinkExpression> loadLinkExpressions) {
            var linkTargetIdsWithDuplicates = loadLinkExpressions.GetNotUniqueKey(loadLinkExpression => loadLinkExpression.LinkTargetId);

            if (linkTargetIdsWithDuplicates.Any()) {
                throw new ArgumentException(
                    string.Format(
                        "Can only have one load link expression per link target id, but there are many for : {0}.",
                        String.Join(",", linkTargetIdsWithDuplicates)
                    )
                );
            }
        }

        private void EnsureNoCyclesInRootLoadLinkExpressions(List<ILoadLinkExpression> loadLinkExpressions, LoadLinkExpressionTreeFactory loadLinkExpressionTreeFactory) {
            //var cycles = GetRootLoadLinkExpressions(loadLinkExpressions)
            //    .Select(
            //        rootLoadLinkExpression =>
            //            new {
            //                RootLoadLinkExpression = rootLoadLinkExpression,
            //                ReferenceTypeThatCreatesACycle = loadLinkExpressionTreeFactory.GetReferenceTypeThatCreatesACycleFromTree(rootLoadLinkExpression)
            //            })
            //    .Where(potentialCycle => potentialCycle.ReferenceTypeThatCreatesACycle != null)
            //    .ToList();

            //if (cycles.Any()) {
            //    var cycleAsString = cycles
            //        .Select(cycle => string.Format("{0} creates a cycle in {1}", cycle.ReferenceTypeThatCreatesACycle, cycle.RootLoadLinkExpression.ChildLinkedSourceTypes))
            //        .ToList();

            //    throw new ArgumentException(
            //        string.Format(
            //            "Some root load link expressions contain a cycle: {0}.",
            //            String.Join(",", cycleAsString)
            //        ),
            //        "loadLinkExpressions"
            //    );
            //}
        }
        #endregion

        //stle: should go in protocol
        public List<ILoadLinkExpression> GetLoadExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, _allLoadLinkExpressions);
        }

        //stle: should go in protocol
        public List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource, _allLoadLinkExpressions, referenceType);
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource, loadLinkExpressions)
                .Where(loadLinkExpression => loadLinkExpression.ReferenceTypes.Contains(referenceType))
                .ToList();
        }

        public List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions) {
            var linkedSourceType = linkedSource.GetType();
            return loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType.Equals(linkedSourceType))
                .ToList();
        }

        public ILinkedSourceConfig<TLinkedSource> GetLinkedSourceConfig<TLinkedSource>()
        {
            //stle: lazy load is good enough

            return CreateLinkedSourceConfig<TLinkedSource>();
        }

        public ILinkedSourceConfig<TLinkedSource> CreateLinkedSourceConfig<TLinkedSource>()
        {
            Type ctorGenericType = typeof(LinkedSourceConfig<,>);

            var linkedSourceType = typeof(TLinkedSource);

            Type[] typeArgs ={
                linkedSourceType,
                //stle: dry; !move this into LinkedSourceExpression
                LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>.GetLinkedSourceModelType(linkedSourceType)
            };

            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            //stle: simplify this
            var referenceTypeToBeLoadedForEachLoadingLevel = _referenceTypeByLoadingLevelParser
                .ParseReferenceTypeByLoadingLevel(linkedSourceType);

            var ctor = ctorSpecificType.GetConstructors().Single();
            var args = new object[]{
                referenceTypeToBeLoadedForEachLoadingLevel,
                this
            };
            var uncasted = ctor.Invoke(args);
            return (ILinkedSourceConfig<TLinkedSource>)uncasted;
        }
    }
}

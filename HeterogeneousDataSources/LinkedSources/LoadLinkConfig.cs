using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        private readonly List<ILoadLinkExpression> _allLoadLinkExpressions;

        #region Initialization
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions)
        {
            EnsureLoadLinkExpressionLinkTargetIdsAreUnique(loadLinkExpressions);
            _allLoadLinkExpressions = loadLinkExpressions;
            InitLoadingLevelsForEachPossibleRootLinkedSourceType();
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
        #endregion

        public List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource)
                .Where(loadLinkExpression => loadLinkExpression.ReferenceTypes.Contains(referenceType))
                .ToList();
        }

        public List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource){
            return GetLoadLinkExpressions(linkedSource.GetType());
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions(Type linkedSourceType) {
            return _allLoadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType == linkedSourceType)
                .ToList();
        }

        public ILoadLinker<TRootLinkedSource> CreateLoadLinker<TRootLinkedSource>(IReferenceLoader referenceLoader) 
        {
            return LinkedSourceConfigs.GetConfigFor<TRootLinkedSource>().CreateLoadLinker(
                referenceLoader,
                GetLoadingLevelsFor<TRootLinkedSource>(),
                this
            );
        }

        #region Loading Levels
        private Dictionary<Type, List<List<Type>>> _loadingLevelsByRootLinkedSourceType;

        private List<List<Type>> GetLoadingLevelsFor<TRootLinkedSource>(){
            var rootLinkedSourceType = typeof (TRootLinkedSource);
            if (!_loadingLevelsByRootLinkedSourceType.ContainsKey(rootLinkedSourceType)) {
                throw new InvalidOperationException(
                    String.Format(
                        "The type {0} cannot be used as root linked source because there are no load link expression associated with this linked source.",
                        rootLinkedSourceType
                    )
                );
            }

            return _loadingLevelsByRootLinkedSourceType[rootLinkedSourceType];
        }

        private void InitLoadingLevelsForEachPossibleRootLinkedSourceType() {
            _loadingLevelsByRootLinkedSourceType = new Dictionary<Type, List<List<Type>>>();
            foreach (var rootLinkedSourceType in GetAllPossibleRootLinkedSourceTypes()) {
                var rootReferenceTree = CreateRootReferenceTree(rootLinkedSourceType);
                var loadingLevels = rootReferenceTree.ParseLoadLevels();
                _loadingLevelsByRootLinkedSourceType.Add(rootLinkedSourceType, loadingLevels);
            }
        }

        private List<Type> GetAllPossibleRootLinkedSourceTypes() {
            return _allLoadLinkExpressions
                .Select(loadLinkExpression => loadLinkExpression.LinkedSourceType)
                .Distinct()
                .ToList();
        }
        #endregion

        #region Reference Trees
        public void AddReferenceTreeForEachLinkTarget(Type linkedSourceType, ReferenceTree parent) {
            foreach (var loadLinkExpression in GetLoadLinkExpressions(linkedSourceType)){
                loadLinkExpression.AddReferenceTreeForEachInclude(parent, this);
            }
        }

        public ReferenceTree CreateRootReferenceTree(Type rootLinkedSourceType) {
            var rootLinkedSourceConfig = LinkedSourceConfigs.GetConfigFor(rootLinkedSourceType);
            var rootReferenceTree = new ReferenceTree(
                rootLinkedSourceConfig.LinkedSourceModelType,
                "root",
                null
            );

            try {
                AddReferenceTreeForEachLinkTarget(rootLinkedSourceConfig.LinkedSourceType, rootReferenceTree);
            }
            catch (NotSupportedException ex) {
                throw new NotSupportedException(
                    string.Format(
                        "Unable to create root reference tree for {0}. For more details, see inner exception.",
                        rootLinkedSourceConfig.LinkedSourceType
                    ),
                    ex
                );
            }

            return rootReferenceTree;
        }

        #endregion
    }
}

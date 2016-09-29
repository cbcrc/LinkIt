using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Core.Interfaces;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;

namespace LinkIt.Core {
    //In addition to the responsiblies of ILoadLinkProtocol,
    //responsible for gathering and giving access to the load link expressions
    //responsible to infer loading levels for each possible root linked source
    public class LoadLinkProtocol : ILoadLinkProtocol
    {
        private readonly List<ILoadLinkExpression> _allLoadLinkExpressions;
        private readonly Func<IReferenceLoader> _createReferenceLoader;

        #region Initialization
        internal LoadLinkProtocol(
            List<ILoadLinkExpression> loadLinkExpressions, 
            Func<IReferenceLoader> createReferenceLoader)
        {
            _allLoadLinkExpressions = loadLinkExpressions;
            _createReferenceLoader = createReferenceLoader;
            InitLoadingLevelsForEachPossibleRootLinkedSourceType();
        }

        #endregion

        public ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>() {
            return LinkedSourceConfigs.GetConfigFor<TRootLinkedSource>().CreateLoadLinker(
                _createReferenceLoader(),
                GetLoadingLevelsFor<TRootLinkedSource>(),
                this
            );
        }

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

                List<List<Type>> loadingLevels;
                try {
                    loadingLevels = rootReferenceTree.ParseLoadingLevels();
                }
                catch (NotSupportedException ex) {
                    throw new NotSupportedException(
                        $"Unable to create the load link protocol for {rootLinkedSourceType}. For more details, see inner exception.",
                        ex
                    );
                }
                
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
                $"root of {rootLinkedSourceType}",
                null
            );

            try {
                AddReferenceTreeForEachLinkTarget(rootLinkedSourceConfig.LinkedSourceType, rootReferenceTree);
            }
            catch (NotSupportedException ex) {
                throw new NotSupportedException(
                    string.Format(
                        "Unable to create the load link protocol for {0}. For more details, see inner exception.",
                        rootLinkedSourceConfig.LinkedSourceType
                    ),
                    ex
                );
            }

            return rootReferenceTree;
        }

        #endregion

        public LoadLinkProtocolStatistics Statistics{
            get { return new LoadLinkProtocolStatistics(_loadingLevelsByRootLinkedSourceType); }
        }
    }
}

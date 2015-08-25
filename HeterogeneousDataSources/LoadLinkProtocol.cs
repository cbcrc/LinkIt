using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkProtocol {
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<ILoadLinkExpression> _loadLinkExpressions;

        public LoadLinkProtocol(IReferenceLoader referenceLoader, List<ILoadLinkExpression> loadLinkExpressions)
        {
            _referenceLoader = referenceLoader;
            _loadLinkExpressions = loadLinkExpressions;
        }

        public TLinkedSource LoadLink<TLinkedSource>(TLinkedSource linkedSource)
        {
            var dataContext = new LoadedReferenceContext();
            Load(linkedSource, dataContext);

            Link(linkedSource, dataContext);

            return linkedSource;
        }

        private void Load(
            object linkedSource,
            LoadedReferenceContext loadedReferenceContext)
        {
            var lookupIdContext = new LookupIdContext();

            foreach (var loadLinkExpression in _loadLinkExpressions)
            {
                loadLinkExpression.AddLookupIds(linkedSource, lookupIdContext);
            }

            _referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);

            _referenceLoader.Dispose();
        }

        private void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkExpression in _loadLinkExpressions) {
                linkExpression.Link(linkedSource, loadedReferenceContext);
            }
        }
    }
}

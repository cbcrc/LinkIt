using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkProtocol {
        public LoadLinkProtocol(List<IReferenceTypeConfig> referenceTypeConfigs, List<ILoadLinkExpression> loadLinkExpressions)
        {
            ReferenceTypeConfigs = referenceTypeConfigs;
            LoadLinkExpressions = loadLinkExpressions;
        }

        private List<IReferenceTypeConfig> ReferenceTypeConfigs { get; set; }
        private List<ILoadLinkExpression> LoadLinkExpressions { get; set; }

        public TLinkedSource LoadLink<TLinkedSource>(TLinkedSource linkedSource)
        {
            var dataContext = new DataContext();
            Load(linkedSource, dataContext);

            Link(linkedSource, dataContext);

            return linkedSource;
        }

        private void Load(
            object linkedSource,
            DataContext dataContext) 
        {
            foreach (var referenceTypeConfig in ReferenceTypeConfigs) {
                referenceTypeConfig.LoadReferences(linkedSource, LoadLinkExpressions, dataContext);
            }
        }

        private void Link(object linkedSource, DataContext dataContext) {
            foreach (var linkExpression in LoadLinkExpressions) {
                linkExpression.Link(linkedSource, dataContext);
            }
        }
    }
}

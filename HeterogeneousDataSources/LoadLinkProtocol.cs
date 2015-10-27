using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkProtocol {
        private readonly IReferenceLoader _referenceLoader;
        private readonly LoadLinkConfig _config;

        public LoadLinkProtocol(IReferenceLoader referenceLoader, LoadLinkConfig config)
        {
            _referenceLoader = referenceLoader;
            _config = config;
        }

        public IReferenceLoader ReferenceLoader { get { return _referenceLoader; } }

        //stle: TId doesnot make sense in that context
        //stle: dry
        public TRootLinkedSource LoadLinkModel<TRootLinkedSource,TId>(object model)
        {
            var loadLinker = _config
                .GetLinkedSourceConfig<TRootLinkedSource>()
                .CreateLoadLinker(_referenceLoader);

            return loadLinker.FromModel(model);
        }

        //stle: TId doesnot make sense in that context
        //stle: dry
        public List<TRootLinkedSource> LoadLinkModel<TRootLinkedSource,TId>(List<object> models) {
            var loadLinker = _config
                .GetLinkedSourceConfig<TRootLinkedSource>()
                .CreateLoadLinker(_referenceLoader);

            return loadLinker.FromModel(models);
        }

        public TRootLinkedSource LoadLink<TRootLinkedSource,TId>(TId modelId)
        {
            var loadLinker = _config
                .GetLinkedSourceConfig<TRootLinkedSource>()
                .CreateLoadLinker(_referenceLoader);

            return loadLinker.ById(modelId);
        }
    }
}

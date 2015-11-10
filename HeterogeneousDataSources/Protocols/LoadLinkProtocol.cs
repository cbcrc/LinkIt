namespace HeterogeneousDataSources.Protocols {
    public class LoadLinkProtocol {
        private readonly IReferenceLoader _referenceLoader;
        private readonly LoadLinkConfig _config;

        public LoadLinkProtocol(IReferenceLoader referenceLoader, LoadLinkConfig config)
        {
            _referenceLoader = referenceLoader;
            _config = config;
        }

        public IReferenceLoader ReferenceLoader { get { return _referenceLoader; } }

        public ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>()
        {
            return _config.CreateLoadLinker<TRootLinkedSource>(_referenceLoader);
        }
    }
}

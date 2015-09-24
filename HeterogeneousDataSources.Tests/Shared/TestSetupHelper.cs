namespace HeterogeneousDataSources.Tests.Shared {
    public static class TestSetupHelper {
        public static ReferenceTypeByLoadingLevelParser CreateReferenceTypeByLoadingLevelParser(LoadLinkProtocolBuilder loadLinkProtocolBuilder) {
            var factory = new LoadLinkExpressionTreeFactory(loadLinkProtocolBuilder.GetLoadLinkExpressions());
            return new ReferenceTypeByLoadingLevelParser(factory);
        }

    }
}

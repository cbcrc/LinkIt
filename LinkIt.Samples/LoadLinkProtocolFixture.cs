using System;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.PublicApi;
using LinkIt.Shared;
using Xunit;

namespace LinkIt.Samples
{
    public class LoadLinkProtocolFixture
    {
        public LoadLinkProtocolFixture()
        {
            try
            {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            LoadLinkProtocol = loadLinkProtocolBuilder.Build(
                () => new FakeReferenceLoader(),
                Assembly.GetExecutingAssembly().Yield(),
                LoadLinkExpressionConvention.Default);
            }
            catch (Exception e)
            {
                Assert.Null(e);
            }
        }

        public ILoadLinkProtocol LoadLinkProtocol { get; }
    }
}
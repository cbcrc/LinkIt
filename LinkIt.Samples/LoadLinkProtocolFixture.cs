using System;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.PublicApi;
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
                new [] { Assembly.GetExecutingAssembly() },
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
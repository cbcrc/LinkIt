// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

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
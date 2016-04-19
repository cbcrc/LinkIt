#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Tests.ReferenceTrees {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTree_SimplestRootLinkedSourceTests
    {
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>();
            _sut = (LoadLinkProtocol) loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void CreateRootReferenceTree() {
            var actual = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void ParseLoadingLevels() {
            var rootReferenceTree = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var actual = rootReferenceTree.ParseLoadingLevels();

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
        }

        public class Model {
            public int Id { get; set; }
        }
    }
}

#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Tests.Core.Exploratory.Generics
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class GenericPieTests{
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<StringPieLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PieContent,
                    linkedSource => linkedSource.SummaryImage
                );

            loadLinkProtocolBuilder.For<IntPieLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PieContent,
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(() => 
                new ReferenceLoaderStub(
                    new ReferenceTypeConfig<Pie<string>, string>(
                        ids => new PieRepository<string>().GetByPieContentIds(ids),
                        reference => reference.Id
                    ),
                    new ReferenceTypeConfig<Pie<int>, string>(
                        ids => new PieRepository<int>().GetByPieContentIds(ids),
                        reference => reference.Id
                    )
                )
            );
        }

        [Test]
        public void LoadLink_StringPie()
        {
            var actual = _sut.LoadLink<StringPieLinkedSource>().ById("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_IntPie() {
            var actual = _sut.LoadLink<IntPieLinkedSource>().ById("2");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

    }

    public class StringPieLinkedSource : ILinkedSource<Pie<string>> {
        public Pie<string> Model { get; set; }
        public Image SummaryImage { get; set; }
    }

    public class IntPieLinkedSource : ILinkedSource<Pie<int>> {
        public Pie<int> Model { get; set; }
        public Image SummaryImage { get; set; }
    }
}
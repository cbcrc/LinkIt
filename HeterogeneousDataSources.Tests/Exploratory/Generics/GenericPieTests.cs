using ApprovalTests.Reporters;
using HeterogeneousDataSources.ConfigBuilders;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Exploratory.Generics
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class GenericPieTests
    {
        private FakeReferenceLoader<object, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

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

            _fakeReferenceLoader = 
                new FakeReferenceLoader<object, string>(
                    new ReferenceTypeConfig<Pie<string>, string>(
                        ids => new PieRepository<string>().GetByPieContentIds(ids),
                        reference => reference.Id
                    ),
                    new ReferenceTypeConfig<Pie<int>, string>(
                        ids => new PieRepository<int>().GetByPieContentIds(ids),
                        reference => reference.Id
                    )
                );

            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
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
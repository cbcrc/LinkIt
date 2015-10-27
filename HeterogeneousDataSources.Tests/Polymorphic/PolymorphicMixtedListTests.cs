using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicMixtedListTests {
        private FakeReferenceLoader<Model, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.TargetReference,
                    linkedSource => linkedSource.Target,
                    link => link.GetType(),
                    includes => includes
                        .WhenReference<Person,string>(
                            typeof(int),
                            link=>link.ToString()
                        )
                        .WhenNestedLinkedSource<PersonLinkedSource, string>(
                            typeof(string),
                            link => link.ToString()
                        )
                        .WhenSubLinkedSource<PersonLinkedSource, Person>(
                            typeof(Person)
                        )
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<Model, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_PolymorphicMixteList() {
            _fakeReferenceLoader.FixValue(
                new Model {
                    Id = "1",
                    TargetReference = new List<object>{
                        1,
                        "nested",
                        new Person{
                            Id = "as-sub-linked-source",
                            Name = "The Name",
                            SummaryImageId = "the-id"
                        }
                    }
                }
            );

            var actual = _sut.LoadLink<LinkedSource,string>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<object> Target { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public List<object> TargetReference { get; set; }
        }
    }
}

using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicMixtedListTests {
        private LoadLinkConfig _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.TargetReference,
                    linkedSource => linkedSource.Target,
                    link => link.GetType(),
                    includes => includes
                        .Include<Person>().AsReferenceById(
                            typeof(int),
                            link=>link.ToString()
                        )
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceById(
                            typeof(string),
                            link => link.ToString()
                        )
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceFromModel(
                            typeof(Person),
                            link => (Person)link
                        )
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            
            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_PolymorphicMixteList() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
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

using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;

namespace LinkIt.Tests.Core.Exploratory {
    public class ContextualizationFromModelTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.Person,
                    linkedSource => linkedSource.Person,
                    (linkedSource, childLinkedSource) => 
                        childLinkedSource.Contextualization = linkedSource.Model.PersonContextualization
                );
            loadLinkProtocolBuilder.For<PersonContextualizedLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => 
                        linkedSource.Contextualization?.SummaryImageId ?? 
                        linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void LoadLink_WithoutContextualization_ShouldLinkDefaultImage()
        {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Person = new Person {
                        Id = "32",
                        Name = "dont-care",
                        SummaryImageId = "defaultSummaryImageId"
                    },
                    PersonContextualization = new PersonContextualization {
                        Id = "32",
                        Name = "dont-care",
                        SummaryImageId = null
                    }
                }
            );

            Assert.That(actual.Person.Contextualization.SummaryImageId, Is.Null);
            Assert.That(actual.Person.SummaryImage.Id, Is.EqualTo("defaultSummaryImageId"));
        }

        [Fact]
        public void LoadLink_WithContextualization_ShouldLinkOverriddenImage() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Person = new Person {
                        Id = "32",
                        Name = "dont-care",
                        SummaryImageId = "defaultSummaryImageId"
                    },
                    PersonContextualization = new PersonContextualization {
                        Id = "32",
                        Name = "dont-care",
                        SummaryImageId = "overriddenSummaryImageId"
                    }
                }
            );

            Assert.That(actual.Person.Contextualization.SummaryImageId, Is.EqualTo("overriddenSummaryImageId"));
            Assert.That(actual.Person.SummaryImage.Id, Is.EqualTo("overriddenSummaryImageId"));
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public PersonContextualizedLinkedSource Person { get; set; }
        }

        public class Model {
            public Person Person { get; set; }
            public PersonContextualization PersonContextualization { get; set; }
        }
    }

   
}

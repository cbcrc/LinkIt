using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Exploratory
{
    public class ContextualizationFromModelTests
    {
        public ContextualizationFromModelTests()
        {
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
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        private readonly ILoadLinkProtocol _sut;

        public class LinkedSource : ILinkedSource<Model>
        {
            public PersonContextualizedLinkedSource Person { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public Person Person { get; set; }
            public PersonContextualization PersonContextualization { get; set; }
        }

        [Fact]
        public void LoadLink_WithContextualization_ShouldLinkOverriddenImage()
        {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model
                {
                    Person = new Person
                    {
                        Id = "32",
                        Name = "dont-care",
                        SummaryImageId = "defaultSummaryImageId"
                    },
                    PersonContextualization = new PersonContextualization
                    {
                        Id = "32",
                        Name = "dont-care",
                        SummaryImageId = "overriddenSummaryImageId"
                    }
                }
            );

            Assert.Equal("overriddenSummaryImageId", actual.Person.Contextualization.SummaryImageId);
            Assert.Equal("overriddenSummaryImageId", actual.Person.SummaryImage.Id);
        }

        [Fact]
        public void LoadLink_WithoutContextualization_ShouldLinkDefaultImage()
        {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model
                {
                    Person = new Person
                    {
                        Id = "32",
                        Name = "dont-care",
                        SummaryImageId = "defaultSummaryImageId"
                    },
                    PersonContextualization = new PersonContextualization
                    {
                        Id = "32",
                        Name = "dont-care",
                        SummaryImageId = null
                    }
                }
            );

            Assert.Null(actual.Person.Contextualization.SummaryImageId);
            Assert.Equal("defaultSummaryImageId", actual.Person.SummaryImage.Id);
        }
    }
}
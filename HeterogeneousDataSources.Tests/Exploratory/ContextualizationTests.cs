using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Exploratory {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ContextualizationTests
    {
        private FakeReferenceLoader<WithContextualizedReference, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithContextualizedReferenceLinkedSource>()
                .IsRoot<string>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.PersonContextualization.Id,
                    linkedSource => linkedSource.Person
                );
            loadLinkProtocolBuilder.For<PersonContextualizedLinkedSource>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Contextualization.SummaryImageId ?? linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<WithContextualizedReference, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_WithoutContextualization_ShouldLinkDefaultImage()
        {
            _fakeReferenceLoader.FixValue(
                new WithContextualizedReference {
                    Id = "1",
                    PersonContextualization = new PersonContextualization{
                        Id = "32",
                        Name = "Altered named",
                        SummaryImageId = null
                    }
                }
            );

            var actual = _sut.LoadLink<WithContextualizedReferenceLinkedSource>("1");

            Assert.That(actual.Person.Contextualization.SummaryImageId, Is.Null);
            Assert.That(actual.Person.SummaryImage.Id, Is.EqualTo("person-img-32"));
        }

        [Test]
        public void LoadLink_WithContextualization_ShouldLinkOverriddenImage() {
            _fakeReferenceLoader.FixValue(
                new WithContextualizedReference {
                    Id = "1",
                    PersonContextualization = new PersonContextualization {
                        Id = "32",
                        Name = "Altered named",
                        SummaryImageId = "overriden-image"
                    }
                }
            );

            var actual = _sut.LoadLink<WithContextualizedReferenceLinkedSource>("1");

            Assert.That(actual.Person.Contextualization.SummaryImageId, Is.EqualTo("overriden-image"));
            Assert.That(actual.Person.SummaryImage.Id, Is.EqualTo("overriden-image"));
        }

    }

    public class WithContextualizedReferenceLinkedSource : ILinkedSource<WithContextualizedReference>
    {
        public WithContextualizedReference Model { get; set; }
        public PersonContextualizedLinkedSource Person { get; set; }
    }

    public class PersonContextualizedLinkedSource: ILinkedSource<Person>
    {
        public Person Model { get; set; }
        public PersonContextualization Contextualization { get; set; }
        public Image SummaryImage { get; set; }
    }

    public class WithContextualizedReference {
        public string Id { get; set; }
        public PersonContextualization PersonContextualization { get; set; }
    }

    public class PersonContextualization{
        public string Id { get; set; }
        public string Name { get; set; }
        public string SummaryImageId { get; set; }
    }
}

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
        private LoadLinkProtocolFactory<WithContextualizedReference, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<WithContextualizedReference, string>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<WithContextualizedReferenceLinkedSource, WithContextualizedReference, string>(),

                    new NestedLinkedSourceLoadLinkExpression<WithContextualizedReferenceLinkedSource, PersonContextualizedLinkedSource, Person, string>(
                        linkedSource => linkedSource.Model.PersonContextualization.Id,
                        (linkedSource, nestedLinkedSource) => {
                            nestedLinkedSource.Contextualization = linkedSource.Model.PersonContextualization;
                            linkedSource.Person = nestedLinkedSource;
                        }),
                    new ReferenceLoadLinkExpression<PersonContextualizedLinkedSource,Image, string>(
                        linkedSource => linkedSource.Contextualization.SummaryImageId ?? linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference)
                },
                getReferenceIdFunc: reference => reference.Id
            );
        }

        [Test]
        public void LoadLink_WithoutContextualization_ShouldLinkDefaultImage()
        {
            var sut = _loadLinkProtocolFactory.Create(
                new WithContextualizedReference {
                    Id = "1",
                    PersonContextualization = new PersonContextualization{
                        Id = "32",
                        Name = "Altered named",
                        SummaryImageId = null
                    }
                }
            );

            var actual = sut.LoadLink<WithContextualizedReferenceLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_WithContextualization_ShouldLinkOverriddenImage() {
            var sut = _loadLinkProtocolFactory.Create(
                new WithContextualizedReference {
                    Id = "1",
                    PersonContextualization = new PersonContextualization {
                        Id = "32",
                        Name = "Altered named",
                        SummaryImageId = "overriden-image"
                    }
                }
            );

            var actual = sut.LoadLink<WithContextualizedReferenceLinkedSource>("1");

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

using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;
using LinkIt.Tests.Core;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;

namespace LinkIt.Tests.ReferenceTrees {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTree_SideEffectBetweenNestedLinkedSourceTests {
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.PersonId,
                    linkedSource => linkedSource.Person
                )
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.PersonGroupId,
                    linkedSource => linkedSource.PersonGroup
                );
            loadLinkProtocolBuilder.For<PersonGroupLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PersonIds,
                    linkedSource => linkedSource.People
                );


            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            _sut = (LoadLinkProtocol)loadLinkProtocolBuilder.Build(
                () => null //not required
            );
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

            Assert.That(actual[2].Contains(typeof(Person)));
            Assert.That(actual[3].Contains(typeof(Image)));
            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public PersonLinkedSource Person { get; set; }
            public PersonGroupLinkedSource PersonGroup { get; set; }
        }

        public class PersonGroupLinkedSource : ILinkedSource<PersonGroup> {
            public PersonGroup Model { get; set; }
            public List<Person> People { get; set; }
        }

        public class Model {
            public int Id { get; set; }
            public string PersonId { get; set; }
            public int PersonGroupId { get; set; }
        }

        public class PersonGroup {
            public int Id { get; set; }
            public List<string> PersonIds { get; set; }
        }

    }
}

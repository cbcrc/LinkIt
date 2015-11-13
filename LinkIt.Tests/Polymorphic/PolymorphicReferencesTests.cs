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
    public class PolymorphicReferencesTests {
        private FakeReferenceLoader<Model, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes
                        .Include<Image>().AsReferenceById(
                            "image",
                            link=>link.Id
                        )
                        .Include<Person>().AsReferenceById(
                            "person",
                            link => link.Id
                        )
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<Model, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_PolymorphicReferenceWithImage() {
            _fakeReferenceLoader.FixValue(
                new Model {
                    Id = "1",
                    Target = new List<PolymorphicReference>
                    {
                        new PolymorphicReference {
                            Type = "person",
                            Id = "a"
                        },
                        new PolymorphicReference{
                            Type = "image",
                            Id = "a"
                        }
                    }
                }
            );

            var actual = _sut.LoadLink<LinkedSource>().ById("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<object> Target { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public List<PolymorphicReference> Target { get; set; }
        }

        //stle: should be shared
        public class PolymorphicReference {
            public string Type { get; set; }
            public string Id { get; set; }
        }
    }
}

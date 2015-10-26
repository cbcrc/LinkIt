using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicReferenceTests {
        private FakeReferenceLoader<Model, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .IsRoot<string>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes
                        .WhenReference<Image,string>(
                            "image",
                            link=>link.Id
                        )
                        .WhenReference<Person, string>(
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
                    Target = new PolymorphicReference{
                        Type = "image",
                        Id = "a"
                    }
                }
            );

            var actual = _sut.LoadLink<LinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_PolymorphicReferenceWithPerson() {
            _fakeReferenceLoader.FixValue(
                new Model {
                    Id = "1",
                    Target = new PolymorphicReference {
                        Type = "person",
                        Id = "a"
                    }
                }
            );

            var actual = _sut.LoadLink<LinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public object Target { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public PolymorphicReference Target { get; set; }
        }

        public class PolymorphicReference {
            public string Type { get; set; }
            public string Id { get; set; }
        }
    }
}

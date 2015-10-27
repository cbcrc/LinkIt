using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkModelTests
    {
        private FakeReferenceLoader<SingleReferenceContent, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<SingleReferenceContent, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_WithModel_ShouldLinkModel() {
            var actual = _sut.LoadLinkModel<SingleReferenceLinkedSource,string>(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            Assert.That(actual.Model.Id, Is.EqualTo("1"));
            Assert.That(actual.SummaryImage.Id, Is.EqualTo("a"));
        }

        //stle: after new syntaxe
        [Ignore]
        [Test]
        public void LoadLink_WithWrontModelType_ShouldThrow() {
            TestDelegate act = () => _sut.LoadLinkModel<SingleReferenceLinkedSource, string>(
                "The model of SingleReferenceLinkedSource is not a string"
            );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("SingleReferenceContent").And
                .With.Message.ContainsSubstring("string")
            );
        }

        //stle: after new syntaxe
        [Ignore]
        [Test]
        public void LoadLink_ModelWithNullModel_ShouldReturnNull() {
            var actual = _sut.LoadLinkModel<SingleReferenceLinkedSource, string>(
                null
            );

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void LoadLink_WithModels_ShouldLinkModels() {
            var actual = _sut.LoadLinkModel<SingleReferenceLinkedSource,string>(
                new List<object>{
                    new SingleReferenceContent {
                        Id = "1",
                        SummaryImageId = "a"
                    },
                    new SingleReferenceContent {
                        Id = "2",
                        SummaryImageId = "b"
                    }
                }
            );

            var summaryImageIds = actual
                .Select(item => item.SummaryImage.Id)
                .ToList();
            Assert.That(summaryImageIds, Is.EquivalentTo(new[] {"a", "b"}));
        }

        [Test]
        public void LoadLink_ModelsWithNullModel_ShouldLinkNull() {
            var actual = _sut.LoadLinkModel<SingleReferenceLinkedSource, string>(
                   new List<object>{null}
            );
            
            Assert.That(actual, Is.EquivalentTo(new List<SingleReferenceLinkedSource>{null}));
        }

        //stle: after new syntaxe
        [Ignore]
        [Test]
        public void LoadLink_ModelsWithWrontModelType_ShouldThrow() {
            TestDelegate act = () => _sut.LoadLinkModel<SingleReferenceLinkedSource, string>(
                new List<object>{
                    "The model of SingleReferenceLinkedSource is not a string"
                }
            );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("SingleReferenceContent").And
                .With.Message.ContainsSubstring("string")
            );
        }


    }
}

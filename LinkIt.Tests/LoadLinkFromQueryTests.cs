using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Protocols;
using LinkIt.Protocols.Interfaces;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkFromQueryTests
    {
        private LoadLinkProtocol _sut;
        private ReferenceLoaderStub _referenceLoaderStub;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            _referenceLoaderStub = new ReferenceLoaderStub();
            _sut = loadLinkProtocolBuilder.Build(()=>_referenceLoaderStub);
        }

        [Test]
        public void LoadLink_FromQuery_ShouldLinkModels() {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>()
                .FromQuery(()=>FakeQuery("dont-care"));

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_FromQueryWithNullModel_ShouldLinkNull() {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>()
                .FromQuery(() => new List<SingleReferenceContent>{null});

            Assert.That(actual, Is.EquivalentTo(new List<SingleReferenceLinkedSource> { null }));
        }

        [Test]
        public void LoadLink_FromQueryWithWrongModelType_ShouldThrow() {
            TestDelegate act = () => _sut.LoadLink<SingleReferenceLinkedSource>()
                .FromQuery(()=>
                    new List<string>{"The model of SingleReferenceLinkedSource is not a string"}
                );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("SingleReferenceContent").And
                .With.Message.ContainsSubstring("String")
            );
        }

        [Test]
        public void LoadLink_FromQueryWithException_ShouldDisposeReferenceLoader()
        {
            try
            {
                _sut.LoadLink<SingleReferenceLinkedSource>()
                    .FromQuery(
                        () =>
                        {
                            throw new Exception("pow");
                            return FakeQuery("dont-care");
                        }
                    );
            }
            catch{
                //Ignore exception
            }

            Assert.That(_referenceLoaderStub.IsDisposed, Is.True);
        }

        [Test]
        public void LoadLink_FromQueryWithDependencyOnReferenceLoader_ShouldPassSameReferenceLoaderInstance()
        {
            IReferenceLoader transiantReferenceLoader=null;
            _sut.LoadLink<SingleReferenceLinkedSource>()
                .FromQuery(
                    referenceLoader => {
                        transiantReferenceLoader = referenceLoader;
                        return new List<SingleReferenceContent>();
                    }
                );

            Assert.That(transiantReferenceLoader, Is.Not.Null);
            Assert.That(ReferenceEquals(transiantReferenceLoader, _referenceLoaderStub), Is.True);
        }


        private List<SingleReferenceContent> FakeQuery(string keyword)
        {
            return new List<SingleReferenceContent>
            {
                new SingleReferenceContent
                {
                    Id = "1",
                    SummaryImageId = "a"
                },
                new SingleReferenceContent
                {
                    Id = "2",
                    SummaryImageId = "b"
                }
            };
        }
    }
}

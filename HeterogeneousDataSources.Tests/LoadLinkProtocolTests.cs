using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocolTests
    {
        [Test]
        public void LoadLink_ShouldDisposeLoader()
        {
            var referenceLoader = new FakeReferenceLoader();
            var sut = new LoadLinkProtocol(referenceLoader, 
                new LoadLinkConfig(
                    new List<ILoadLinkExpression>{
                        new RootLoadLinkExpression<WithoutReferenceLinkedSource, Image, string>(),
                    }, 
                    new[] {
                        new List<Type>{typeof(string)},
                    }
                )
            );

            var actual = sut.LoadLink<WithoutReferenceLinkedSource>("dont-care");

            Assert.That(referenceLoader.IsDisposed, Is.True);
        }

        [Test]
        public void LoadLink_ModelIdIsNull_ShouldThrow() {
            var referenceLoader = new FakeReferenceLoader();
            var sut = new LoadLinkProtocol(referenceLoader,
                new LoadLinkConfig( new List<ILoadLinkExpression>{} )
            );

            TestDelegate act = () => sut.LoadLink<WithoutReferenceLinkedSource>(null);

            Assert.That(
                act, 
                Throws
                    .InstanceOf<ArgumentNullException>()
                    .With.Message.ContainsSubstring("modelId")
            );
        }

        [Test]
        public void LoadLink_NotARootLinkedSourceType_ShouldThrow() {
            var referenceLoader = new FakeReferenceLoader();
            var sut = new LoadLinkProtocol(referenceLoader,
                new LoadLinkConfig(new List<ILoadLinkExpression> { })
            );

            TestDelegate act = () => sut.LoadLink<WithoutReferenceLinkedSource>("dont-care");

            Assert.That(
                act,
                Throws.ArgumentException.With.Message.ContainsSubstring("WithoutReferenceLinkedSource")
            );
        }


    }

    public class WithoutReferenceLinkedSource : ILinkedSource<Image> {
        public Image Model { get; set; }
    }
}
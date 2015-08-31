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
    public class LoadLinkProtocol_LifeCycleTests
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
    }

    public class WithoutReferenceLinkedSource : ILinkedSource<Image> {
        public Image Model { get; set; }
    }
}
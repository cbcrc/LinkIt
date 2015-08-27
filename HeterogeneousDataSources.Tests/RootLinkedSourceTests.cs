using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class RootLinkedSourceTests
    {
        [Test]
        public void LoadLink_WithoutReferenceId_ShouldLinkNull() {
            var referenceLoader = new FakeReferenceLoader(
                new ReferenceTypeConfig<RootContent, string>(
                    ids => new RootContentRepository().GetByIds(ids),
                    reference => reference.Id
                )
            );

            var sut = new LoadLinkProtocol(
                referenceLoader,
                new LoadLinkConfig(
                    GetLoadLinkExpressions(),
                    FakeReferenceTypeForLoadingLevel()
                )
            );

            var actual = sut.LoadLink<RootLinkedSource, string, RootContent>(null);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void LoadLink_CannotBeResolved_ShouldLinkNull() {
            var referenceLoader = new FakeReferenceLoader(
                new ReferenceTypeConfig<RootContent, string>(
                    ids => new RootContentRepository().GetByIds(ids),
                    reference => reference.Id
                )
            );

            var sut = new LoadLinkProtocol(
                referenceLoader,
                new LoadLinkConfig(
                    GetLoadLinkExpressions(),
                    FakeReferenceTypeForLoadingLevel()
                )
            );

            var actual = sut.LoadLink<RootLinkedSource, string, RootContent>("cannot-be-resolved");

            Assert.That(actual, Is.Null);
        }

        private static List<Type>[] FakeReferenceTypeForLoadingLevel() {
            return new[] {
                new List<Type>{typeof(RootContent)},
            };
        }

        private static List<ILoadLinkExpression> GetLoadLinkExpressions()
        {
            return new List<ILoadLinkExpression>{};
        }

        private static Func<SingleReferenceContent, string> GetReferenceIdFunc()
        {
            return reference => reference.Id;
        }
    }

    public class RootLinkedSource: ILinkedSource<RootContent>
    {
        public RootContent Model { get; set; }
    }

    public class RootContent {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class RootContentRepository {
        public List<RootContent> GetByIds(List<string> ids) {
            return ids
                .Where(id => id != "cannot-be-resolved")
                .Select(id => new RootContent {
                    Id = id,
                    Name = "name-"+id
                })
                .ToList();
        }
    }

}

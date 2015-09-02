using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTypeByLoadingLevelParserTests
    {
        private ReferenceTypeByLoadingLevelParser CreateSut(List<ILoadLinkExpression> loadLinkExpressions)
        {
            var factory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);
            return new ReferenceTypeByLoadingLevelParser(factory);
        }

        [Test]
        public void GetReferenceTypeForLoadingLevel_OneLevel()
        {
            var rootLoadLinkExpression =
                new RootLoadLinkExpression<OneLoadingLevelContentLinkedSource, OneLoadingLevelContent, string>();
            var sut = CreateSut(
                new List<ILoadLinkExpression>{
                    rootLoadLinkExpression,
                    new NestedLinkedSourceLoadLinkExpression<OneLoadingLevelContentRefererLinkedSource, OneLoadingLevelContentLinkedSource, OneLoadingLevelContent, string>(
                        linkedSource => linkedSource.Model.OneLoadingLevelContentId,
                        (linkedSource, childLinkedSource) => linkedSource.OneLoadingLevelContent = childLinkedSource
                    )
                }
            );

            var actual = sut.ParseReferenceTypeByLoadingLevel(rootLoadLinkExpression);

            var expected = new Dictionary<int, List<Type>>{
                {0, new List<Type> {typeof (OneLoadingLevelContent)}}
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        public class OneLoadingLevelContentLinkedSource : ILinkedSource<OneLoadingLevelContent> {
            public OneLoadingLevelContent Model { get; set; }
        }

        public class OneLoadingLevelContent {
            public string Id { get; set; }
        }

        public class OneLoadingLevelContentRefererLinkedSource : ILinkedSource<OneLoadingLevelContentReferer> {
            public OneLoadingLevelContentReferer Model { get; set; }
            public OneLoadingLevelContentLinkedSource OneLoadingLevelContent { get; set; }
        }

        public class OneLoadingLevelContentReferer {
            public string Id { get; set; }
            public string OneLoadingLevelContentId { get; set; }
        }

        [Test]
        public void GetReferenceTypeForLoadingLevel_ManyLevel()
        {
            var rootLoadLinkExpression =
                new RootLoadLinkExpression<ManyLoadingLevelContentLinkedSource, ManyLoadingLevelContent, string>();
            var sut = CreateSut(
                new List<ILoadLinkExpression>{
                    rootLoadLinkExpression,
                    new ReferenceLoadLinkExpression<ManyLoadingLevelContentLinkedSource, BlogPost, string>(
                        linkedSource => linkedSource.Model.BlogPostId,
                        (linkedSource, reference) => linkedSource.BlogPost = reference),
                    new ReferenceLoadLinkExpression<ManyLoadingLevelContentLinkedSource, Image, string>(
                        linkedSource => linkedSource.Model.PreImageId,
                        (linkedSource, reference) => linkedSource.PreImage = reference),
                    new NestedLinkedSourceLoadLinkExpression<ManyLoadingLevelContentLinkedSource, PersonLinkedSource, Person, string>(
                        linkedSource => linkedSource.Model.PersonId,
                        (linkedSource, nestedLinkedSource) => linkedSource.Person = nestedLinkedSource),
                    new ReferenceLoadLinkExpression<ManyLoadingLevelContentLinkedSource, Image, string>(
                        linkedSource => linkedSource.Model.PostImageId,
                        (linkedSource, reference) => linkedSource.PostImage = reference),
                    new ReferenceLoadLinkExpression<PersonLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference)
                }
            );

            var actual = sut.ParseReferenceTypeByLoadingLevel(rootLoadLinkExpression);

            var expected = new Dictionary<int, List<Type>>{
                {0, new List<Type> {typeof(ManyLoadingLevelContent)}},
                {1, new List<Type> {typeof(BlogPost), typeof(Person)}},
                {2, new List<Type> {typeof(Image)}}
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        public class ManyLoadingLevelContentLinkedSource : ILinkedSource<ManyLoadingLevelContent> {
            public ManyLoadingLevelContent Model { get; set; }
            public BlogPost BlogPost { get; set; }
            public Image PreImage { get; set; }
            public PersonLinkedSource Person { get; set; }
            public Image PostImage { get; set; }
        }

        public class ManyLoadingLevelContent {
            public string Id { get; set; }
            public string BlogPostId { get; set; }
            public string PreImageId { get; set; }
            public string PersonId { get; set; }
            public string PostImageId { get; set; }
        }

        public class BlogPost {
            public string Id { get; set; }
            public string Title { get; set; }
        }
    }
}

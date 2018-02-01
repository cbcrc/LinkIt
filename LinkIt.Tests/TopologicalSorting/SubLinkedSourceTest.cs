#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using LinkIt.TopologicalSorting;
using Xunit;

namespace LinkIt.Tests.TopologicalSorting
{
    public class SubLinkedSourceTest
    {
        private LoadLinkProtocol _sut;

        public SubLinkedSourceTest()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.PostThread,
                    linkedSource => linkedSource.PostThread
                );
            loadLinkProtocolBuilder.For<PostThreadLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.Posts,
                    linkedSource => linkedSource.Posts)
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.AuthorId,
                    linkedSource => linkedSource.Author);
            loadLinkProtocolBuilder.For<PostLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = (LoadLinkProtocol) loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void ParseLoadingLevels()
        {
            var dependencyGraph = _sut.CreateDependencyGraph(typeof(LinkedSource));

            var actual = dependencyGraph.Sort().GetLoadingLevels();

            Type[][] expected =
            {
                new[] { typeof(Model) },
                new[] { typeof(Image), typeof(Person) },
            };

            actual.Should().BeEquivalentTo(expected);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public PostThreadLinkedSource PostThread { get; set; }
            public Model Model { get; set; }
        }

        public class PostThreadLinkedSource : ILinkedSource<PostThread>
        {
            public List<PostLinkedSource> Posts { get; set; }
            public Person Author { get; set; }
            public PostThread Model { get; set; }
        }

        public class PostLinkedSource : ILinkedSource<Post>
        {
            public Image SummaryImage { get; set; }
            public Post Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public PostThread PostThread { get; set; }
        }

        public class PostThread
        {
            public string AuthorId { get; set; }
            public List<Post> Posts { get; set; }
        }

        public class Post
        {
            public string SummaryImageId { get; set; }
        }
    }
}
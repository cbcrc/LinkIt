﻿using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests;
using HeterogeneousDataSources.Tests.Shared;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.Conventions.Interfaces;
using LinkIt.LinkedSources.Interfaces;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Conventions.Tests.DefaultConventions
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkSingleValueWhenIdSuffixMatchesTests {
        [Test]
        public void GetLinkedSourceTypes(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkSingleValueWhenIdSuffixMatches() }
            );

            var fakeReferenceLoader =
                new FakeReferenceLoader<Model, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            var actual = sut.LoadLink<LinkedSource>().FromModel(
                new Model{
                    Id="One",
                    MediaReferenceId = 1,
                    MediaNestedLinkedSourceId = 2
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Media MediaReference { get; set; }
            public MediaLinkedSource MediaNestedLinkedSource { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public int MediaReferenceId { get; set; }
            public int MediaNestedLinkedSourceId { get; set; }
        }
    }
}

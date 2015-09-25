﻿using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicReferencesTests {
        private FakeReferenceLoader<Model, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .IsRoot<string>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    subContent => subContent.Type,
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

            var actual = _sut.LoadLink<LinkedSource>("1");

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

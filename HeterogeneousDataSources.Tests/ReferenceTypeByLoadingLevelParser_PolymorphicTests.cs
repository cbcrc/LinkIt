using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Polymorphic;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTypeByLoadingLevelParser_PolymorphicTests
    {
        private FakeReferenceLoader<NestedContents, int> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [Test]
        public void ParseReferenceTypeByLoadingLevel_WithNestedPerson_ShouldLoadPersonBeforeImage()
        {
            ParseReferenceTypeByLoadingLevelParameterizableTest(
                includes => includes
                    .Include<PersonLinkedSource>().AsNestedLinkedSource(
                        typeof(string),
                        reference => (string)reference
                    )
            );
        }

        [Test]
        public void ParseReferenceTypeByLoadingLevel_WithSubPerson_ShouldNotLoadPerson()
        {
            ParseReferenceTypeByLoadingLevelParameterizableTest(
                includes => includes
                    .Include<PersonLinkedSource>().AsSubLinkedSource(
                        typeof (PersonLinkedSource)
                    )
            );
        }

        [Test]
        public void ParseReferenceTypeByLoadingLevel_WithSubAndNestedPerson_NestedPersonShouldWin() {
            ParseReferenceTypeByLoadingLevelParameterizableTest(
                includes => includes
                    .Include<PersonLinkedSource>().AsSubLinkedSource(
                        typeof(PersonLinkedSource)
                    )
                    .Include<PersonLinkedSource>().AsNestedLinkedSource(
                        typeof(string),
                        reference => (string)reference
                    )
            );
        }

        [Test]
        public void ParseReferenceTypeByLoadingLevel_WithNestedPersonAndSub_NestedPersonShouldWin() {
            ParseReferenceTypeByLoadingLevelParameterizableTest(
                includes => includes
                    .Include<PersonLinkedSource>().AsNestedLinkedSource(
                        typeof(string),
                        reference => (string)reference
                    )
                    .Include<PersonLinkedSource>().AsSubLinkedSource(
                        typeof(PersonLinkedSource)
                    )
            );
        }

        public void ParseReferenceTypeByLoadingLevelParameterizableTest(Action<IncludeTargetConcreteTypeBuilder<LinkedSource, object, object, Type>> setupIncludes) {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.PolyTargetLinks,
                    linkedSource => linkedSource.PolyTargets,
                    reference => reference.GetType(),
                    includes => {
                        includes.Include<PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource>().AsNestedLinkedSource(
                            typeof(int),
                            reference => (string)reference
                        );
                        setupIncludes(includes);
                    }
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            var sut = TestSetupHelper.CreateReferenceTypeByLoadingLevelParser(loadLinkProtocolBuilder);

            var actual = sut.ParseReferenceTypeByLoadingLevel(typeof(LinkedSource));

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<object> PolyTargets { get; set; }
        }

        public class Model {
            public string Id { get; set; }
            public List<object> PolyTargetLinks { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
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
        public void ParseReferenceTypeByLoadingLevel_WithNestedPerson()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .IsRoot<string>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.PolyTargetLinks,
                    linkedSource => linkedSource.PolyTargets,
                    reference => reference.GetType(),
                    includes => includes
                        .When<PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource, string>(
                            typeof(int),
                            reference => (string)reference
                        )
                        .When<PersonLinkedSource, string>(
                            typeof(string),
                            reference => (string)reference
                        )
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            var rootLoadLinkExpression = loadLinkProtocolBuilder.GetLoadLinkExpressions()[0];
            var sut = TestSetupHelper.CreateReferenceTypeByLoadingLevelParser(loadLinkProtocolBuilder);

            var actual = sut.ParseReferenceTypeByLoadingLevel(rootLoadLinkExpression);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void ParseReferenceTypeByLoadingLevel_WithSubPerson() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .IsRoot<string>()
                .LinkSubLinkedSource(
                    linkedSource => linkedSource.Model.PolyTargetLinks,
                    linkedSource => linkedSource.PolyTargets,
                    reference => reference.GetType(),
                    includes => includes
                        //stle: keep as nested
                        .WhenSub<PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource>(
                            typeof(PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource)
                        )
                        .WhenSub<PersonLinkedSource>(
                            typeof(PersonLinkedSource)
                        )
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            var rootLoadLinkExpression = loadLinkProtocolBuilder.GetLoadLinkExpressions()[0];
            var sut = TestSetupHelper.CreateReferenceTypeByLoadingLevelParser(loadLinkProtocolBuilder);

            var actual = sut.ParseReferenceTypeByLoadingLevel(rootLoadLinkExpression);

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

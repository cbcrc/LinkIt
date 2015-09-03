using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ConditionalLoadLinkExpressionTests
    {
        private LoadLinkProtocolFactory<WithPolymorphicReference, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<WithPolymorphicReference, string>(
                loadLinkExpressions: new List<ILoadLinkExpression> {
                    new RootLoadLinkExpression<WithPolymorphicReferenceLinkedSource, WithPolymorphicReference, string>(),
                    new ConditionalLoadLinkExpression<WithPolymorphicReferenceLinkedSource, string>(
                        linkedSource => linkedSource.Model.ReferenceTypeDiscriminant,
                        new Dictionary<string, ILoadLinkExpression>{
                            {
                                "person",
                                new ReferenceLoadLinkExpression<WithPolymorphicReferenceLinkedSource, Person, string>(
                                    linkedSource => (string)linkedSource.Model.ReferenceId,
                                    (linkedSource, reference) => linkedSource.Reference = reference
                                )
                            },
                            {
                                "image",
                                new ReferenceLoadLinkExpression<WithPolymorphicReferenceLinkedSource, Image, string>(
                                    linkedSource => (string)linkedSource.Model.ReferenceId,
                                    (linkedSource, reference) => linkedSource.Reference = reference
                                )
                            }
                        }
                    )
                },
                getReferenceIdFunc: reference => reference.Id
            );
        }

        [Test]
        public void LoadLink_PersonPolymorphicReference() {
            var sut = _loadLinkProtocolFactory.Create(
                new WithPolymorphicReference {
                    Id = "1",
                    ReferenceTypeDiscriminant = "person",
                    ReferenceId = "person-a"
                }
            );

            var actual = sut.LoadLink<WithPolymorphicReferenceLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ImagePolymorphicReference() {
            var sut = _loadLinkProtocolFactory.Create(
                new WithPolymorphicReference {
                    Id = "1",
                    ReferenceTypeDiscriminant = "image",
                    ReferenceId = "image-b"
                }
            );

            var actual = sut.LoadLink<WithPolymorphicReferenceLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_NotSupportedPolymorphicReference() {
            var sut = _loadLinkProtocolFactory.Create(
                new WithPolymorphicReference {
                    Id = "1",
                    ReferenceTypeDiscriminant = "not-supported",
                    ReferenceId = "not-supported-c"
                }
            );

            var actual = sut.LoadLink<WithPolymorphicReferenceLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }

    public class WithPolymorphicReferenceLinkedSource : ILinkedSource<WithPolymorphicReference>
    {
        public WithPolymorphicReference Model { get; set; }
        public object Reference { get; set; }
    }
    
    public class WithPolymorphicReference {
        public string Id { get; set; }
        public string ReferenceTypeDiscriminant { get; set; }
        public object ReferenceId { get; set; }
    }
}

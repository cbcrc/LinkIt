using System;
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
        private LoadLinkProtocolFactory<WithConditional, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<WithConditional, string>(
                loadLinkExpressions: new List<ILoadLinkExpression> {
                    new RootLoadLinkExpression<WithConditionalLinkedSource, WithConditional, string>(),
                    new ConditionalLoadLinkExpression<WithConditionalLinkedSource, string>(
                        linkedSource => linkedSource.Model.ReferenceTypeDiscriminant,
                        new Dictionary<string, ILoadLinkExpression>{
                            {
                                "person",
                                new ReferenceLoadLinkExpression<WithConditionalLinkedSource, Person, string>(
                                    linkedSource => (string)linkedSource.Model.ReferenceId,
                                    (linkedSource, reference) => linkedSource.Reference = reference
                                )
                            },
                            {
                                "image",
                                new ReferenceLoadLinkExpression<WithConditionalLinkedSource, Image, string>(
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
                new WithConditional {
                    Id = "1",
                    ReferenceTypeDiscriminant = "person",
                    ReferenceId = "person-a"
                }
            );

            var actual = sut.LoadLink<WithConditionalLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ImagePolymorphicReference() {
            var sut = _loadLinkProtocolFactory.Create(
                new WithConditional {
                    Id = "1",
                    ReferenceTypeDiscriminant = "image",
                    ReferenceId = "image-b"
                }
            );

            var actual = sut.LoadLink<WithConditionalLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_NotSupportedPolymorphicReference() {
            var sut = _loadLinkProtocolFactory.Create(
                new WithConditional {
                    Id = "1",
                    ReferenceTypeDiscriminant = "not-supported",
                    ReferenceId = "not-supported-c"
                }
            );

            TestDelegate act = () => sut.LoadLink<WithConditionalLinkedSource>("1");

            Assert.That(act, Throws
                .InstanceOf<InvalidOperationException>()
                .With.Message.Contains("not-supported").And
                .With.Message.Contains("WithConditionalLinkedSource")
            );
        }
    }

    public class WithConditionalLinkedSource : ILinkedSource<WithConditional>
    {
        public WithConditional Model { get; set; }
        public object Reference { get; set; }
    }
    
    public class WithConditional {
        public string Id { get; set; }
        public string ReferenceTypeDiscriminant { get; set; }
        public object ReferenceId { get; set; }
    }
}

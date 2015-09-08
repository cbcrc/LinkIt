using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Polymorphic;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class NestedPolymorphicReferenceTests {

        private LoadLinkProtocolFactory<WithNestedPolymorphicReference, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp()
        {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<WithNestedPolymorphicReference, string>(
                loadLinkExpressions: new List<ILoadLinkExpression> {
                    new RootLoadLinkExpression<WithNestedPolymorphicReferenceLinkedSource, WithNestedPolymorphicReference, string>(),
                    new PolymorphicNestedLinkedSourcesLoadLinkExpression<WithNestedPolymorphicReferenceLinkedSource, object, object, Type>(
                        linkedSource => linkedSource.Model.PolyIds,
                        linkedSource => linkedSource.Contents, 
                        (linkedSource, childLinkedSource) => linkedSource.Contents = childLinkedSource,
                        link => link.GetType(),
                        new Dictionary<
                            Type, 
                            IPolymorphicNestedLinkedSourceInclude<object, object>
                        >{
                            {
                                typeof(string), 
                                new PolymorphicNestedLinkedSourceInclude<
                                    object,
                                    object,
                                    PersonLinkedSource,
                                    Person,
                                    string
                                >(
                                    link => (string)link
                                )
                            },
                            {
                                typeof(int), 
                                new PolymorphicNestedLinkedSourceInclude<
                                    object,
                                    object,
                                    PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource,
                                    Image,
                                    string
                                >(
                                    link => ((int)link).ToString()
                                )
                            },
                        }
                    ),
                },
                getReferenceIdFunc: reference => reference.Id
            );
        }

        [Test]
        public void LoadLink_NestedPolymorphicReference() {
            var sut = _loadLinkProtocolFactory.Create(
                new WithNestedPolymorphicReference {
                    Id = "1",
                    PolyIds = new List<object>{"p1",32}
                }
            );

            var actual = sut.LoadLink<WithNestedPolymorphicReferenceLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class WithNestedPolymorphicReferenceLinkedSource : ILinkedSource<WithNestedPolymorphicReference> {
            public WithNestedPolymorphicReference Model { get; set; }
            public List<object> Contents { get; set; }
        }

        public class WithNestedPolymorphicReference {
            public string Id { get; set; }
            public List<object> PolyIds { get; set; }
        }

        
    }
}

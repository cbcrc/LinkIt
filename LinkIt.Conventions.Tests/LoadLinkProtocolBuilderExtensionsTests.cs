using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.ConfigBuilders;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.Tests.Shared;
using LinkIt.Conventions.Interfaces;
using NUnit.Framework;

namespace LinkIt.Conventions.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocolBuilderExtensionsTests {
        [Test]
        public void ApplyConventions_ShouldMatchExpectedLinkTargets(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var conventionStub = new ConventionStub();
            
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage), typeof(LinkedSourceWithPerson) },
                new List<ILoadLinkExpressionConvention> { conventionStub }
            );

            Assert.That(
                conventionStub.LinkTargetPropertyNamesWhereConventionApplies,
                Is.EquivalentTo(new[] { "Image", "Person" })
            );
        }

        [Test]
        public void ApplyConventions_ShouldFilterModelOutWhenMatchingLinkTarget() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var conventionStub = new ConventionStub();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage)},
                new List<ILoadLinkExpressionConvention> { conventionStub }
            );

            Assert.That(
                conventionStub.DidAttemptToMatchModelAsLinkTarget,
                Is.False
            );
        }

        [Test]
        public void ApplyConventions_DuplicateConventions_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            TestDelegate act = () => loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage) },
                new List<ILoadLinkExpressionConvention>{
                    new ConventionStub("same-name"),
                    new ConventionStub("same-name")
                }
            );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("with the same name")
                .With.Message.ContainsSubstring("same-name")
            );
        }

        [Test]
        public void ApplyConventions_ParameterizableConventions_ShouldNotThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage) },
                new List<ILoadLinkExpressionConvention>{
                    new ConventionStub("same-name"),
                    new ConventionStub("different-name")
                }
            );
        }


        public class LinkedSourceWithImage : ILinkedSource<Model>{
            public Model Model { get; set; }
            public Image NotImage { get; set; }
            public Image Image { get; set; }
        }

        public class LinkedSourceWithPerson : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Person Person { get; set; }
            public Person NotPerson { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public string ImageId { get; set; }
            public string PersonId { get; set; }
        }

        public class ConventionStub:ISingleValueConvention
        {
            public ConventionStub(string name=null)
            {
                Name = name??"Convention stub";
            }

            public string Name { get; private set; }

            public readonly List<string> LinkTargetPropertyNamesWhereConventionApplies = new List<string>();

            public string Id { get { return "Stub"; } }
            

            public bool DoesApply(
                PropertyInfo linkTargetProperty, 
                PropertyInfo linkedSourceModelProperty)
            {
                if (linkTargetProperty.Name == "Model"){
                    DidAttemptToMatchModelAsLinkTarget = true;
                }

                var matchingName = linkTargetProperty.Name + "Id";
                return matchingName == linkedSourceModelProperty.Name;
            }

            public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
                LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
                Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty)
            {
                LinkTargetPropertyNamesWhereConventionApplies.Add(linkTargetProperty.Name);
            }

            public bool DidAttemptToMatchModelAsLinkTarget { get; private set; }
        }
    }
}

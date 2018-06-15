// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;
using LinkIt.Shared;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Conventions.Tests
{
    public class LoadLinkProtocolBuilderExtensionsTests
    {
        [Fact]
        public void ApplyConventions_DuplicateConventions_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            Action act = () => loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage) },
                new List<ILoadLinkExpressionConvention>
                {
                    new ConventionStub("same-name"),
                    new ConventionStub("same-name")
                }
            );

            var exception = Assert.Throws<LinkItException>(act);
            Assert.Contains("with the same name", exception.Message);
            Assert.Contains("same-name", exception.Message);
        }

        [Fact]
        public void ApplyConventions_ParameterizableConventions_ShouldNotThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage) },
                new List<ILoadLinkExpressionConvention>
                {
                    new ConventionStub("same-name"),
                    new ConventionStub("different-name")
                }
            );
        }

        [Fact]
        public void ApplyConventions_ShouldFilterModelOutWhenMatchingLinkTarget()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var conventionStub = new ConventionStub();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage) },
                new List<ILoadLinkExpressionConvention> { conventionStub }
            );

            Assert.False(conventionStub.DidAttemptToMatchModelAsLinkTarget);
        }

        [Fact]
        public void ApplyConventions_ShouldMatchExpectedLinkTargets()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var conventionStub = new ConventionStub();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage), typeof(LinkedSourceWithPerson) },
                new List<ILoadLinkExpressionConvention> { conventionStub }
            );

            Assert.Equal(
                new[] { "Image", "Person" },
                conventionStub.LinkTargetPropertyNamesWhereConventionApplies);
        }

        public class LinkedSourceWithImage : ILinkedSource<Model>
        {
            public Image NotImage { get; set; }
            public Image Image { get; set; }
            public Model Model { get; set; }
        }

        public class LinkedSourceWithPerson : ILinkedSource<Model>
        {
            public Person Person { get; set; }
            public Person NotPerson { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public string ImageId { get; set; }
            public string PersonId { get; set; }
        }

        public class ConventionStub : IReferenceConvention
        {
            public readonly List<string> LinkTargetPropertyNamesWhereConventionApplies = new List<string>();

            public ConventionStub(string name = null)
            {
                Name = name ?? "Convention stub";
            }

            public string Id => "Stub";

            public bool DidAttemptToMatchModelAsLinkTarget { get; private set; }

            public string Name { get; }

            public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
            {
                if (linkTargetProperty.Name == "Model")
                {
                    DidAttemptToMatchModelAsLinkTarget = true;
                }

                var matchingName = linkTargetProperty.Name + "Id";
                return matchingName == linkedSourceModelProperty.Name;
            }

            public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty, PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
                where TLinkedSource: ILinkedSource
            {
                LinkTargetPropertyNamesWhereConventionApplies.Add(linkTargetProperty.Name);
            }
        }
    }
}
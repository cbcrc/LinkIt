using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Polymorphic;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LinkedSourceConfigTests
    {
        [Test]
        public void CreateNestedLinkedSourceInclude_TLinkedSourceDoesNotImplementTIChildLinkedSource_ShouldThrow()
        {
            TestDelegate act =
                () => LinkedSourceConfigs.GetConfigFor<NestedLinkedSource>()
                    .CreateNestedLinkedSourceInclude<object, IPolymorphicSource, object, object>(
                        link => link
                    );

            Assert.That(
                act, 
                Throws.ArgumentException
                    .With.Message.Contains("NestedLinkedSource").And
                    .With.Message.Contains("IPolymorphicSource")
            );
        }

        [Test]
        public void CreateSubLinkedSourceInclude_TLinkedSourceDoesNotImplementTIChildLinkedSource_ShouldThrow()
        {
            TestDelegate act =
                () => LinkedSourceConfigs.GetConfigFor<NestedLinkedSource>()
                    .CreateSubLinkedSourceInclude<IPolymorphicSource, object, NestedContent>(
                        link => (NestedContent)link
                    );

            Assert.That(
                act,
                Throws.ArgumentException
                    .With.Message.Contains("NestedLinkedSource").And
                    .With.Message.Contains("IPolymorphicSource")
            );
        }
    }
}
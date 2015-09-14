using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [TestFixture]
    public class ReferencesLoadLinkExpressionTests {
        [Test]
        public void LoadLinkReference_WithEnumerableTLinkedSource_ShouldThrow() {
            TestDelegate act = () =>
                new ReferencesLoadLinkExpression<List<object>, object, string>("the-id", null, null);

            Assert.That(act,
                Throws.ArgumentException
                .With.Message.ContainsSubstring("the-id").And
                .With.Message.ContainsSubstring("TLinkedSource")
            );
        }

        [Test]
        public void LoadLinkReference_WithEnumerableTReference_ShouldThrow() {
            TestDelegate act = () => 
                new ReferencesLoadLinkExpression<object,List<object>,string>("the-id",null,null);

            Assert.That(act,
                Throws.ArgumentException
                .With.Message.ContainsSubstring("the-id").And
                .With.Message.ContainsSubstring("TReference")
            );
        }

        [Test]
        public void LoadLinkReference_WithEnumerableTId_ShouldThrow() {
            TestDelegate act = () =>
                new ReferencesLoadLinkExpression<object, object, List<string>>("the-id", null, null);

            Assert.That(act,
                Throws.ArgumentException
                .With.Message.ContainsSubstring("the-id").And
                .With.Message.ContainsSubstring("TId")
            );
        }
    }
}

using System;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSource.Conventions.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocolBuilderExtensionsTests
    {
        [Test]
        public void GetLinkedSourceTypes(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            
            var actual = loadLinkProtocolBuilder.GetLinkedSourceTypes(AppDomain.CurrentDomain.GetAssemblies());

            var doesContainLinkedSource = actual
                .Any(linkedSourceType => linkedSourceType == typeof (LinkedSource));
            Assert.That(doesContainLinkedSource, Is.True);
        }

        [Test]
        public void X()
        {
            var actual = LoadLinkProtocolBuilderExtensions.GetLinkTargetProperties(typeof (LinkedSource));

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void Y() {
            var actual = LoadLinkProtocolBuilderExtensions.GetLinkedSourceModelProperties(typeof(LinkedSource));

            ApprovalsExt.VerifyPublicProperties(actual);
        }


        public class LinkedSource : ILinkedSource<Model>{
            public Model Model { get; set; }
            public Image Image { get; set; }
            public Image NotImage { get; set; }
            public Person Person { get; set; }
            public Person PersonId { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public string ImageId { get; set; }
            public string PersonId { get; set; }
        }
    }
}

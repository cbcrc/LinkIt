using LinkIt.PublicApi;
using LinkIt.TestHelpers;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PersonWithoutContextualizationLinkedSource : IPolymorphicSource, ILinkedSource<Person>
    {
        public Person Model { get; set; }
    }
}
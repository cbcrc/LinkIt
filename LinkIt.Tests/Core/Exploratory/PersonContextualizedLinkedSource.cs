using LinkIt.PublicApi;
using LinkIt.TestHelpers;

namespace LinkIt.Tests.Core.Exploratory
{
    public class PersonContextualizedLinkedSource: ILinkedSource<Person>
    {
        public Person Model { get; set; }
        public PersonContextualization Contextualization { get; set; }
        public Image SummaryImage { get; set; }
    }
}
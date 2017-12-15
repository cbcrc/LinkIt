using LinkIt.PublicApi;

namespace LinkIt.TestHelpers
{
    public class PersonLinkedSource : ILinkedSource<Person>
    {
        public Image SummaryImage { get; set; }
        public Person Model { get; set; }
    }
}
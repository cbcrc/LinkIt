using LinkIt.PublicApi;

namespace LinkIt.TestHelpers
{
    public class NestedLinkedSource : ILinkedSource<NestedContent>
    {
        public PersonLinkedSource AuthorDetail { get; set; }
        public Person ClientSummary { get; set; }
        public NestedContent Model { get; set; }
    }
}

using LinkIt.PublicApi;

namespace LinkIt.Tests.Core.Exploratory
{
    public class WithContextualizedReferenceLinkedSource : ILinkedSource<WithContextualizedReference>
    {
        public WithContextualizedReference Model { get; set; }
        public PersonContextualizedLinkedSource Person { get; set; }
    }
}
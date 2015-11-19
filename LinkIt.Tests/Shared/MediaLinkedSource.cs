using LinkIt.PublicApi;

namespace LinkIt.Tests.Shared
{
    public class MediaLinkedSource : ILinkedSource<Media> {
        public Media Model { get; set; }
        public Image SummaryImage { get; set; }
    }
}
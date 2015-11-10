using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.Tests.Shared;

namespace HeterogeneousDataSources.Tests
{
    public class MediaLinkedSource : ILinkedSource<Media> {
        public Media Model { get; set; }
        public Image SummaryImage { get; set; }
    }
}
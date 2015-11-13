using HeterogeneousDataSources.Tests.Shared;
using LinkIt.LinkedSources.Interfaces;

namespace HeterogeneousDataSources.Tests
{
    public class MediaLinkedSource : ILinkedSource<Media> {
        public Media Model { get; set; }
        public Image SummaryImage { get; set; }
    }
}
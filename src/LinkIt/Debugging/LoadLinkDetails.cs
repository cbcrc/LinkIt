using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Debugging
{

    internal class LoadLinkDetails<TLinkedSource, TLinkedSourceModel> : ILoadLinkDetails
        where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>
    {
        private readonly Stopwatch _stopwatch;

        private int _currentStepIndex;

        public LoadLinkDetails(LoadLinkCallDetails callDetails, IReadOnlyList<IReadOnlyList<Type>> referenceTypesToBeLoadedForEachLoadingLevel)
        {
            CallDetails = callDetails;

            Steps = GetSteps(referenceTypesToBeLoadedForEachLoadingLevel);
            CurrentStep = Steps[0];

            _stopwatch = Stopwatch.StartNew();
        }

        private static IReadOnlyList<LoadLinkStepDetails> GetSteps(IReadOnlyList<IReadOnlyList<Type>> referenceTypesToBeLoadedForEachLoadingLevel)
        {
            return referenceTypesToBeLoadedForEachLoadingLevel
                .Select((referenceTypes, index) => new LoadLinkStepDetails(referenceTypes, index + 1))
                .ToImmutableList();
        }

        public Type LinkedSourceType => typeof(TLinkedSource);

        public Type LinkedSourceModelType => typeof(TLinkedSourceModel);

        public LoadLinkCallDetails CallDetails { get; }

        public IReadOnlyList<object> Result { get; private set; } = ImmutableList<object>.Empty;

        public IReadOnlyList<LoadLinkStepDetails> Steps { get; }

        public LoadLinkStepDetails CurrentStep { get; private set; }

        public TimeSpan? Took { get; private set; }

        internal void NextStep()
        {
            ++_currentStepIndex;
            CurrentStep = Steps[_currentStepIndex];
        }

        internal void SetResult(IEnumerable<object> linkedSources)
        {
            Result = linkedSources.ToImmutableList();
        }

        internal void LoadLinkEnd()
        {
            _stopwatch.Stop();
            Took = _stopwatch.Elapsed;
        }
    }
}

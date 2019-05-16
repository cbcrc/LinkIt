// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Diagnostics
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
                .ToList();
        }

        public Type LinkedSourceType => typeof(TLinkedSource);

        public Type LinkedSourceModelType => typeof(TLinkedSourceModel);

        public LoadLinkCallDetails CallDetails { get; }

        public IReadOnlyList<object> Result { get; private set; } = new List<object>();

        public IReadOnlyList<LoadLinkStepDetails> Steps { get; }

        public LoadLinkStepDetails CurrentStep { get; private set; }

        public TimeSpan? Took { get; private set; }

        public TimeSpan LoadTook => TimeSpan.FromTicks(Steps.Sum(step => step.LoadTook.GetValueOrDefault().Ticks));

        public TimeSpan LinkTook => TimeSpan.FromTicks(Steps.Sum(step => step.LinkTook.GetValueOrDefault().Ticks));

        internal void NextStep()
        {
            ++_currentStepIndex;
            CurrentStep = Steps[_currentStepIndex];
        }

        internal void SetResult(IEnumerable<object> linkedSources)
        {
            Result = linkedSources.ToList();
        }

        internal void LoadLinkEnd()
        {
            _stopwatch.Stop();
            Took = _stopwatch.Elapsed;
        }
    }
}

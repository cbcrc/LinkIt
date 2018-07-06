using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace LinkIt.Debugging
{

    /// <summary>
    /// Provides information on a load-link step.
    /// </summary>
    public class LoadLinkStepDetails
    {
        private readonly ImmutableDictionary<Type, ReferenceLoadDetails> _references;
        private Stopwatch _stopwatch;

        internal LoadLinkStepDetails(IReadOnlyList<Type> referenceTypes, int stepNumber)
        {
            StepNumber = stepNumber;
            _references = referenceTypes
                .ToImmutableDictionary(
                    type => type,
                    type => new ReferenceLoadDetails(type)
                );
        }

        /// <summary>
        /// Load-link step number.
        /// </summary>
        public int StepNumber { get; }

        /// <summary>
        /// Information on the references loaded or to load in this step.
        /// </summary>
        public IReadOnlyList<ReferenceLoadDetails> References => _references.Values.ToImmutableList();

        /// <summary>
        /// Time taken to load all references.
        /// </summary>
        public TimeSpan? LoadTook { get; private set; }

        /// <summary>
        /// Time taken to perform linking in the linked sources.
        /// </summary>
        public TimeSpan? LinkTook { get; private set; }

        internal void LoadStart()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        internal void LoadEnd()
        {
            _stopwatch.Stop();
            LoadTook = _stopwatch.Elapsed;
        }

        internal void LinkStart()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        internal void LinkEnd()
        {
            _stopwatch.Stop();
            LinkTook = _stopwatch.Elapsed;
        }

        internal void SetReferenceIds(ImmutableDictionary<Type, IReadOnlyList<object>> referenceIds)
        {
            foreach (var referenceLoadDetails in _references.Values)
            {
                referenceLoadDetails.SetIds(referenceIds.GetValueOrDefault(referenceLoadDetails.Type));
            }
        }

        internal void AddReferenceValues<TReference>(IEnumerable<TReference> references)
        {
            var referenceLoadDetails = _references.GetValueOrDefault(typeof(TReference));
            referenceLoadDetails?.AddValues(references.Cast<object>());
        }
    }
}

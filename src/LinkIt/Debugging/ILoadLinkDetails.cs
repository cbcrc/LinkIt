using System;
using System.Collections.Generic;

namespace LinkIt.Debugging
{
    /// <summary>
    /// Provides information to aid in debugging load-link operations.
    /// </summary>
    public interface ILoadLinkDetails
    {
        /// <summary>
        /// Type of the linked source(s) loaded.
        /// </summary>
        Type LinkedSourceType { get; }

        /// <summary>
        /// Type of the model of the linked source(s).
        /// </summary>
        Type LinkedSourceModelType { get; }

        /// <summary>
        /// Details on the call to initiate the load-link operation.
        /// </summary>
        LoadLinkCallDetails CallDetails { get; }

        /// <summary>
        /// Final or partial result of the load-link operation.
        /// </summary>
        IReadOnlyList<object> Result { get; }

        /// <summary>
        /// Steps of loading and linking.
        /// </summary>
        IReadOnlyList<LoadLinkStepDetails> Steps { get; }

        /// <summary>
        /// Current step of loading and linking.
        /// </summary>
        LoadLinkStepDetails CurrentStep { get; }

        /// <summary>
        /// Time taken to load-link the result.
        /// </summary>
        TimeSpan? Took { get; }
    }
}

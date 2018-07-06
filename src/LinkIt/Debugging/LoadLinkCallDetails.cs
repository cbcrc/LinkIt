using System.Collections.Generic;
using System.Collections.Immutable;

namespace LinkIt.Debugging
{

    /// <summary>
    /// Details on the call to initiate the load-link operation.
    /// </summary>
    public class LoadLinkCallDetails
    {
        internal LoadLinkCallDetails(string method, IEnumerable<object> values)
        {
            Method = method;
            Values = values.ToImmutableList();
        }

        /// <summary>
        /// Method used to start the load-link.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Values passed to initiate the load-link: models or IDs.
        /// </summary>
        public IReadOnlyList<object> Values { get; }
    }
}

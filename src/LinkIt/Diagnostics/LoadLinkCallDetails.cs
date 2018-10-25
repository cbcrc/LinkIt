// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Diagnostics
{

    /// <summary>
    /// Details on the call to initiate the load-link operation.
    /// </summary>
    public class LoadLinkCallDetails
    {
        internal LoadLinkCallDetails(string method, IEnumerable values)
        {
            Method = method;
            Values = values.Cast<object>().ToList();
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

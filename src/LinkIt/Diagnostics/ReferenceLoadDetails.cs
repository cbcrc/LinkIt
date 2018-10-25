// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Diagnostics
{

    /// <summary>
    /// Provides information on references, of a specific type, loaded or to load in a load-link step.
    /// </summary>
    public class ReferenceLoadDetails
    {
        private readonly List<object> _values = new List<object>();

        internal ReferenceLoadDetails(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Type of the references loaded or to load.
        /// </summary>
        public Type Type { get; }


        /// <summary>
        /// IDs of the references loaded or to load.
        /// </summary>
        public IReadOnlyList<object> Ids { get; private set; } = new object[0];

        /// <summary>
        /// References loaded.
        /// </summary>
        public IReadOnlyList<object> Values => _values.ToList();

        internal void SetIds(IReadOnlyList<object> ids)
        {
            if (ids != null)
            {
                Ids = ids.ToList();
            }
        }

        internal void AddValues(IEnumerable<object> values)
        {
            _values.AddRange(values);
        }
    }
}

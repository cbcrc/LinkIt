using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LinkIt.Debugging
{

    /// <summary>
    /// Provides information on references, of a specific type, loaded or to load in a load-link step.
    /// </summary>
    public class ReferenceLoadDetails
    {
        private ImmutableList<object> _values = ImmutableList<object>.Empty;

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
        public IReadOnlyList<object> Values => _values;

        internal void SetIds(IReadOnlyList<object> ids)
        {
            if (ids != null)
            {
                Ids = ids;
            }
        }

        internal void AddValues(IEnumerable<object> values)
        {
            _values = _values.AddRange(values);
        }
    }
}

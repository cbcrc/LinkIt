using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    internal static class ListExtensions {
        internal static List<T> DistinctWithoutNull<T>(this List<T> list) {
            return list
                .Where(item => item != null)
                .Distinct()
                .ToList();
        }

    }
}

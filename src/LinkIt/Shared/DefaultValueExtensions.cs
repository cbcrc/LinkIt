using System.Collections.Generic;

namespace LinkIt.Shared
{
    internal static class DefaultValueExtensions
    {
        public static bool EqualsDefaultValue<T>(this T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default);
        }
    }
}

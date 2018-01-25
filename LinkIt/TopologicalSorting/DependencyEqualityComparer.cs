using System.Collections.Generic;

namespace LinkIt.TopologicalSorting
{
    internal class DependencyEqualityComparer : IEqualityComparer<Dependency>
    {
        public bool Equals(Dependency x, Dependency y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Equals(x.Graph, y.Graph) && x.LinkedSourceType == y.LinkedSourceType && x.Type == y.Type;
        }

        public int GetHashCode(Dependency obj)
        {
            unchecked
            {
                var hashCode = (obj.Graph != null ? obj.Graph.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.LinkedSourceType != null ? obj.LinkedSourceType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Type != null ? obj.Type.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
using System.Collections.Generic;

namespace HeterogeneousDataSources.Tests
{
    public interface IReferenceLoader
    {
        List<object> LoadReferences(List<object> ids);
    }
}
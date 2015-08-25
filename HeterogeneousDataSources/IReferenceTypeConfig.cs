using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface IReferenceTypeConfig
    {
        void LoadReferences(object linkedSource, List<ILoadLinkExpression> loadExpressions, LoadedReferenceContext loadedReferenceContext);
    }
}
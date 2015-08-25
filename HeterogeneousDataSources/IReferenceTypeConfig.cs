using System;
using System.Collections.Generic;
using System.Dynamic;

namespace HeterogeneousDataSources
{
    public interface IReferenceTypeConfig
    {
        void LoadReferences(object linkedSource, List<ILoadLinkExpression> loadExpressions, LoadedReferenceContext loadedReferenceContext);
    }
}
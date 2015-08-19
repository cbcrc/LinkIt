using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadLinkExpression
    {
        List<object> GetLookupIds(object linkedSource);
        Type ReferenceType { get; }
        void Link(object linkedSource, DataContext dataContext);
        object GetReferenceId(object reference);
    }
}
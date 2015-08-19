using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadLinkExpression<TId>
    {
        List<TId> GetLookupIds(object linkedSource);
    }
}
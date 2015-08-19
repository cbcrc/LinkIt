using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadExpression<TLinkedSource, TId>
    {
        List<TId> GetLookupIds(TLinkedSource linkedSource);
    }
}
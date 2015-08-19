using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadExpression<TLinkedSource>
    {
        List<object> GetLookupIds(TLinkedSource linkedSource);
    }
}
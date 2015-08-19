using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadExpression
    {
        List<object> GetLookupIds(object linkedSource);
        Type ReferenceType { get; }
        object GetReferenceId(object reference);
    }

    public interface ILinkExpression<TLinkedSource> {
        void Link(TLinkedSource linkedSource, DataContext dataContext);
    }
}
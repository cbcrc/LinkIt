using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadLinkExpression {
        List<object> ReferenceIds { get; }
        Type ReferenceType { get; }
        void Link(DataContext dataContext);
        object GetReferenceId(object reference);
    }
}
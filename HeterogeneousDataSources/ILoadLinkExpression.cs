using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadLinkExpression {
        List<object> ReferenceIds { get; }
        Type ReferenceType { get; }
    }
}
using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface INestedLoadLinkExpression : ILoadLinkExpression
    {
        List<Type> ChildLinkedSourceTypes { get; }
    }
}
using System;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface INestedLoadLinkExpression : ILoadLinkExpression
    {
        Type ChildLinkedSourceType { get; }
    }
}
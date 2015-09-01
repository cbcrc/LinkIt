using System;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface IRootLoadLinkExpression : ILoadLinkExpression
    {
        Type RootLinkedSourceType { get; }
    }
}
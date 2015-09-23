using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithChildLinkedSource:IInclude {
        Type ChildLinkedSourceType { get; }
    }
}
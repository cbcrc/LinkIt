using System;

namespace LinkIt.LoadLinkExpressions.Includes.Interfaces
{
    public interface IIncludeWithChildLinkedSource:IInclude {
        Type ChildLinkedSourceType { get; }
    }
}
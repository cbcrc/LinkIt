using System;

namespace LinkIt.Core.Includes.Interfaces
{
    public interface IIncludeWithChildLinkedSource:IInclude {
        Type ChildLinkedSourceType { get; }
    }
}
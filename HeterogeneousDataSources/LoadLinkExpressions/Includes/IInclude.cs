using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IInclude
    {
        //stle: is this really common? sub linked source has no reference concept
        Type ReferenceType { get; }

        Type ChildLinkedSourceType { get; }
    }
}
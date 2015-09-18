using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public interface IPolymorphicInclude
    {
        //stle: is this really common? sub linked source has no reference concept
        Type ReferenceType { get; }
        Type ChildLinkedSourceType { get; }
    }
}
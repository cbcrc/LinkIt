namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public interface IPolymorphicSubLinkedSourceInclude<TIChildLinkedSource> : IPolymorphicInclude 
    {
        TIChildLinkedSource CreateSubLinkedSource(object iChildLinkedSourceModel, LoadedReferenceContext loadedReferenceContext);
    }
}
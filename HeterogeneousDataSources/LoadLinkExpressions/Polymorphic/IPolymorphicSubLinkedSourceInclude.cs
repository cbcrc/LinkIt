namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public interface IPolymorphicSubLinkedSourceInclude<TIChildLinkedSource, TIChildLinkedSourceModel> : IPolymorphicInclude 
    {
        TIChildLinkedSource CreateSubLinkedSource(TIChildLinkedSourceModel iChildLinkedSourceModel, LoadedReferenceContext loadedReferenceContext);
    }
}
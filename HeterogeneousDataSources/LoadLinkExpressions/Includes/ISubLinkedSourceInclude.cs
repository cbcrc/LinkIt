namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface ISubLinkedSourceInclude<TIChildLinkedSource> : IInclude 
    {
        TIChildLinkedSource CreateSubLinkedSource(
            object iChildLinkedSourceModel, 
            LoadedReferenceContext loadedReferenceContext
        );
    }
}
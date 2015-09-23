namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IWithCreateSubLinkedSource<TIChildLinkedSource>
    {
        TIChildLinkedSource CreateSubLinkedSource(object childLinkedSourceModel, LoadedReferenceContext loadedReferenceContext);
    }
}
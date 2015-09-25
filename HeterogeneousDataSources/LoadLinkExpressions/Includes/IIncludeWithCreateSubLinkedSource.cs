namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithCreateSubLinkedSource<TIChildLinkedSource,TLink>:IInclude
    {
        TIChildLinkedSource CreateSubLinkedSource(TLink link, LoadedReferenceContext loadedReferenceContext);
    }
}
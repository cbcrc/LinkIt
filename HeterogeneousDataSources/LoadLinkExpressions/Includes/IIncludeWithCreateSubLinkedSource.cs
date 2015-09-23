namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithCreateSubLinkedSource<TIChildLinkedSource>:IInclude
    {
        TIChildLinkedSource CreateSubLinkedSource(object childLinkedSourceModel, LoadedReferenceContext loadedReferenceContext);
    }
}
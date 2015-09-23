namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithGetReference<TIReference, TLink>
    {
        TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext);
    }
}
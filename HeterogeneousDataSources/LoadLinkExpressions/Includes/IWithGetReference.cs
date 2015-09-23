namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IWithGetReference<TIReference, TLink>
    {
        TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext);
    }
}
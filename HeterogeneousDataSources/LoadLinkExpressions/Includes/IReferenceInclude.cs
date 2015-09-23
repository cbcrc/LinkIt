namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IReferenceInclude<TIReference, TLink> : IInclude, IWithAddLookupId<TLink>
    {
        TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext);
    }
}
namespace HeterogeneousDataSources
{
    public interface IReferenceLoader
    {
        void LoadReferences(
            LookupIdContext lookupIdContext, 
            LoadedReferenceContext loadedReferenceContext
        );
    }
}
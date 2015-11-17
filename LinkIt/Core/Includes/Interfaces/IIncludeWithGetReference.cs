namespace LinkIt.Core.Includes.Interfaces
{
    public interface IIncludeWithGetReference<TIReference, TLink>
    {
        TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext);
    }
}
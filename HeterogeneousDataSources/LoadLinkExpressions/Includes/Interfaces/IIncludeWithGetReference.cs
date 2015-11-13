using LinkIt.Protocols;

namespace LinkIt.LoadLinkExpressions.Includes.Interfaces
{
    public interface IIncludeWithGetReference<TIReference, TLink>
    {
        TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext);
    }
}
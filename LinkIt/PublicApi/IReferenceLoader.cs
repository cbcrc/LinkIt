using System;

namespace LinkIt.PublicApi
{
    public interface IReferenceLoader: IDisposable
    {
        void LoadReferences(
            ILookupIdContext lookupIdContext, 
            ILoadedReferenceContext loadedReferenceContext
        );
    }
}
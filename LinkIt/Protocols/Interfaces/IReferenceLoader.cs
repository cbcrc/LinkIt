using System;

namespace LinkIt.Protocols.Interfaces
{
    public interface IReferenceLoader: IDisposable
    {
        void LoadReferences(
            LookupIdContext lookupIdContext, 
            LoadedReferenceContext loadedReferenceContext
        );
    }
}
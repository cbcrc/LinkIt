using System;

namespace LinkIt.PublicApi
{
    //Responsible for loading references of a loading level.
    public interface IReferenceLoader: IDisposable
    {
        void LoadReferences(
            ILookupIdContext lookupIdContext, 
            ILoadedReferenceContext loadedReferenceContext
        );
    }
}
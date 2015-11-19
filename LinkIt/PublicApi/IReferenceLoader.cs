using System;

namespace LinkIt.PublicApi
{
    //Responsible for loading references of a loading level.
    public interface IReferenceLoader
        //Dispose will always be invoked as soon as the load phase is completed or
        //if an exception is thrown
        : IDisposable
    {
        void LoadReferences(
            ILookupIdContext lookupIdContext, 
            ILoadedReferenceContext loadedReferenceContext
        );
    }
}
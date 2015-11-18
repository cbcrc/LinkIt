using System;
using System.Collections.Generic;

namespace LinkIt.PublicApi
{
    //Responsible for gathering all loaded references of a root linked source.
    public interface ILoadedReferenceContext
    {
        void AddReferences<TReference, TId>(List<TReference> references, Func<TReference,TId> getReferenceId);
        void AddReferences<TReference, TId>(IDictionary<TId,TReference> referencesById);
    }
}
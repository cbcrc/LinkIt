using System;
using System.Collections.Generic;

namespace LinkIt.PublicApi
{
    public interface ILookupIdContext
    {
        void AddSingle<TReference, TId>(TId lookupId);
        void AddMulti<TReference, TId>(List<TId> lookupIds);
        List<Type> GetReferenceTypes();
        List<TId> GetReferenceIds<TReference, TId>();
    }
}
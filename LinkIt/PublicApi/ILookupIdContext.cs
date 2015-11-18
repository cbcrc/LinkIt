using System;
using System.Collections.Generic;

namespace LinkIt.PublicApi
{
    //Responsible for giving access to the lookup ids of a loading level.
    public interface ILookupIdContext
    {
        List<Type> GetReferenceTypes();
        List<TId> GetReferenceIds<TReference, TId>();
    }
}
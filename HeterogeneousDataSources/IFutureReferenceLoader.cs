using System;

namespace HeterogeneousDataSources
{
    public class FutureReferenceLoader<TReference>
    {
        public FutureReferenceLoader(Func<TReference, object> getReferenceIdFunc)
        {
            GetReferenceIdFunc = getReferenceIdFunc;
        }

        public Func<TReference, object> GetReferenceIdFunc { get; private set; }
    }
}
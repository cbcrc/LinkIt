using System;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    internal class Loader
    {
        private Func<IReferenceLoader> createReferenceLoader;

        public Loader(Func<IReferenceLoader> createReferenceLoader)
        {
            this.createReferenceLoader = createReferenceLoader;
        }
    }
}
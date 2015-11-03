using System;

namespace HeterogeneousDataSources
{
    public class ReferenceToLoad{
        public ReferenceToLoad(Type referenceType, string linkTargetId)
        {
            ReferenceType = referenceType;
            LinkTargetId = linkTargetId;
        }

        public Type ReferenceType { get; private set; }
        public string LinkTargetId { get; private set; }
    }
}
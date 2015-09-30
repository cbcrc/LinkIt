using System;

namespace HeterogeneousDataSources
{
    public interface ILinkedSourceExpression<TLinkedSource>
    {
        TLinkedSource CreateLinkedSource(object model, LoadedReferenceContext loadedReferenceContext);

        TLinkedSource LoadLinkModel(
            object modelId, 
            LoadedReferenceContext loadedReferenceContext,
            IReferenceLoader referenceLoader
        );
    }
}
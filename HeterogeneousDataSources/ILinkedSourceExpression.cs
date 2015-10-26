using System;

namespace HeterogeneousDataSources
{
    public interface ILinkedSourceExpression<TLinkedSource,TId>
    {
        TLinkedSource CreateLinkedSource(object model, LoadedReferenceContext loadedReferenceContext);

        TLinkedSource LoadLinkModel(
            TId modelId, 
            LoadedReferenceContext loadedReferenceContext,
            IReferenceLoader referenceLoader
        );
    }
}
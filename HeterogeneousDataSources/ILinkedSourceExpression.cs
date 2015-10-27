using System;

namespace HeterogeneousDataSources
{
    //stle: delete this
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
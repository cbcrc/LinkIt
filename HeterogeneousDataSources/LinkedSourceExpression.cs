using System;
using System.Collections.Generic;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{
    public class LinkedSourceExpression<TLinkedSource, TLinkedSourceModel, TId> 
        : ILinkedSourceExpression<TLinkedSource,TId>
        where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new() 
    {
        public LinkedSourceExpression()
        {
            LinkedSourceType = typeof (TLinkedSource);
            ReferenceType = typeof (TLinkedSourceModel);
            ReferenceTypes = new List<Type>{
                ReferenceType
            };
        }

        //stle: interface segragation
        public Type LinkedSourceType { get; private set; }
        public Type ReferenceType { get; private set; }
        public List<Type> ReferenceTypes { get; private set; }

        //Is LoadLinkExpressionUtil still required?
        public TLinkedSource CreateLinkedSource(object model, LoadedReferenceContext loadedReferenceContext)
        {
            //stle: better error for invalid cast

            return LoadLinkExpressionUtil.CreateLinkedSource<TLinkedSource, TLinkedSourceModel>(
                (TLinkedSourceModel) model,
                loadedReferenceContext
            );
        }

        public TLinkedSource LoadLinkModel(
            TId modelId, 
            LoadedReferenceContext loadedReferenceContext,
            IReferenceLoader referenceLoader)
        {
            var lookupIdContext = new LookupIdContext();
            lookupIdContext.AddSingle<TLinkedSourceModel,TId>(modelId);

            referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);
            var model = loadedReferenceContext.GetOptionalReference<TLinkedSourceModel, TId>(modelId);

            return CreateLinkedSource(model, loadedReferenceContext);
        }


    }
}
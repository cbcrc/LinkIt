using System;
using System.Collections.Generic;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{
    public class LinkedSourceExpression<TLinkedSource, TLinkedSourceModel> 
        : ILinkedSourceExpression<TLinkedSource>
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
            object modelId, 
            LoadedReferenceContext loadedReferenceContext,
            IReferenceLoader referenceLoader)
        {
            //stle: TId is problematic, remove it (what about int?) or get it by reflection
            var modelIdAsString = (string) modelId;

            var lookupIdContext = new LookupIdContext();
            lookupIdContext.AddSingle<TLinkedSourceModel,string>(modelIdAsString);

            referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);
            var model = loadedReferenceContext.GetOptionalReference<TLinkedSourceModel, string>(modelIdAsString);

            return CreateLinkedSource(model, loadedReferenceContext);
        }


    }
}
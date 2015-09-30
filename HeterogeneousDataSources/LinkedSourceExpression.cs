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
            ReferenceTypes = new List<Type>{
                typeof (TLinkedSourceModel)
            };
        }

        //stle: interface segragation
        public Type LinkedSourceType { get; private set; }
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
    }
}
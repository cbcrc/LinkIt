using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class RootLoadLinkExpression<TNestedLinkedSource, TNestedLinkedSourceModel, TId>
        : LoadLinkExpression<TId, TNestedLinkedSourceModel, TId>, ILoadLinkExpression
        where TNestedLinkedSource : class, ILinkedSource<TNestedLinkedSourceModel>, new() 
    {
        protected override List<TId> GetLookupIdsTemplate(TId id)
        {
            //stle: hey you and your inheritance crap! Try a functional approach
            return new List<TId>{id};
        }

        //stle: hey you and your inheritance crap! Try a functional approach
        protected override void LinkAction(TId id, List<TNestedLinkedSourceModel> references, LoadedReferenceContext loadedReferenceContext)
        {
            var nestedLinkedSource = LoadLinkExpressionUtil.CreateNestedLinkedSource<TNestedLinkedSource, TNestedLinkedSourceModel>(references);
            loadedReferenceContext.AddLinkedSourceToBeBuilt(nestedLinkedSource);
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.NestedLinkedSource; }
        }

    }
}

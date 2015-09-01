using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class RootLoadLinkExpression<TRootLinkedSource, TRootLinkedSourceModel, TId>
        : LoadLinkExpression<TId, TRootLinkedSourceModel, TId>, IRootLoadLinkExpression
        where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new() 
    {
        public RootLoadLinkExpression()
        {
            RootLinkedSourceType = typeof(TRootLinkedSource);
        }

        protected override List<TId> GetLookupIdsTemplate(TId id)
        {
            //stle: hey you and your inheritance crap! Try a functional approach
            return new List<TId>{id};
        }

        //stle: hey you and your inheritance crap! Try a functional approach
        protected override void LinkAction(TId id, List<TRootLinkedSourceModel> references, LoadedReferenceContext loadedReferenceContext)
        {
            var nestedLinkedSource = LoadLinkExpressionUtil.CreateLinkedSources<TRootLinkedSource, TRootLinkedSourceModel>(references);
            loadedReferenceContext.AddLinkedSourcesToBeBuilt(nestedLinkedSource);
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.Root; }
        }

        public Type RootLinkedSourceType { get; private set; }
    }
}

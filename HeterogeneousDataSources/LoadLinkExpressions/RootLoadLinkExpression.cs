using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class RootLoadLinkExpression<TChildLinkedSource, TChildLinkedSourceModel, TId>
        : LoadLinkExpression<TId, TChildLinkedSourceModel, TId>, INestedLoadLinkExpression
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
    {
        public RootLoadLinkExpression()
        {
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        protected override List<TId> GetLookupIdsTemplate(TId id)
        {
            //stle: hey you and your inheritance crap! Try a functional approach
            return new List<TId>{id};
        }

        //stle: hey you and your inheritance crap! Try a functional approach
        protected override void LinkAction(TId id, List<TChildLinkedSourceModel> references, LoadedReferenceContext loadedReferenceContext)
        {
            var nestedLinkedSource = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(references);
            loadedReferenceContext.AddLinkedSourcesToBeBuilt(nestedLinkedSource);
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.Root; }
        }

        public Type ChildLinkedSourceType { get; private set; }
    }
}

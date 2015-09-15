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
            ChildLinkedSourceModelType = typeof(TChildLinkedSourceModel);
            ModelType = typeof (TChildLinkedSourceModel);
        }

        protected override List<TId> GetLookupIdsTemplate(TId id)
        {
            //stle: hey you and your inheritance crap! Try a functional approach
            return new List<TId>{id};
        }

        //stle: hey you and your inheritance crap! Try a functional approach
        protected override void LinkAction(TId id, List<TChildLinkedSourceModel> references, LoadedReferenceContext loadedReferenceContext)
        {
            LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(references, loadedReferenceContext);
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.Root; }
        }

        public Type ChildLinkedSourceType { get; private set; }
        public Type ChildLinkedSourceModelType { get; private set; }
        public Type ModelType { get; private set; }
        public List<Type> ChildLinkedSourceTypes { get { return new List<Type> { ChildLinkedSourceType }; } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class NestedLinkedSourceLoadLinkExpression<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel, TId>
        : LoadLinkExpression<TLinkedSource, TChildLinkedSourceModel, TId>, INestedLoadLinkExpression
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
    {
        private readonly Func<TLinkedSource, TId> _getLookupIdFunc;
        private readonly Action<TLinkedSource, TChildLinkedSource> _linkAction;

        public NestedLinkedSourceLoadLinkExpression(Func<TLinkedSource, TId> getLookupIdFunc, Action<TLinkedSource, TChildLinkedSource> linkAction)
        {
            ChildLinkedSourceType = typeof (TChildLinkedSource);
            ChildLinkedSourceModelType = typeof (TChildLinkedSourceModel);
            _getLookupIdFunc = getLookupIdFunc;
            _linkAction = linkAction;
            ModelType = typeof(TChildLinkedSourceModel);
        }

        protected override List<TId> GetLookupIdsTemplate(TLinkedSource linkedSource)
        {
            //stle: hey you and your inheritance crap! Try a functional approach
            return new List<TId>{
                _getLookupIdFunc(linkedSource)
            };
        }

        protected override void LinkAction(TLinkedSource linkedSource, List<TChildLinkedSourceModel> references, LoadedReferenceContext loadedReferenceContext)
        {
            var nestedLinkedSource = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(references, loadedReferenceContext);
            _linkAction(linkedSource, nestedLinkedSource.SingleOrDefault());
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.NestedLinkedSource; }
        }

        public Type ChildLinkedSourceType { get; private set; }
        public Type ChildLinkedSourceModelType { get; private set; }

        public Type ModelType { get; private set; }
        public List<Type> ChildLinkedSourceTypes { get{return new List<Type>{ChildLinkedSourceType};} }
    }
}

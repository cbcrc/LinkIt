using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class NestedLinkedSourcesLoadLinkExpression<TLinkedSource, TNestedLinkedSource, TNestedLinkedSourceModel, TId>
        : LoadLinkExpression<TLinkedSource, TNestedLinkedSourceModel, TId>, ILoadLinkExpression
        where TNestedLinkedSource : class, ILinkedSource<TNestedLinkedSourceModel>, new() 
    {
        private readonly Func<TLinkedSource, List<TId>> _getLookupIdsFunc;
        private readonly Action<TLinkedSource, List<TNestedLinkedSource>> _linkAction;

        public NestedLinkedSourcesLoadLinkExpression(Func<TLinkedSource, List<TId>> getLookupIdsFunc, Action<TLinkedSource, List<TNestedLinkedSource>> linkAction)
        {
            _getLookupIdsFunc = getLookupIdsFunc;
            _linkAction = linkAction;
        }

        protected override List<TId> GetLookupIdsTemplate(TLinkedSource linkedSource)
        {
            return _getLookupIdsFunc(linkedSource);
        }

        protected override void LinkAction(TLinkedSource linkedSource, List<TNestedLinkedSourceModel> references, LoadedReferenceContext loadedReferenceContext)
        {
            var nestedLinkedSource = LoadLinkExpressionUtil.CreateLinkedSources<TNestedLinkedSource, TNestedLinkedSourceModel>(references);
            loadedReferenceContext.AddLinkedSourcesToBeBuilt(nestedLinkedSource);
            _linkAction(linkedSource, nestedLinkedSource);
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.NestedLinkedSource; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class NestedLinkedSourcesLoadLinkExpression<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel, TId>
        : LoadLinkExpression<TLinkedSource, TChildLinkedSourceModel, TId>, INestedLoadLinkExpression
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
    {
        private readonly Func<TLinkedSource, List<TId>> _getLookupIdsFunc;
        private readonly Action<TLinkedSource, List<TChildLinkedSource>> _linkAction;

        public NestedLinkedSourcesLoadLinkExpression(Func<TLinkedSource, List<TId>> getLookupIdsFunc, Action<TLinkedSource, List<TChildLinkedSource>> linkAction)
        {
            ChildLinkedSourceType = typeof (TChildLinkedSource);
            ChildLinkedSourceModelType = typeof(TChildLinkedSourceModel);
            _getLookupIdsFunc = getLookupIdsFunc;
            _linkAction = linkAction;
            ModelType = typeof(TChildLinkedSourceModel);
        }

        protected override List<TId> GetLookupIdsTemplate(TLinkedSource linkedSource)
        {
            return _getLookupIdsFunc(linkedSource);
        }

        protected override void LinkAction(TLinkedSource linkedSource, List<TChildLinkedSourceModel> references, LoadedReferenceContext loadedReferenceContext)
        {
            var nestedLinkedSource = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(references);
            loadedReferenceContext.AddLinkedSourcesToBeBuilt(nestedLinkedSource);
            _linkAction(linkedSource, nestedLinkedSource);
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.NestedLinkedSource; }
        }

        public Type ChildLinkedSourceType { get; private set; }
        public Type ChildLinkedSourceModelType { get; private set; }

        public Type ModelType { get; private set; }
    }
}

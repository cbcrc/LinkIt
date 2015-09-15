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

        public NestedLinkedSourcesLoadLinkExpression(
            string linkTargetId,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc, Action<TLinkedSource,
            List<TChildLinkedSource>> linkAction) 
        {
            //stle: dry and test
            LoadLinkExpressionUtil.EnsureGenericParameterCannotBeList<TLinkedSource>(linkTargetId, "TLinkedSource");
            LoadLinkExpressionUtil.EnsureGenericParameterCannotBeList<TChildLinkedSource>(linkTargetId, "TChildLinkedSource");
            LoadLinkExpressionUtil.EnsureGenericParameterCannotBeList<TChildLinkedSource>(linkTargetId, "TChildLinkedSourceModel");
            LoadLinkExpressionUtil.EnsureGenericParameterCannotBeList<TId>(linkTargetId, "TId");

            ChildLinkedSourceType = typeof(TChildLinkedSource);
            ChildLinkedSourceModelType = typeof(TChildLinkedSourceModel);
            _getLookupIdsFunc = getLookupIdsFunc;
            _linkAction = linkAction;
            ModelType = typeof(TChildLinkedSourceModel);
        }

        //stle: delete
        [Obsolete]
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
            var nestedLinkedSource = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(references, loadedReferenceContext);
            _linkAction(linkedSource, nestedLinkedSource);
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.NestedLinkedSource; }
        }

        public Type ChildLinkedSourceType { get; private set; }
        public Type ChildLinkedSourceModelType { get; private set; }

        public Type ModelType { get; private set; }
        public List<Type> ChildLinkedSourceTypes { get { return new List<Type> { ChildLinkedSourceType }; } }
    }
}

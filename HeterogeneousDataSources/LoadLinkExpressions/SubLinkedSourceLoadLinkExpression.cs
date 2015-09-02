using System;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class SubLinkedSourceLoadLinkExpression<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel> : INestedLoadLinkExpression
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
    {
        private readonly Func<TLinkedSource, TChildLinkedSourceModel> _getSubLinkedSourceModelFunc;
        private readonly Action<TLinkedSource, TChildLinkedSource> _linkAction;

        public SubLinkedSourceLoadLinkExpression(Func<TLinkedSource, TChildLinkedSourceModel> getSubLinkedSourceModelFunc, Action<TLinkedSource, TChildLinkedSource> linkAction)
        {
            LinkedSourceType = typeof(TLinkedSource);

            _getSubLinkedSourceModelFunc = getSubLinkedSourceModelFunc;
            _linkAction = linkAction;
            ModelType = typeof (TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof (TChildLinkedSource);
            ChildLinkedSourceModelType = typeof(TChildLinkedSourceModel);
        }

        //stle: hey you and your inheritance crap! Try a functional approach
        public Type LinkedSourceType { get; private set; }
        
        //stle: that is not a load expression, but only a link expression
        public Type ReferenceType { get { return null; } }

        public LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressions.LoadLinkExpressionType.SubLinkedSource; }
        }

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext)
        {
            //stle: maybe split load and link iterfaces
        }

        public void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            //stle: hey you and your inheritance crap! Try a functional approach
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            
            Link((TLinkedSource) linkedSource, loadedReferenceContext);
        }

        public void Link(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext) {
            var subLinkedSourceModel = _getSubLinkedSourceModelFunc(linkedSource);
            var subLinkedSources = LoadLinkExpressionUtil.CreateLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(subLinkedSourceModel);
            loadedReferenceContext.AddLinkedSourcesToBeBuilt(subLinkedSources);
            _linkAction(linkedSource, subLinkedSources.SingleOrDefault());
        }

        public Type ModelType { get; private set; }
        public Type ChildLinkedSourceType { get; private set; }
        public Type ChildLinkedSourceModelType { get; private set; }
    }
}

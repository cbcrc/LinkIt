using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class SubLinkedSourceLoadLinkExpression<TLinkedSource, TSubLinkedSource, TSubLinkedSourceModel>: ILoadLinkExpression
        where TSubLinkedSource : class, ILinkedSource<TSubLinkedSourceModel>, new() 
    {
        private readonly Func<TLinkedSource, TSubLinkedSourceModel> _getSubLinkedSourceModelFunc;
        private readonly Action<TLinkedSource, TSubLinkedSource> _linkAction;

        public SubLinkedSourceLoadLinkExpression(Func<TLinkedSource, TSubLinkedSourceModel> getSubLinkedSourceModelFunc, Action<TLinkedSource, TSubLinkedSource> linkAction)
        {
            LinkedSourceType = typeof(TLinkedSource);

            _getSubLinkedSourceModelFunc = getSubLinkedSourceModelFunc;
            _linkAction = linkAction;
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
            var subLinkedSource = LoadLinkExpressionUtil.CreateLinkedSource<TSubLinkedSource, TSubLinkedSourceModel>(subLinkedSourceModel);
            loadedReferenceContext.AddLinkedSourceToBeBuilt(subLinkedSource);
            _linkAction(linkedSource, subLinkedSource);
        }
    }
}

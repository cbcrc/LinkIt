using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class SubLinkedSourcesLoadLinkExpression<TLinkedSource, TSubLinkedSource, TSubLinkedSourceModel>: ILoadLinkExpression
        where TSubLinkedSource : class, ILinkedSource<TSubLinkedSourceModel>, new() 
    {
        private readonly Func<TLinkedSource, List<TSubLinkedSourceModel>> _getSubLinkedSourceModelsFunc;
        private readonly Action<TLinkedSource, List<TSubLinkedSource>> _linkAction;

        public SubLinkedSourcesLoadLinkExpression(
            Func<TLinkedSource, List<TSubLinkedSourceModel>> getSubLinkedSourceModelsFunc, Action<TLinkedSource, List<TSubLinkedSource>> linkAction)
        {
            LinkedSourceType = typeof(TLinkedSource);
            _getSubLinkedSourceModelsFunc = getSubLinkedSourceModelsFunc;
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
            var subLinkedSourceModels = _getSubLinkedSourceModelsFunc(linkedSource);
            var subLinkedSources = LoadLinkExpressionUtil.CreateLinkedSources<TSubLinkedSource, TSubLinkedSourceModel>(subLinkedSourceModels);

            loadedReferenceContext.AddLinkedSourcesToBeBuilt(subLinkedSources);
            
            _linkAction(linkedSource, subLinkedSources);
        }
    }
}

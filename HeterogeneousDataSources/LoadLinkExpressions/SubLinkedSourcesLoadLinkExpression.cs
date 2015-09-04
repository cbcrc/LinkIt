using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class SubLinkedSourcesLoadLinkExpression<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel>: INestedLoadLinkExpression
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
    {
        private readonly Func<TLinkedSource, List<TChildLinkedSourceModel>> _getSubLinkedSourceModelsFunc;
        private readonly Action<TLinkedSource, List<TChildLinkedSource>> _linkAction;

        public SubLinkedSourcesLoadLinkExpression(
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getSubLinkedSourceModelsFunc, Action<TLinkedSource, List<TChildLinkedSource>> linkAction)
        {
            LinkedSourceType = typeof(TLinkedSource);
            ReferenceTypes= new List<Type>();
            _getSubLinkedSourceModelsFunc = getSubLinkedSourceModelsFunc;
            _linkAction = linkAction;
            ModelType = typeof (TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof(TChildLinkedSource);
            ChildLinkedSourceModelType = typeof(TChildLinkedSourceModel);
        }

        //stle: hey you and your inheritance crap! Try a functional approach
        public Type LinkedSourceType { get; private set; }
        
        //stle: that is not a load expression, but only a link expression
        public List<Type> ReferenceTypes { get; private set; }

        public LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressions.LoadLinkExpressionType.SubLinkedSource; }
        }

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded)
        {
            //stle: maybe split load and link iterfaces
        }

        public void Link(
            object linkedSource, 
            LoadedReferenceContext loadedReferenceContext,
            //stle: common interface sucks
            Type referenceTypeToBeLinked)
        {
            //stle: hey you and your inheritance crap! Try a functional approach
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            
            Link((TLinkedSource) linkedSource, loadedReferenceContext);
        }

        private void Link(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext) {
            var subLinkedSourceModels = _getSubLinkedSourceModelsFunc(linkedSource);
            var subLinkedSources = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(subLinkedSourceModels, loadedReferenceContext);
            
            _linkAction(linkedSource, subLinkedSources);
        }

        public Type ModelType { get; private set; }
        public Type ChildLinkedSourceType { get; private set; }
        public Type ChildLinkedSourceModelType { get; private set; }
    }
}

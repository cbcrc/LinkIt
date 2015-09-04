using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public class PolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel, TId> 
        : IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>
        where TChildLinkedSource : class, TIChildLinkedSource, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TId> _getLookupIdFunc;
        private readonly Action<TLink, TChildLinkedSource> _initChildLinkedSourceAction;

        public PolymorphicNestedLinkedSourceInclude(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLink, TChildLinkedSource> initChildLinkedSourceAction=null)
        {
            _getLookupIdFunc = getLookupIdFunc;
            _initChildLinkedSourceAction = initChildLinkedSourceAction;
            ReferenceType = typeof(TChildLinkedSourceModel);
        }

        public Type ReferenceType { get; private set; }

        public void AddLookupIds(TLink link, LookupIdContext lookupIdContext){
            var lookupIds = GetLookupIds(link);
            lookupIdContext.Add<TChildLinkedSourceModel, TId>(lookupIds);
        }

        public List<TIChildLinkedSource> CreateChildLinkedSources(TLink link, LoadedReferenceContext loadedReferenceContext){
            //stle: dry with other load link expressions
            var ids = GetLookupIds(link);
            var references = loadedReferenceContext.GetOptionalReferences<TChildLinkedSourceModel, TId>(ids);
            var childLinkedSources = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(references, loadedReferenceContext);

            InitChildLinkedSource(link, childLinkedSources);

            return childLinkedSources
                .Cast<TIChildLinkedSource>()
                .ToList();
        }

        private void InitChildLinkedSource(TLink link, List<TChildLinkedSource> childLinkedSources)
        {
            if (_initChildLinkedSourceAction == null) { return; }

            var childLinkedSource = childLinkedSources.SingleOrDefault();
            if (childLinkedSource == null) { return ;}

            _initChildLinkedSourceAction(link, childLinkedSources.Single());
        }


        //stle: dry with load link expressions
        private List<TId> GetLookupIds(TLink link) {
            var lookupId = _getLookupIdFunc(link);
            var lookupIds = new List<TId> { lookupId };
            return LoadLinkExpressionUtil.GetCleanedLookupIds(lookupIds);
        }

    }
}
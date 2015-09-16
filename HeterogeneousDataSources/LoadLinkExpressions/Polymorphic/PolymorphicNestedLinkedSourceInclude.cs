using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public class PolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel, TId>
        : IPolymorphicNestedLinkedSourceInclude<TLinkedSource,TIChildLinkedSource, TLink>
        where TChildLinkedSource : class, TIChildLinkedSource, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TId> _getLookupIdFunc;
        private readonly Action<TLink, TChildLinkedSource> _initChildLinkedSourceAction;
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSourceAction2;

        public PolymorphicNestedLinkedSourceInclude(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLink, TChildLinkedSource> initChildLinkedSourceAction=null,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction2 = null
        )
        {
            _getLookupIdFunc = getLookupIdFunc;
            _initChildLinkedSourceAction = initChildLinkedSourceAction;
            _initChildLinkedSourceAction2 = initChildLinkedSourceAction2;
            ReferenceType = typeof(TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ReferenceType { get; private set; }

        public void AddLookupIds(TLink link, LookupIdContext lookupIdContext){
            var lookupIds = GetLookupIds(link);
            lookupIdContext.Add<TChildLinkedSourceModel, TId>(lookupIds);
        }

        public List<TIChildLinkedSource> CreateChildLinkedSources(TLink link, LoadedReferenceContext loadedReferenceContext, TLinkedSource linkedSource, int referenceIndex){
            //stle: dry with other load link expressions
            var ids = GetLookupIds(link);
            var references = loadedReferenceContext.GetOptionalReferences<TChildLinkedSourceModel, TId>(ids);
            var childLinkedSources = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(references, loadedReferenceContext);

            InitChildLinkedSource(link, childLinkedSources, linkedSource, referenceIndex);

            return childLinkedSources
                .Cast<TIChildLinkedSource>()
                .ToList();
        }

        private void InitChildLinkedSource(TLink link, List<TChildLinkedSource> childLinkedSources, TLinkedSource linkedSource, int referenceIndex){
            var childLinkedSource = childLinkedSources.SingleOrDefault();
            if (childLinkedSource == null) { return; }

            if (_initChildLinkedSourceAction != null){
                _initChildLinkedSourceAction(link, childLinkedSources.Single());
            }
            if (_initChildLinkedSourceAction2 != null) {
                _initChildLinkedSourceAction2(linkedSource, referenceIndex, childLinkedSources.Single());
            }
        }


        //stle: dry with load link expressions
        private List<TId> GetLookupIds(TLink link) {
            var lookupId = _getLookupIdFunc(link);
            var lookupIds = new List<TId> { lookupId };
            return LoadLinkExpressionUtil.GetCleanedLookupIds(lookupIds);
        }

        public Type ChildLinkedSourceType { get; private set; }

    }
}
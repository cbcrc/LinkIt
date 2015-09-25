using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public class NestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel, TId> :
        IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink>, 
        IIncludeWithAddLookupId<TLink>, 
        IIncludeWithChildLinkedSource 
        where TChildLinkedSource : class, TIChildLinkedSource, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TId> _getLookupIdFunc;
        private readonly Func<TLinkedSource, int, TChildLinkedSource, TChildLinkedSource> _initChildLinkedSource;

        public NestedLinkedSourceInclude(
            Func<TLink, TId> getLookupIdFunc,
            Func<TLinkedSource, int, TChildLinkedSource, TChildLinkedSource> initChildLinkedSource = null
        )
        {
            _getLookupIdFunc = getLookupIdFunc;
            _initChildLinkedSource = initChildLinkedSource;
            ReferenceType = typeof(TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ReferenceType { get; private set; }

        public void AddLookupId(TLink link, LookupIdContext lookupIdContext)
        {
            var lookupId = _getLookupIdFunc(link);
            lookupIdContext.AddSingle<TChildLinkedSourceModel, TId>(lookupId);
        }

        public TIChildLinkedSource CreateNestedLinkedSource(TLink link, LoadedReferenceContext loadedReferenceContext, TLinkedSource linkedSource, int referenceIndex){
            //stle: dry with other load link expressions
            var lookupId = _getLookupIdFunc(link);
            var reference = loadedReferenceContext.GetOptionalReference<TChildLinkedSourceModel, TId>(lookupId);
            var childLinkedSource = LoadLinkExpressionUtil.CreateLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
                reference, 
                loadedReferenceContext
            );

            var initializedChildLinkedSource = InitChildLinkedSource(linkedSource, referenceIndex, childLinkedSource);

            return initializedChildLinkedSource;
        }

        private TChildLinkedSource InitChildLinkedSource(TLinkedSource linkedSource, int referenceIndex, TChildLinkedSource childLinkedSource) 
        {
            if (_initChildLinkedSource == null) { return childLinkedSource; }

            return _initChildLinkedSource(linkedSource, referenceIndex, childLinkedSource);
        }

        public Type ChildLinkedSourceType { get; private set; }

    }
}
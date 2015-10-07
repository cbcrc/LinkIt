using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public class NestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel> :
        IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink>, 
        IIncludeWithAddLookupId<TLink>, 
        IIncludeWithChildLinkedSource 
        where TChildLinkedSource : class, TIChildLinkedSource, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, object> _getLookupIdFunc;
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSourceAction;

        public NestedLinkedSourceInclude(
            Func<TLink, object> getLookupIdFunc,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction = null
        )
        {
            _getLookupIdFunc = getLookupIdFunc;
            _initChildLinkedSourceAction = initChildLinkedSourceAction;
            ReferenceType = typeof(TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ReferenceType { get; private set; }

        public void AddLookupId(TLink link, LookupIdContext lookupIdContext)
        {
            var lookupId = _getLookupIdFunc(link);
            lookupIdContext.AddSingle<TChildLinkedSourceModel>(lookupId);
        }

        public TIChildLinkedSource CreateNestedLinkedSource(TLink link, LoadedReferenceContext loadedReferenceContext, TLinkedSource linkedSource, int referenceIndex){
            //stle: dry with other load link expressions
            var lookupId = _getLookupIdFunc(link);
            var reference = loadedReferenceContext.GetOptionalReference<TChildLinkedSourceModel>(lookupId);
            var childLinkedSource = LoadLinkExpressionUtil.CreateLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
                reference, 
                loadedReferenceContext
            );

            InitChildLinkedSource(linkedSource, referenceIndex, childLinkedSource);

            return childLinkedSource;
        }

        private void InitChildLinkedSource(TLinkedSource linkedSource, int referenceIndex, TChildLinkedSource childLinkedSource) {
        if (childLinkedSource == null) { return; }

            if (_initChildLinkedSourceAction != null) {
                _initChildLinkedSourceAction(linkedSource, referenceIndex, childLinkedSource);
            }
        }

        public Type ChildLinkedSourceType { get; private set; }

    }
}
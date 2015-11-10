using System;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.ReferenceTrees;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public class IncludeNestedLinkedSourceById<TLinkedSource, TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel, TId> :
        IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink>, 
        IIncludeWithAddLookupId<TLink>, 
        IIncludeWithChildLinkedSource 
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TId> _getLookupIdFunc;
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSourceAction;

        public IncludeNestedLinkedSourceById(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction
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
            lookupIdContext.AddSingle<TChildLinkedSourceModel, TId>(lookupId);
        }

        public TIChildLinkedSource CreateNestedLinkedSource(TLink link, LoadedReferenceContext loadedReferenceContext, TLinkedSource linkedSource, int referenceIndex){
            var lookupId = _getLookupIdFunc(link);
            var reference = loadedReferenceContext.GetOptionalReference<TChildLinkedSourceModel, TId>(lookupId);
            var childLinkedSource = loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(reference);

            InitChildLinkedSource(linkedSource, referenceIndex, childLinkedSource);

            return (TIChildLinkedSource)(object)childLinkedSource;
        }

        private void InitChildLinkedSource(TLinkedSource linkedSource, int referenceIndex, TChildLinkedSource childLinkedSource) {
        if (childLinkedSource == null) { return; }

            if (_initChildLinkedSourceAction != null) {
                _initChildLinkedSourceAction(linkedSource, referenceIndex, childLinkedSource);
            }
        }

        public Type ChildLinkedSourceType { get; private set; }

        public void AddReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkConfig config)
        {
            var referenceTree = new ReferenceTree(
                ReferenceType,
                linkTargetId,
                parent
            );

            config.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, referenceTree);
        }
    }
}
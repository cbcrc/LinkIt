using System;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.LoadLinkExpressions.Includes.Interfaces;
using LinkIt.Protocols;
using LinkIt.ReferenceTrees;

namespace LinkIt.LoadLinkExpressions.Includes
{
    public class IncludeNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel, TId> :
        IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink>, 
        IIncludeWithAddLookupId<TLink>, 
        IIncludeWithChildLinkedSource 
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TId> _getLookupId;
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSourceAction;

        public IncludeNestedLinkedSourceById(
            Func<TLink, TId> getLookupId,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction
        )
        {
            _getLookupId = getLookupId;
            _initChildLinkedSourceAction = initChildLinkedSourceAction;
            ReferenceType = typeof(TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ReferenceType { get; private set; }

        public void AddLookupId(TLink link, LookupIdContext lookupIdContext)
        {
            var lookupId = _getLookupId(link);
            lookupIdContext.AddSingle<TChildLinkedSourceModel, TId>(lookupId);
        }

        public TAbstractChildLinkedSource CreateNestedLinkedSourceById(TLink link, LoadedReferenceContext loadedReferenceContext, TLinkedSource linkedSource, int referenceIndex){
            var lookupId = _getLookupId(link);
            var reference = loadedReferenceContext.GetOptionalReference<TChildLinkedSourceModel, TId>(lookupId);
            var childLinkedSource = loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(reference);

            InitChildLinkedSource(linkedSource, referenceIndex, childLinkedSource);

            return (TAbstractChildLinkedSource)(object)childLinkedSource;
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
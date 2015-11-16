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
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSource;

        public IncludeNestedLinkedSourceById(
            Func<TLink, TId> getLookupId,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource
        )
        {
            _getLookupId = getLookupId;
            _initChildLinkedSource = initChildLinkedSource;
            ReferenceType = typeof(TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ReferenceType { get; private set; }

        public void AddLookupId(TLink link, LookupIdContext lookupIdContext)
        {
            var lookupId = _getLookupId(link);
            lookupIdContext.AddSingle<TChildLinkedSourceModel, TId>(lookupId);
        }

        public TAbstractChildLinkedSource CreateNestedLinkedSourceById(TLink link, LoadedReferenceContext loadedReferenceContext, TLinkedSource linkedSource, int referenceIndex, LoadLinkProtocol loadLinkProtocol){
            var lookupId = _getLookupId(link);
            var reference = loadedReferenceContext.GetOptionalReference<TChildLinkedSourceModel, TId>(lookupId);
            var childLinkedSource = loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource(
                    reference,
                    loadLinkProtocol, CreateInitChildLinkedSourceAction(linkedSource, referenceIndex));

            return (TAbstractChildLinkedSource)(object)childLinkedSource;
        }

        private Action<TChildLinkedSource> CreateInitChildLinkedSourceAction(TLinkedSource linkedSource, int referenceIndex) {
            if (_initChildLinkedSource == null) { return null; }

            return childLinkedSource => _initChildLinkedSource(linkedSource, referenceIndex, childLinkedSource);
        }

        public Type ChildLinkedSourceType { get; private set; }

        public void AddReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkProtocol loadLinkProtocol)
        {
            var referenceTree = new ReferenceTree(
                ReferenceType,
                linkTargetId,
                parent
            );

            loadLinkProtocol.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, referenceTree);
        }
    }
}
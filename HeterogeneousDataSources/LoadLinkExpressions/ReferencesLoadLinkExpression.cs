using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class ReferencesLoadLinkExpression<TLinkedSource, TReference, TId>
        : LoadLinkExpression<TLinkedSource, TReference, TId>, ILoadLinkExpression
    {
        private readonly Func<TLinkedSource, List<TId>> _getLookupIdsFunc;
        private readonly Action<TLinkedSource, List<TReference>> _linkAction;

        public ReferencesLoadLinkExpression(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Action<TLinkedSource, List<TReference>> linkAction)
        {
            _getLookupIdsFunc = getLookupIdsFunc;
            _linkAction = linkAction;
        }

        protected override List<TId> GetLookupIdsTemplate(TLinkedSource linkedSource)
        {
            return _getLookupIdsFunc(linkedSource);
        }

        protected override void LinkAction(TLinkedSource linkedSource, List<TReference> references, LoadedReferenceContext loadedReferenceContext)
        {
            _linkAction(linkedSource, references);
        }

        public override bool IsNestedLinkedSourceLoadLinkExpression {
            get { return false; }
        }
    }
}

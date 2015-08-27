using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class ReferenceLoadLinkExpression<TLinkedSource, TReference, TId>
        : LoadLinkExpression<TLinkedSource, TReference, TId>, ILoadLinkExpression
    {
        private readonly Func<TLinkedSource, TId> _getLookupIdFunc;
        private readonly Action<TLinkedSource, TReference> _linkAction;

        public ReferenceLoadLinkExpression(Func<TLinkedSource, TId> getLookupIdFunc, Action<TLinkedSource, TReference> linkAction)
        {
            _getLookupIdFunc = getLookupIdFunc;
            _linkAction = linkAction;
        }

        protected override List<TId> GetLookupIds(TLinkedSource linkedSource)
        {
            return new List<TId>{
                _getLookupIdFunc(linkedSource)
            };
        }

        protected override void LinkAction(TLinkedSource linkedSource, List<TReference> references, LoadedReferenceContext loadedReferenceContext)
        {
            _linkAction(linkedSource, references.SingleOrDefault());
        }

        public override bool IsNestedLinkedSourceLoadLinkExpression {
            get { return false; }
        }
    }
}

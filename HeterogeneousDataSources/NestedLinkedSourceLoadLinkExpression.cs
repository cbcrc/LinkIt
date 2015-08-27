using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    public class NestedLinkedSourceLoadLinkExpression<TLinkedSource, TNestedLinkedSource, TNestedLinkedSourceModel, TId>
        : LoadLinkExpressionAbs<TLinkedSource, TNestedLinkedSourceModel, TId>, ILoadLinkExpression
        where TNestedLinkedSource : ILinkedSource<TNestedLinkedSourceModel>, new() 
    {
        private readonly Func<TLinkedSource, TId> _getLookupIdFunc;
        private readonly Action<TLinkedSource, TNestedLinkedSource> _linkAction;

        public NestedLinkedSourceLoadLinkExpression(Func<TLinkedSource, TId> getLookupIdFunc, Action<TLinkedSource, TNestedLinkedSource> linkAction)
        {
            _getLookupIdFunc = getLookupIdFunc;
            _linkAction = linkAction;
        }

        protected override List<TId> GetLookupIds(TLinkedSource linkedSource)
        {
            //stle: hey you and your inheritance crap! Try a functional approach
            return new List<TId>{
                _getLookupIdFunc(linkedSource)
            };
        }

        protected override void LinkAction(TLinkedSource linkedSource, List<TNestedLinkedSourceModel> references, LoadedReferenceContext loadedReferenceContext)
        {
            //stle: handle null
            var nestedLinkedSource = new TNestedLinkedSource{
                Model = references.SingleOrDefault()
            };

            loadedReferenceContext.AddLinkedSourceToBeBuilt(nestedLinkedSource);

            _linkAction(linkedSource, nestedLinkedSource);
        }

        public override bool IsNestedLinkedSourceLoadLinkExpression {
            get { return true; }
        }

    }
}

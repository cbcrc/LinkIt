using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpression<TLinkedSource, TReference> : ILoadExpression<TLinkedSource>, ILinkExpression<TLinkedSource>
    {
        public LoadLinkExpression(Func<TLinkedSource, object> getLookupIdFunc, Action<TLinkedSource, TReference> linkAction, Func<TReference, object> getIdFromReferenceFunc)
        {
            LinkAction = linkAction;
            GetIdFromReferenceFunc = getIdFromReferenceFunc;
            GetLookupIdFunc = getLookupIdFunc;
        }

        private Func<TLinkedSource, object> GetLookupIdFunc { get; set; }

        private Action<TLinkedSource, TReference> LinkAction { get; set; }
        private Func<TReference, object> GetIdFromReferenceFunc { get; set; }

        public List<object> GetLookupIds(TLinkedSource linkedSource)
        {
            return new List<object> { GetLookupIdFunc(linkedSource) };
        }

        public void Link(TLinkedSource linkedSource, DataContext dataContext) {
            var id = GetLookupIdFunc(linkedSource);
            var reference = dataContext.GetOptionalReference<TReference>(id);
            LinkAction(linkedSource, reference);
        }
    }
}

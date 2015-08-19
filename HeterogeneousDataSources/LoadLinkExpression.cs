using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpression<TLinkedSource, TReference, TId> : ILoadExpression<TLinkedSource, TId>, ILinkExpression<TLinkedSource>
    {
        public LoadLinkExpression(Func<TLinkedSource, TId> getLookupIdFunc, Action<TLinkedSource, TReference> linkAction)
        {
            GetLookupIdFunc = getLookupIdFunc;
            LinkAction = linkAction;
        }

        #region Load

        public List<TId> GetLookupIds(TLinkedSource linkedSource) {
            return new List<TId> { GetLookupIdFunc(linkedSource) };
        }
        private Func<TLinkedSource, TId> GetLookupIdFunc { get; set; }
        
        #endregion

        #region Link

        public void Link(TLinkedSource linkedSource, DataContext dataContext) {
            var id = GetLookupIdFunc(linkedSource);
            var reference = dataContext.GetOptionalReference<TReference, TId>(id);
            LinkAction(linkedSource, reference);
        }

        private Action<TLinkedSource, TReference> LinkAction { get; set; }
 
	    #endregion    
    }
}

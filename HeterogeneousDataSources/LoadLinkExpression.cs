using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpression<TLinkedSource, TReference> : ILoadExpression<TLinkedSource>, ILinkExpression<TLinkedSource>
    {
        public LoadLinkExpression(Func<TLinkedSource, object> getLookupIdFunc, Action<TLinkedSource, TReference> linkAction)
        {
            GetLookupIdFunc = getLookupIdFunc;
            LinkAction = linkAction;
        }

        #region Load

        public List<object> GetLookupIds(TLinkedSource linkedSource) {
            return new List<object> { GetLookupIdFunc(linkedSource) };
        }
        private Func<TLinkedSource, object> GetLookupIdFunc { get; set; }
        
        #endregion

        #region Link

        public void Link(TLinkedSource linkedSource, DataContext dataContext) {
            var id = GetLookupIdFunc(linkedSource);
            var reference = dataContext.GetOptionalReference<TReference>(id);
            LinkAction(linkedSource, reference);
        }

        private Action<TLinkedSource, TReference> LinkAction { get; set; }
 
	    #endregion    
    }
}

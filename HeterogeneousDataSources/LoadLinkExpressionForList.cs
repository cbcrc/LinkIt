using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpressionForList<TLinkedSource, TReference, TId> : ILoadExpression<TLinkedSource, TId>, ILinkExpression<TLinkedSource>
    {
        public LoadLinkExpressionForList(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc, 
            Action<TLinkedSource, List<TReference>> linkAction)
        {
            GetLookupIdsFunc = getLookupIdsFunc;
            LinkAction = linkAction;
        }

        #region Load

        public List<TId> GetLookupIds(TLinkedSource linkedSource) {
            return GetLookupIdsFunc(linkedSource);
        }
        private Func<TLinkedSource, List<TId>> GetLookupIdsFunc { get; set; }
        
        #endregion

        #region Link

        public void Link(TLinkedSource linkedSource, DataContext dataContext) {
            var ids = GetLookupIds(linkedSource);
            var reference = dataContext.GetOptionalReferences<TReference, TId>(ids);
            LinkAction(linkedSource, reference);
        }

        private Action<TLinkedSource, List<TReference>> LinkAction { get; set; }
 
	    #endregion    
    }
}

using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpressionForList<TLinkedSource, TReference, TId>: ILoadLinkExpression
    {
        public LoadLinkExpressionForList(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc, 
            Action<TLinkedSource, List<TReference>> linkAction)
        {
            GetLookupIdsFunc = getLookupIdsFunc;
            LinkAction = linkAction;
        }

        #region Load
        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext) {
            if (!(linkedSource is TLinkedSource)) { return; }

            var lookupIds = GetLookupIds((TLinkedSource)linkedSource);
            lookupIdContext.Add<TReference, TId>(lookupIds);
        }

        public List<TId> GetLookupIds(TLinkedSource linkedSource) {
            return GetLookupIdsFunc(linkedSource);
        }
        private Func<TLinkedSource, List<TId>> GetLookupIdsFunc { get; set; }

        public Type ReferenceType { get { return typeof(TReference); } }
        
        #endregion

        #region Link

        public void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext) {
            //stle: what should we do here? preconditions or defensive?
            if (!(linkedSource is TLinkedSource)) { throw new NotImplementedException(); }

            Link((TLinkedSource)linkedSource, loadedReferenceContext);
        }

        public void Link(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext) {
            var ids = GetLookupIds(linkedSource);
            var reference = loadedReferenceContext.GetOptionalReferences<TReference, TId>(ids);
            LinkAction(linkedSource, reference);
        }

        private Action<TLinkedSource, List<TReference>> LinkAction { get; set; }
 
	    #endregion
    }
}

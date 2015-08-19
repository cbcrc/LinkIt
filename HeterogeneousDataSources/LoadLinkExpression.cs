using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpression<TLinkedSource, TReference, TId> : ILoadLinkExpression<TId>, ILoadLinkExpression
    {
        public LoadLinkExpression(Func<TLinkedSource, TId> getLookupIdFunc, Action<TLinkedSource, TReference> linkAction)
        {
            GetLookupIdFunc = getLookupIdFunc;
            LinkAction = linkAction;
        }

        #region Load
        public List<TId> GetLookupIds(object linkedSource) {
            //stle: what should we do here? preconditions or defensive?
            if (!(linkedSource is TLinkedSource)) { throw new NotImplementedException(); }

            return GetLookupIds((TLinkedSource) linkedSource);
        }

        private List<TId> GetLookupIds(TLinkedSource linkedSource) {
            return new List<TId> { GetLookupIdFunc(linkedSource) };
        }
        private Func<TLinkedSource, TId> GetLookupIdFunc { get; set; }


        public Type ReferenceType { get { return typeof(TReference); } }

        #endregion

        #region Link

        public void Link(object linkedSource, DataContext dataContext) {
            //stle: what should we do here? preconditions or defensive?
            if (!(linkedSource is TLinkedSource)) { throw new NotImplementedException(); }

            Link((TLinkedSource)linkedSource, dataContext);
        }

        private void Link(TLinkedSource linkedSource, DataContext dataContext) {
            var id = GetLookupIdFunc(linkedSource);
            var reference = dataContext.GetOptionalReference<TReference, TId>(id);
            LinkAction(linkedSource, reference);
        }

        private Action<TLinkedSource, TReference> LinkAction { get; set; }
 
	    #endregion
    }
}

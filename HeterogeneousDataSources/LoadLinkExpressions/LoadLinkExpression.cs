using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public abstract class LoadLinkExpression<TLinkedSource, TReference, TId> 
    {
        protected LoadLinkExpression()
        {
            LinkedSourceType = typeof (TLinkedSource);
            ReferenceType = typeof (TReference);
        }

        public Type LinkedSourceType { get; private set; }
        public Type ReferenceType { get; private set; }

        public abstract LoadLinkExpressionType LoadLinkExpressionType { get; }

        #region Load
        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext)
        {
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            var lookupIds = GetLookupIds((TLinkedSource)linkedSource);
            lookupIdContext.Add<TReference, TId>(lookupIds);
        }

        protected abstract List<TId> GetLookupIdsTemplate(TLinkedSource linkedSource);
        
        #endregion

        #region Link
        public void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            Link((TLinkedSource) linkedSource, loadedReferenceContext);
        }

        private void Link(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext) {
            var ids = GetLookupIds(linkedSource);
            var references = loadedReferenceContext.GetOptionalReferences<TReference, TId>(ids);
            LinkAction(linkedSource, references, loadedReferenceContext);
        }

        protected abstract void LinkAction(
            TLinkedSource linkedSource, 
            List<TReference> references,
            //stle: hey you and your inheritance crap! Try a functional approach
            LoadedReferenceContext loadedReferenceContext
        ); 
        #endregion

        private List<TId> GetLookupIds(TLinkedSource linkedSource)
        {
            var lookupIds = GetLookupIdsTemplate(linkedSource);
            if(lookupIds==null){ return new List<TId>(); }

            return lookupIds
                .Where(id => id != null)
                .ToList();
        }
    }
}
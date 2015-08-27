using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public abstract class LoadLinkExpression<TLinkedSource, TReference, TId> : ILoadLinkExpression
    {
        protected LoadLinkExpression()
        {
            LinkedSourceType = typeof (TLinkedSource);
            ReferenceType = typeof (TReference);
        }

        public Type LinkedSourceType { get; private set; }
        public Type ReferenceType { get; private set; }
        

        public abstract bool IsNestedLinkedSourceLoadLinkExpression { get; }

        #region Load
        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext)
        {
            EnsureLinkedSourceIsOfTLinkedSource(linkedSource);

            lookupIdContext.Add<TReference, TId>(GetLookupIds((TLinkedSource)linkedSource));
        }

        protected abstract List<TId> GetLookupIds(TLinkedSource linkedSource);
        
        #endregion

        #region Link
        public void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            EnsureLinkedSourceIsOfTLinkedSource(linkedSource);

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

        private void EnsureLinkedSourceIsOfTLinkedSource(object linkedSource) {
            if (!(linkedSource is TLinkedSource)) {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot invoke load-link expression for {0} with linked source of type {1}",
                        LinkedSourceType.Name,
                        linkedSource != null
                            ? linkedSource.GetType().Name
                            : "Null"
                        )
                    );
            }
        }
    }
}
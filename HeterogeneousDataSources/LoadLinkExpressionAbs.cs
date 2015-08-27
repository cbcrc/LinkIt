using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public abstract class LoadLinkExpressionAbs<TLinkedSource, TReference, TId> : ILoadLinkExpression
    {
        protected LoadLinkExpressionAbs()
        {
            ReferenceType = typeof (TReference);
        }

        public Type ReferenceType { get; private set; }
        public abstract bool IsNestedLinkedSourceLoadLinkExpression { get; }

        #region Load
        public void AddLookupIds(List<object> linkedSources, LookupIdContext lookupIdContext) {
            var lookupIds = GetLinkedSourceOfTLinkedSource(linkedSources)
                .SelectMany(GetLookupIds)
                .ToList();

            //stle: eventully throw instead of skipping, because of easier debuging and little performance improvement
            if (lookupIds.Any() == false) { return; }

            lookupIdContext.Add<TReference, TId>(lookupIds);
        }

        private List<TLinkedSource> GetLinkedSourceOfTLinkedSource(List<object> linkedSources)
        {
            return linkedSources
                //stle: eventully throw instead of skipping, because of easier debuging and little performance improvement
                .Where(linkedSource => linkedSource is TLinkedSource)
                .Cast<TLinkedSource>()
                .ToList();
        }

        protected abstract List<TId> GetLookupIds(TLinkedSource linkedSource);
        
        #endregion

        #region Link
        public void Link(List<object> linkedSources, LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkedSource in GetLinkedSourceOfTLinkedSource(linkedSources))
            {
                Link(linkedSource, loadedReferenceContext);
            }
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public abstract class LoadLinkExpression<TLinkedSource, TReference, TId> : ILoadLinkExpression
    {
        protected LoadLinkExpression()
        {
            ReferenceType = typeof (TReference);
        }

        public Type ReferenceType { get; private set; }
        public abstract bool IsNestedLinkedSourceLoadLinkExpression { get; }

        #region Load
        public void AddLookupIds(List<object> linkedSources, LookupIdContext lookupIdContext) {
            var lookupIds = GetLinkedSourcesOfTLinkedSource(linkedSources)
                .SelectMany(GetLookupIds)
                .ToList();

            //stle: eventully throw instead of skipping, because of easier debuging and little performance improvement
            if (lookupIds.Any() == false) { return; }

            lookupIdContext.Add<TReference, TId>(lookupIds);
        }

        private List<TLinkedSource> GetLinkedSourcesOfTLinkedSource(List<object> linkedSources)
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
        public void Link(LoadedReferenceContext loadedReferenceContext)
        {
            var linkedSources = GetLinkedSourcesOfTLinkedSource(loadedReferenceContext.LinkedSourcesToBeBuilt);
            foreach (var linkedSource in linkedSources)
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
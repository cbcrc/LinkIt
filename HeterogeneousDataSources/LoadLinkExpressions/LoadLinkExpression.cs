using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public abstract class LoadLinkExpression<TLinkedSource, TReference, TId>:ILoadLinkExpression
    {
        protected LoadLinkExpression()
        {
            LinkedSourceType = typeof (TLinkedSource);
            ReferenceType = typeof (TReference);
            ReferenceTypes = new List<Type> { ReferenceType };
        }

        //stle: delete or do it
        public virtual string LinkTargetId { get { return "n/a"; } }

        public Type LinkedSourceType { get; private set; }
        
        private Type ReferenceType { get; set; }

        //In order to implement ILoadLinkExpression for sub class
        public List<Type> ReferenceTypes { get; private set; }

        public abstract LoadLinkExpressionType LoadLinkExpressionType { get; }

        #region Load

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded)
        {
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            LoadLinkExpressionUtil.EnsureIsOfReferenceType(this,referenceTypeToBeLoaded);

            var lookupIds = GetLookupIds((TLinkedSource)linkedSource);
            lookupIdContext.Add<TReference, TId>(lookupIds);
        }

        protected abstract List<TId> GetLookupIdsTemplate(TLinkedSource linkedSource);
        
        #endregion

        #region Link
        public void Link(
            object linkedSource, 
            LoadedReferenceContext loadedReferenceContext,
            //stle: common interface sucks
            Type referenceTypeToBeLinked)
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
            return LoadLinkExpressionUtil.GetCleanedLookupIds(lookupIds);
        }
    }
}
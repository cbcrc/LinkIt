using System;
using System.Collections.Generic;

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

        public virtual void LinkNestedLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked)
        {
            //no operations
        }

        public virtual void LinkSubLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            //no operations
        }

        public virtual void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            //no operations
        }

        protected abstract List<TId> GetLookupIdsTemplate(TLinkedSource linkedSource);
        
        #endregion

        //stle: dry
        protected List<TReference> GetReferences(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            var ids = GetLookupIds(linkedSource);
            return loadedReferenceContext.GetOptionalReferences<TReference, TId>(ids);
        }


        //stle: dry
        protected List<TId> GetLookupIds(TLinkedSource linkedSource)
        {
            var lookupIds = GetLookupIdsTemplate(linkedSource);
            return LoadLinkExpressionUtil.GetCleanedLookupIds(lookupIds);
        }
    }
}
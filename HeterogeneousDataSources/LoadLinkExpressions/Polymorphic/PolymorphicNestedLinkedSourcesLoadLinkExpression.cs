using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public class PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> : ILoadLinkExpression
    {
        public PolymorphicNestedLinkedSourcesLoadLinkExpression(
            Func<TLinkedSource, List<TLink>> getLinksFunc,
            Action<TLinkedSource, List<TIChildLinkedSource>> linkAction,
            Func<TLink, TDiscriminant> getDiscriminantFunc,
            Dictionary<string, IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>> polyIncludes)
        {
            throw new NotImplementedException();
        }

        public Type LinkedSourceType { get; private set; }
        public List<Type> ReferenceTypes { get; private set; }
        public LoadLinkExpressionType LoadLinkExpressionType { get; private set; }
        public bool DoesMatchReferenceTypeToBeLoaded(object linkedSource, List<Type> referenceTypesToBeLoaded)
        {
            throw new NotImplementedException();
        }

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext)
        {
            throw new NotImplementedException();
        }

        public void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            throw new NotImplementedException();
        }
    }
}
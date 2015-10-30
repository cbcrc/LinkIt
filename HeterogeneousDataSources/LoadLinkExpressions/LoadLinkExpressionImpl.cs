using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    //stle: TIChildLinkedSource is not more of a TTarget
    public class LoadLinkExpressionImpl<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> :
        INestedLoadLinkExpression
    {
        private readonly LinkTargetBase<TLinkedSource, TIChildLinkedSource> _linkTarget;
        private readonly Func<TLinkedSource, List<TLink>> _getLinksFunc;
        private readonly IncludeSet<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> _includeSet;

        public LoadLinkExpressionImpl(
            LinkTargetBase<TLinkedSource,TIChildLinkedSource> linkTarget,
            Func<TLinkedSource, List<TLink>> getLinksFunc,
            Func<TLink, TDiscriminant> getDiscriminantFunc,
            Dictionary<TDiscriminant, IInclude> includes)
        {
            _linkTarget = linkTarget;
            _getLinksFunc = getLinksFunc;
            _includeSet = new IncludeSet<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>(
                includes,
                getDiscriminantFunc
                );
            LinkedSourceType = typeof (TLinkedSource);

            ReferenceTypes = _includeSet.GetIncludesWithAddLookupId()
                .Select(include => include.ReferenceType)
                .ToList();

            ChildLinkedSourceTypes = _includeSet.GetIncludesWithChildLinkedSource()
                .Select(include => include.ChildLinkedSourceType)
                .ToList();
        }

        public bool IsInDifferentLoadingLevel(ILoadLinkExpression child)
        {
            return _includeSet.GetIncludesWithChildLinkedSource()
                .Where(include => include.ChildLinkedSourceType == child.LinkedSourceType)
                .Any(include => include is IIncludeWithAddLookupId<TLink>);
        }

        public string LinkTargetId { get { return _linkTarget.Id; } }
        public Type LinkedSourceType { get; private set; }
        public List<Type> ReferenceTypes { get; private set; }

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded)
        {
            //stle: dry
            EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            EnsureIsOfReferenceType(this, referenceTypeToBeLoaded);

            var notNullLinks = GetLinks((TLinkedSource)linkedSource)
                .Where(link => link != null)
                .ToList();

            foreach (var link in notNullLinks)
            {
                var include = _includeSet.GetIncludeWithAddLookupId(link);

                if (include != null && include.ReferenceType == referenceTypeToBeLoaded) {
                    include.AddLookupId(link, lookupIdContext);
                }
            }
        }

        public void LinkSubLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            //stle: dry
            EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            SetLinkTargetValue(
                (TLinkedSource)linkedSource,
                link => _includeSet.GetIncludeWithCreateSubLinkedSource(link),
                linkWithIndexAndInclude => linkWithIndexAndInclude.Include.CreateSubLinkedSource(
                    linkWithIndexAndInclude.Link, 
                    loadedReferenceContext
                )
            );
        }

        public void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            //stle: dry
            EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            SetLinkTargetValue(
                (TLinkedSource)linkedSource,
                link => _includeSet.GetIncludeWithGetReference(link),
                linkWithIndexAndInclude => linkWithIndexAndInclude.Include.GetReference(
                    linkWithIndexAndInclude.Link, 
                    loadedReferenceContext
                )
            );
        }

        public void LinkNestedLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked)
        {
            //stle: dry
            EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            SetLinkTargetValue(
                (TLinkedSource)linkedSource, 
                link => _includeSet.GetIncludeWithCreateNestedLinkedSourceForReferenceType(link, referenceTypeToBeLinked),
                linkWithIndexAndInclude =>
                    linkWithIndexAndInclude.Include.CreateNestedLinkedSource(
                        linkWithIndexAndInclude.Link,
                        loadedReferenceContext,
                        (TLinkedSource)linkedSource,
                        linkWithIndexAndInclude.Index
                    )
            );
        }

        #region SetLinkTargetValue

        private void SetLinkTargetValue<TInclude>(
            TLinkedSource linkedSource,
            Func<TLink, TInclude> getInclude,
            Func<LinkWithIndexAndInclude<TLink, TInclude>, TIChildLinkedSource> getLinkTargetValueForLink) 
        {
            var linkTargetResolver = new LinkTargetValueResolver<TIChildLinkedSource, TLink, TInclude>(
                GetLinks(linkedSource),
                getInclude,
                getLinkTargetValueForLink
            );
            var listOfLinkTargetValueWithIndex = linkTargetResolver.Resolve();

            SetLinkTargetValue(linkedSource, listOfLinkTargetValueWithIndex);
        }

        private void SetLinkTargetValue(TLinkedSource linkedSource, List<LinkTargetValueWithIndex<TIChildLinkedSource>> listOfLinkTargetValueWithIndex)
        {
            _linkTarget.SetLinkTargetValues(
                linkedSource, 
                listOfLinkTargetValueWithIndex,
                GetLinks(linkedSource).Count
            );
        }
        #endregion

        private List<TLink> GetLinks(TLinkedSource linkedSource)
        {
            var links = _getLinksFunc(linkedSource);

            if (links == null) { return new List<TLink>(); }

            return links;
        }

        public List<Type> ChildLinkedSourceTypes { get; private set; }

        private static void EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(object linkedSource) {
            if (!(linkedSource is TLinkedSource)) {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot invoke load-link expression for {0} with linked source of type {1}",
                        typeof(TLinkedSource),
                        linkedSource != null
                            ? linkedSource.GetType().ToString()
                            : "Null"
                        )
                    );
            }
        }

        private static void EnsureIsOfReferenceType(ILoadLinkExpression loadLinkExpression, Type referenceType) {
            if (!loadLinkExpression.ReferenceTypes.Contains(referenceType)) {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot invoke this load link expression for reference type {0}. Supported reference types are {1}. This load link expression is for {2}.",
                        referenceType,
                        String.Join(",", loadLinkExpression.ReferenceTypes),
                        loadLinkExpression.LinkedSourceType
                    )
                );
            }
        }
    }
}
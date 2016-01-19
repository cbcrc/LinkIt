using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Core.Includes;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.Core.Interfaces;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.ReferenceTrees;
using LinkIt.Shared;

namespace LinkIt.Core
{
    //See ILoadLinkExpression
    public class LoadLinkExpressionImpl<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>:ILoadLinkExpression
    {
        private readonly ILinkTarget<TLinkedSource, TAbstractLinkTarget> _linkTarget;
        private readonly Func<TLinkedSource, List<TLink>> _getLinks;
        private readonly IncludeSet<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> _includeSet;

        internal LoadLinkExpressionImpl(
            Func<TLinkedSource, List<TLink>> getLinks, 
            ILinkTarget<TLinkedSource, TAbstractLinkTarget> linkTarget, 
            IncludeSet<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> includeset)
        {
            _linkTarget = linkTarget;
            _getLinks = getLinks;
            _includeSet = includeset;
            LinkedSourceType = typeof (TLinkedSource);

            ReferenceTypes = _includeSet.GetIncludesWithAddLookupId()
                .Select(include => include.ReferenceType)
                .ToList();

            ChildLinkedSourceTypes = _includeSet.GetIncludesWithChildLinkedSource()
                .Select(include => include.ChildLinkedSourceType)
                .ToList();
        }

        public string LinkTargetId { get { return _linkTarget.Id; } }
        public Type LinkedSourceType { get; private set; }
        public List<Type> ReferenceTypes { get; private set; }

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded)
        {
            AssumeLinkedSourceIsOfTLinkedSource(linkedSource);
            AssumeIsOfReferenceType(this, referenceTypeToBeLoaded);

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

        public void LinkNestedLinkedSourceFromModel(object linkedSource, LoadedReferenceContext loadedReferenceContext, LoadLinkProtocol loadLinkProtocol){
            SetLinkTargetValues(
                linkedSource,
                _includeSet.GetIncludeWithCreateNestedLinkedSourceFromModel,
                (link, include, linkIndex) => include.CreateNestedLinkedSourceFromModel(link, loadedReferenceContext, loadLinkProtocol)
            );
        }

        public void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext){
            SetLinkTargetValues(
                linkedSource, 
                _includeSet.GetIncludeWithGetReference,
                (link, include, linkIndex) => include.GetReference(link, loadedReferenceContext) 
            );
        }

        public void LinkNestedLinkedSourceById(
            object linkedSource, 
            LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked,
            LoadLinkProtocol loadLinkProtocol
            )
        {
            SetLinkTargetValues(
                linkedSource,
                link => _includeSet.GetIncludeWithCreateNestedLinkedSourceByIdForReferenceType(link, referenceTypeToBeLinked),
                (link, include, linkIndex) => 
                    include.CreateNestedLinkedSourceById(
                        link,
                        loadedReferenceContext,
                        (TLinkedSource)linkedSource,
                        linkIndex,
                        loadLinkProtocol
                    )
            );
        }

        public void FilterOutNullValues(object linkedSource){
            AssumeLinkedSourceIsOfTLinkedSource(linkedSource);

            _linkTarget.FilterOutNullValues((TLinkedSource)linkedSource);
        }

        private void SetLinkTargetValues<TInclude>(
            object linkedSource,
            Func<TLink, TInclude> getInclude,
            Func<TLink, TInclude, int, TAbstractLinkTarget> getLinkTargetValue)
        {
            AssumeLinkedSourceIsOfTLinkedSource(linkedSource);

            SetLinkTargetValues(
                (TLinkedSource)linkedSource,
                getInclude,
                getLinkTargetValue
            );
        }


        private void SetLinkTargetValues<TInclude>(
            TLinkedSource linkedSource,
            Func<TLink, TInclude> getInclude,
            Func<TLink, TInclude, int, TAbstractLinkTarget> getLinkTargetValue) 
        {
            var links = GetLinks(linkedSource);
            _linkTarget.LazyInit(linkedSource, links.Count);

            for (int linkIndex = 0; linkIndex < links.Count; linkIndex++) {
                var link = links[linkIndex];
                if (link == null) { continue; }

                var include = getInclude(link);
                if (include == null) { continue; }

                _linkTarget.SetLinkTargetValue(
                    linkedSource,
                    getLinkTargetValue(link, include, linkIndex),
                    linkIndex
                );
            }
        }

        private List<TLink> GetLinks(TLinkedSource linkedSource)
        {
            var links = _getLinks(linkedSource);

            if (links == null) { return new List<TLink>(); }

            return links;
        }

        public List<Type> ChildLinkedSourceTypes { get; private set; }

        private static void AssumeLinkedSourceIsOfTLinkedSource(object linkedSource) {
            if (!(linkedSource is TLinkedSource)) {
                throw new AssumptionFailed(
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

        private static void AssumeIsOfReferenceType(ILoadLinkExpression loadLinkExpression, Type referenceType) {
            if (!loadLinkExpression.ReferenceTypes.Contains(referenceType)) {
                throw new AssumptionFailed(
                    string.Format(
                        "Cannot invoke this load link expression for reference type {0}. Supported reference types are {1}. This load link expression is for {2}.",
                        referenceType,
                        String.Join(",", loadLinkExpression.ReferenceTypes),
                        loadLinkExpression.LinkedSourceType
                    )
                );
            }
        }

        public void AddReferenceTreeForEachInclude(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol)
        {
            AddReferenceTreeForEachIncludeWithAddLookupId(parent, loadLinkProtocol);
            AddReferenceTreeForEachIncludeWithCreateNestedLinkedSourceFromModel(parent, loadLinkProtocol);
        }

        private void AddReferenceTreeForEachIncludeWithCreateNestedLinkedSourceFromModel(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol)
        {
            _includeSet
                .GetIncludes<IIncludeWithCreateNestedLinkedSourceFromModel<TAbstractLinkTarget, TLink>>()
                .ToList()
                .ForEach(include =>
                    include.AddReferenceTreeForEachLinkTarget(parent, loadLinkProtocol)
                );
        }

        private void AddReferenceTreeForEachIncludeWithAddLookupId(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol)
        {
            _includeSet
                .GetIncludes<IIncludeWithAddLookupId<TLink>>()
                .ToList()
                .ForEach(include =>
                    include.AddReferenceTree(LinkTargetId, parent, loadLinkProtocol)
                );
        }
    }
}
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Core.Includes;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.ReadableExpressions.Extensions;
using LinkIt.Shared;
using LinkIt.TopologicalSorting;

namespace LinkIt.Core
{
    //See ILoadLinkExpression
    internal class LoadLinkExpressionImpl<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> : ILoadLinkExpression
    {
        private readonly Func<TLinkedSource, IEnumerable<TLink>> _getLinks;
        private readonly IncludeSet<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> _includeSet;
        private readonly ILinkTarget<TLinkedSource, TAbstractLinkTarget> _linkTarget;

        internal LoadLinkExpressionImpl(
            Func<TLinkedSource, IEnumerable<TLink>> getLinks,
            ILinkTarget<TLinkedSource, TAbstractLinkTarget> linkTarget,
            IncludeSet<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> includeset)
        {
            _linkTarget = linkTarget;
            _getLinks = getLinks;
            _includeSet = includeset;
            LinkedSourceType = typeof(TLinkedSource);

            ReferenceTypes = _includeSet.GetIncludesWithAddLookupId()
                .Select(include => include.ReferenceType)
                .ToHashSet();

            ChildLinkedSourceTypes = _includeSet.GetIncludesWithChildLinkedSource()
                .Select(include => include.ChildLinkedSourceType)
                .ToHashSet();
        }

        public ISet<Type> ChildLinkedSourceTypes { get; }

        public string LinkTargetId => _linkTarget.Id;
        public Type LinkedSourceType { get; }
        public ISet<Type> ReferenceTypes { get; }

        public void AddLookupIds(object linkedSource, LookupContext lookupContext, Type referenceTypeToBeLoaded)
        {
            EnsureLinkedSourceIsOfTLinkedSource(linkedSource);
            EnsureIsReferenceType(this, referenceTypeToBeLoaded);

            var notNullLinks = GetLinks((TLinkedSource) linkedSource)
                .WhereNotNull();

            foreach (var link in notNullLinks)
            {
                var include = _includeSet.GetIncludeWithAddLookupId(link);
                if (include?.ReferenceType == referenceTypeToBeLoaded)
                {
                    include.AddLookupId(link, lookupContext);
                }
            }
        }

        public void LinkNestedLinkedSourceFromModel(object linkedSource, Linker linker, LoadLinkProtocol loadLinkProtocol)
        {
            SetLinkTargetValues(
                linkedSource,
                _includeSet.GetIncludeWithCreateNestedLinkedSourceFromModel,
                (link, include, linkIndex) => include.CreateNestedLinkedSourceFromModel(
                    link,
                    linker,
                    (TLinkedSource) linkedSource,
                    linkIndex,
                    loadLinkProtocol
                )
            );
        }

        public void LinkReference(object linkedSource, DataStore dataStore)
        {
            SetLinkTargetValues(
                linkedSource,
                _includeSet.GetIncludeWithGetReference,
                (link, include, _) => include.GetReference(link, dataStore)
            );

            _linkTarget.FilterOutNullValues((TLinkedSource) linkedSource);
        }

        public void LinkNestedLinkedSourceById(
            object linkedSource,
            Linker linker,
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
                        linker,
                        (TLinkedSource) linkedSource,
                        linkIndex,
                        loadLinkProtocol
                    )
            );
        }

        public void AddDependencyForEachInclude(Dependency predecessor, LoadLinkProtocol loadLinkProtocol)
        {
            AddDependenciesForAllReferences(predecessor, loadLinkProtocol);
            AddDependenciesForAllNestedLinkedSources(predecessor, loadLinkProtocol);
        }

        private void SetLinkTargetValues<TInclude>(
            object linkedSource,
            Func<TLink, TInclude> getInclude,
            Func<TLink, TInclude, int, TAbstractLinkTarget> getLinkTargetValue)
        {
            EnsureLinkedSourceIsOfTLinkedSource(linkedSource);

            SetLinkTargetValues(
                (TLinkedSource) linkedSource,
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

            TAbstractLinkTarget GetLinkTargetValueForIndex(int index) => GetLinkTargetValue(links, index, getInclude, getLinkTargetValue);

            for (var linkIndex = 0; linkIndex < links.Count; linkIndex++)
            {
                SetLinkTargetValue(linkedSource, linkIndex, GetLinkTargetValueForIndex);
            }
        }

        private void SetLinkTargetValue(TLinkedSource linkedSource, int linkIndex, Func<int, TAbstractLinkTarget> getLinkTargetValueForIndex)
        {
            if (HasLinkTargetValue(linkedSource, linkIndex))
            {
                return;
            }

            var value = getLinkTargetValueForIndex(linkIndex);
            if (value is null)
            {
                return;
            }

            _linkTarget.SetLinkTargetValue(
                linkedSource,
                value,
                linkIndex
            );
        }

        private bool HasLinkTargetValue(TLinkedSource linkedSource, int linkIndex)
        {
            var current = _linkTarget.GetLinkTargetValue(linkedSource, linkIndex);
            return !(current is null);
        }

        private static TAbstractLinkTarget GetLinkTargetValue<TInclude>(
            List<TLink> links,
            int linkIndex,
            Func<TLink, TInclude> getInclude,
            Func<TLink, TInclude, int, TAbstractLinkTarget> getLinkTargetValue)
        {
            var link = links[linkIndex];
            if (link is null)
            {
                return default;
            }

            var include = getInclude(link);
            if (include is null)
            {
                return default;
            }

            return getLinkTargetValue(link, include, linkIndex);
        }

        private List<TLink> GetLinks(TLinkedSource linkedSource)
        {
            var links = _getLinks(linkedSource);

            return links?.ToList() ?? new List<TLink>();
        }

        private static void EnsureLinkedSourceIsOfTLinkedSource(object linkedSource)
        {
            if (!(linkedSource is TLinkedSource))
            {
                throw new LinkItException(
                   $"Cannot invoke load-link expression for {typeof(TLinkedSource).GetFriendlyName()} with linked source of type {linkedSource?.GetType().GetFriendlyName()}"
               );
            }
        }

        private static void EnsureIsReferenceType(ILoadLinkExpression loadLinkExpression, Type referenceType)
        {
            if (!loadLinkExpression.ReferenceTypes.Contains(referenceType))
            {
                throw new LinkItException(
                   $"Cannot invoke this load link expression for reference type {referenceType.GetFriendlyName()}." +
                   $"Supported reference types are {string.Join(",", loadLinkExpression.ReferenceTypes.Select(t => t.GetFriendlyName()))}." +
                   $"This load link expression is for {loadLinkExpression.LinkedSourceType.GetFriendlyName()}."
               );
            }
        }

        private void AddDependenciesForAllNestedLinkedSources(Dependency predecessor, LoadLinkProtocol loadLinkProtocol)
        {
            foreach (var include in _includeSet.GetIncludes<IIncludeWithCreateNestedLinkedSourceFromModel<TLinkedSource, TAbstractLinkTarget, TLink>>())
            {
                include.AddDependenciesForAllLinkTargets(predecessor, loadLinkProtocol);
            }
        }

        private void AddDependenciesForAllReferences(Dependency predecessor, LoadLinkProtocol loadLinkProtocol)
        {
            foreach (var include in _includeSet.GetIncludes<IIncludeWithAddLookupId<TLink>>())
            {
                include.AddDependency(predecessor, loadLinkProtocol);
            }
        }
    }
}

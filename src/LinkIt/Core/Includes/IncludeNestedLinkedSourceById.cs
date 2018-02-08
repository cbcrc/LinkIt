// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.PublicApi;
using LinkIt.TopologicalSorting;

namespace LinkIt.Core.Includes
{
    /// <summary>
    /// For a nested linked source loaded by id,
    /// responsible for loading and linking the link target for a specific discriminant
    /// responsible for creating the reference tree for a specific discriminant
    /// </summary>
    internal class IncludeNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel, TId> :
        IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink>,
        IIncludeWithAddLookupId<TLink>,
        IIncludeWithChildLinkedSource
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TId> _getLookupId;
        private readonly Action<TLink, TChildLinkedSource> _initChildLinkedSourceWithLink;
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSourceWithParentAndReferenceIndex;

        public Type ChildLinkedSourceType { get; }

        public Type ReferenceType { get; }

        public IncludeNestedLinkedSourceById(
            Func<TLink, TId> getLookupId,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource
        )
        {
            _getLookupId = getLookupId;
            _initChildLinkedSourceWithParentAndReferenceIndex = initChildLinkedSource;
            ReferenceType = typeof(TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public IncludeNestedLinkedSourceById(
            Func<TLink, TId> getLookupId,
            Action<TLink, TChildLinkedSource> initChildLinkedSource
        )
        {
            _getLookupId = getLookupId;
            _initChildLinkedSourceWithLink = initChildLinkedSource;
            ReferenceType = typeof(TChildLinkedSourceModel);
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public void AddLookupId(TLink link, LookupContext lookupContext)
        {
            var lookupId = _getLookupId(link);
            lookupContext.AddLookupId<TChildLinkedSourceModel>(lookupId);
        }

        public void AddDependency(Dependency predecessor, LoadLinkProtocol loadLinkProtocol)
        {
            var dependency = predecessor.Graph.GetOrAdd(ReferenceType, ChildLinkedSourceType);
            predecessor.Before(dependency);

            loadLinkProtocol.AddDependenciesForAllLinkTargets(ChildLinkedSourceType, dependency);
        }

        public TAbstractChildLinkedSource CreateNestedLinkedSourceById(TLink link, Linker linker, TLinkedSource linkedSource, int referenceIndex, LoadLinkProtocol loadLinkProtocol)
        {
            var lookupId = _getLookupId(link);
            var childLinkedSource = linker.CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel, TId>(
                lookupId,
                CreateInitChildLinkedSourceAction(linkedSource, referenceIndex, link)
            );

            return (TAbstractChildLinkedSource) (object) childLinkedSource;
        }

        private Action<TChildLinkedSource> CreateInitChildLinkedSourceAction(TLinkedSource linkedSource, int referenceIndex, TLink link)
        {
            if (_initChildLinkedSourceWithParentAndReferenceIndex != null) return childLinkedSource => _initChildLinkedSourceWithParentAndReferenceIndex(linkedSource, referenceIndex, childLinkedSource);

            if (_initChildLinkedSourceWithLink != null) return childLinkedSource => _initChildLinkedSourceWithLink(link, childLinkedSource);

            return null;
        }
    }
}
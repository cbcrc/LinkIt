// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.PublicApi;
using LinkIt.TopologicalSorting;

namespace LinkIt.Core.Includes
{
    /// <summary>
    /// For a nested linked source obtained from the model (configured for specific discriminant of a link target):
    /// responsible for loading and linking the link target for a specific discriminant
    /// responsible for creating the reference tree for a specific discriminant
    /// </summary>
    internal class IncludeNestedLinkedSourceFromModel<TLinkedSource, TAbstractChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel> :
        IIncludeWithCreateNestedLinkedSourceFromModel<TLinkedSource, TAbstractChildLinkedSource, TLink>,
        IIncludeWithChildLinkedSource
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
        where TChildLinkedSourceModel: class
    {
        private readonly Func<TLink, TChildLinkedSourceModel> _getNestedLinkedSourceModel;
        private readonly Action<TLink, TChildLinkedSource> _initChildLinkedSourceWithLink;
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSourceWithParentAndReferenceIndex;

        public IncludeNestedLinkedSourceFromModel(
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource
        )
        {
            _getNestedLinkedSourceModel = getNestedLinkedSourceModel;
            _initChildLinkedSourceWithParentAndReferenceIndex = initChildLinkedSource;
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public IncludeNestedLinkedSourceFromModel(
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel,
            Action<TLink, TChildLinkedSource> initChildLinkedSource
        )
        {
            _getNestedLinkedSourceModel = getNestedLinkedSourceModel;
            _initChildLinkedSourceWithLink = initChildLinkedSource;
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ChildLinkedSourceType { get; }

        public TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(
            TLink link,
            Linker linker,
            TLinkedSource linkedSource,
            int referenceIndex,
            LoadLinkProtocol loadLinkProtocol)
        {
            var childLinkSourceModel = _getNestedLinkedSourceModel(link);

            return (TAbstractChildLinkedSource) (object) linker.CreatePartiallyBuiltLinkedSource(
                childLinkSourceModel,
                CreateInitChildLinkedSourceAction(linkedSource, referenceIndex, link)
            );
        }

        public void AddDependenciesForAllLinkTargets(Dependency predecessor, LoadLinkProtocol loadLinkProtocol)
        {
            loadLinkProtocol.AddDependenciesForAllLinkTargets(ChildLinkedSourceType, predecessor);
        }

        private Action<TChildLinkedSource> CreateInitChildLinkedSourceAction(TLinkedSource linkedSource, int referenceIndex, TLink link)
        {
            if (_initChildLinkedSourceWithParentAndReferenceIndex != null)
            {
                return childLinkedSource => _initChildLinkedSourceWithParentAndReferenceIndex(linkedSource, referenceIndex, childLinkedSource);
            }

            if (_initChildLinkedSourceWithLink != null)
            {
                return childLinkedSource => _initChildLinkedSourceWithLink(link, childLinkedSource);
            }

            return null;
        }
    }
}
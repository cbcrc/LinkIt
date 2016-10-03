#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Includes
{
    //For a nested linked source obtained from the model (configured for specific discriminant of a link target):
    //responsible for loading and linking the link target for a specific discriminant
    //responsible for creating the reference tree for a specific discriminant
    public class IncludeNestedLinkedSourceFromModel<TLinkedSource,TAbstractChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel> :
        IIncludeWithCreateNestedLinkedSourceFromModel<TLinkedSource,TAbstractChildLinkedSource, TLink>,
        IIncludeWithChildLinkedSource
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TChildLinkedSourceModel> _getNestedLinkedSourceModel;
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSourceWithParentAndReferenceIndex;
        private readonly Action<TLink, TChildLinkedSource> _initChildLinkedSourceWithLink;

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

        public Type ChildLinkedSourceType { get; private set; }

        public TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(
            TLink link,
            LoadedReferenceContext loadedReferenceContext,
            TLinkedSource linkedSource,
            int referenceIndex,
            LoadLinkProtocol loadLinkProtocol)
        {
            var childLinkSourceModel = _getNestedLinkedSourceModel(link);

            return (TAbstractChildLinkedSource) (object) loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource(
                    childLinkSourceModel,
                    loadLinkProtocol,
                    CreateInitChildLinkedSourceAction(linkedSource, referenceIndex, link)
                );
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


        public void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol) {
            loadLinkProtocol.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, parent);
        }
    }
}
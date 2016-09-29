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
        private readonly Action<TLinkedSource, int, TChildLinkedSource> _initChildLinkedSource;

        public IncludeNestedLinkedSourceFromModel(
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource
        )
        {
            _getNestedLinkedSourceModel = getNestedLinkedSourceModel;
            _initChildLinkedSource = initChildLinkedSource;
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
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
                    childLinkSourceModel, 
                    loadLinkProtocol,
                    CreateInitChildLinkedSourceAction(linkedSource, referenceIndex)
                );
        }

        private Action<TChildLinkedSource> CreateInitChildLinkedSourceAction(TLinkedSource linkedSource, int referenceIndex) {
            if (_initChildLinkedSource == null) { return null; }

            return childLinkedSource => _initChildLinkedSource(linkedSource, referenceIndex, childLinkedSource);
        }


        public void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol) {
            loadLinkProtocol.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, parent);
        }
    }
}
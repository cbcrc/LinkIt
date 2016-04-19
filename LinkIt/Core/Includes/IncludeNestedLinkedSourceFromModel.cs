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
    public class IncludeNestedLinkedSourceFromModel<TAbstractChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel> : 
        IIncludeWithCreateNestedLinkedSourceFromModel<TAbstractChildLinkedSource,TLink>, 
        IIncludeWithChildLinkedSource
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TChildLinkedSourceModel> _getNestedLinkedSourceModel;

        public IncludeNestedLinkedSourceFromModel(Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel)
        {
            _getNestedLinkedSourceModel = getNestedLinkedSourceModel;
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ChildLinkedSourceType { get; private set; }

        public TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(TLink link, LoadedReferenceContext loadedReferenceContext, LoadLinkProtocol loadLinkProtocol)
        {
            var childLinkSourceModel = _getNestedLinkedSourceModel(link);

            return (TAbstractChildLinkedSource) (object) loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(childLinkSourceModel, loadLinkProtocol, null);
        }

        public void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol) {
            loadLinkProtocol.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, parent);
        }
    }
}
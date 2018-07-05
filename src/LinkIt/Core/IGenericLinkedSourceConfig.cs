// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Responsible for creating a load linker for a specific root linked source type
    /// Responsible for creating include with nested linked source
    /// </summary>
    internal interface IGenericLinkedSourceConfig<TLinkedSource> : ILinkedSourceConfig
    {
        ILoadLinker<TLinkedSource> CreateLoadLinker(Func<IReferenceLoader> createReferenceLoader,
            IEnumerable<IEnumerable<Type>> referenceTypeToBeLoadedForEachLoadingLevel,
            LoadLinkProtocol loadLinkProtocol);

        IInclude CreateIncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupId,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSource = null
        );

        IInclude CreateIncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupId,
            Action<TLink, TLinkedSource> initChildLinkedSource
        );

        IInclude CreateIncludeNestedLinkedSourceFromModel<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TChildLinkedSourceModel>(
            Expression<Func<TLink, TChildLinkedSourceModel>> getNestedLinkedSourceModel,
            ILinkTarget linkTarget,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSource = null
        );

        IInclude CreateIncludeNestedLinkedSourceFromModel<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TChildLinkedSourceModel>(
            Expression<Func<TLink, TChildLinkedSourceModel>> getNestedLinkedSourceModel,
            ILinkTarget linkTarget,
            Action<TLink, TLinkedSource> initChildLinkedSource
        );
    }
}

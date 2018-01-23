#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Interfaces
{
    /// <summary>
    /// Responsible for loading and linking a specific link target.
    /// Responsible for creating the reference trees related to a specific link target.
    /// </summary>
    internal interface ILoadLinkExpression
    {
        string LinkTargetId { get; }

        Type LinkedSourceType { get; }
        List<Type> ReferenceTypes { get; }

        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded);
        void LinkNestedLinkedSourceById(object linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked, LoadLinkProtocol loadLinkProtocol);
        void LinkNestedLinkedSourceFromModel(object linkedSource, LoadedReferenceContext loadedReferenceContext, LoadLinkProtocol loadLinkProtocol);
        void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext);
        void FilterOutNullValues(object linkedSource);

        void AddReferenceTreeForEachInclude(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol);
    }
}
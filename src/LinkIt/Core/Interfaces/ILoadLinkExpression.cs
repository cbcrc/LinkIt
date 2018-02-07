// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using LinkIt.TopologicalSorting;

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

        void AddLookupIds(object linkedSource, LoadingContext loadingContext, Type referenceTypeToBeLoaded);
        void LinkNestedLinkedSourceById(object linkedSource, Linker linker, Type referenceTypeToBeLinked, LoadLinkProtocol loadLinkProtocol);
        void LinkNestedLinkedSourceFromModel(object linkedSource, Linker linker, LoadLinkProtocol loadLinkProtocol);
        void LinkReference(object linkedSource, Linker linker);
        void FilterOutNullValues(object linkedSource);

        void AddDependencyForEachInclude(Dependency predecessor, LoadLinkProtocol loadLinkProtocol);
    }
}
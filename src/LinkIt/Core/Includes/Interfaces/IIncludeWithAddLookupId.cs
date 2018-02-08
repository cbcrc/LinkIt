// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.TopologicalSorting;

namespace LinkIt.Core.Includes.Interfaces
{
    internal interface IIncludeWithAddLookupId<TLink> : IInclude
    {
        Type ReferenceType { get; }
        void AddLookupId(TLink link, LookupContext lookupContext);
        void AddDependency(Dependency predecessor, LoadLinkProtocol loadLinkProtocol);
    }
}
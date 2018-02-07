// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;

namespace LinkIt.Core.Includes.Interfaces
{
    internal interface IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink> : IInclude
    {
        Type ReferenceType { get; }

        TAbstractChildLinkedSource CreateNestedLinkedSourceById(
            TLink link,
            Linker linker,
            TLinkedSource linkedSource,
            int referenceIndex,
            LoadLinkProtocol loadLinkProtocol
        );
    }
}
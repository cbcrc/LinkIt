#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;

namespace LinkIt.Core.Includes.Interfaces
{
    public interface IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink>:IInclude
    {
        Type ReferenceType { get; }

        TAbstractChildLinkedSource CreateNestedLinkedSourceById(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex,
            LoadLinkProtocol loadLinkProtocol
        );
        
    }
}
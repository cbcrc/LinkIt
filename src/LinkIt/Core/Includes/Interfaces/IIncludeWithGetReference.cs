// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

namespace LinkIt.Core.Includes.Interfaces
{
    internal interface IIncludeWithGetReference<TIReference, TLink>
    {
        TIReference GetReference(TLink link, DataStore dataStore);
    }
}
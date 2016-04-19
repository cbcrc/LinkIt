#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;

namespace LinkIt.PublicApi
{
    //Responsible for giving access to the lookup ids of a loading level.
    public interface ILookupIdContext
    {
        List<Type> GetReferenceTypes();
        List<TId> GetReferenceIds<TReference, TId>();
    }
}
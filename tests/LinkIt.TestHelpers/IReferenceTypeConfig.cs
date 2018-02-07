// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.PublicApi;

namespace LinkIt.TestHelpers
{
    public interface IReferenceTypeConfig
    {
        Type ReferenceType { get; }
        void Load(ILoadingContext loadingContext);
    }
}
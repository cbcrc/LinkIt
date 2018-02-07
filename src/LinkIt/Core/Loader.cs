// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    internal class Loader
    {
        private Func<IReferenceLoader> createReferenceLoader;

        public Loader(Func<IReferenceLoader> createReferenceLoader)
        {
            this.createReferenceLoader = createReferenceLoader;
        }
    }
}
#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;

namespace LinkIt.ReferenceTrees
{
    public class ReferenceToLoad{
        public ReferenceToLoad(Type referenceType, string linkTargetId)
        {
            ReferenceType = referenceType;
            LinkTargetId = linkTargetId;
        }

        public Type ReferenceType { get; private set; }
        public string LinkTargetId { get; private set; }
    }
}
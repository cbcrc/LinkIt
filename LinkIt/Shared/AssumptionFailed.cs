#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;

namespace LinkIt.Shared
{
    public class AssumptionFailed : Exception
    {
        public AssumptionFailed(string message)
            : base(message)
        {
        }
    }
}
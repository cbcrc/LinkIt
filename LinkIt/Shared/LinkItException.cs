#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;

namespace LinkIt.Shared
{
    /// <summary>
    /// Represents errors that occured within LinkIt.
    /// </summary>
    public class LinkItException : Exception
    {
        internal LinkItException(string message)
            : base(message)
        {
        }
    }
}
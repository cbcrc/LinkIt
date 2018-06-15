// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;

namespace LinkIt.Shared
{
    /// <summary>
    /// Represents errors that occured within LinkIt.
    /// </summary>
    public class LinkItException : Exception
    {
        internal LinkItException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
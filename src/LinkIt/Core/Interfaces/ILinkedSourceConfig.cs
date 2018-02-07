// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;

namespace LinkIt.Core.Interfaces
{
    /// <summary>
    /// Responsible to know the LinkedSourceModelType without using reflection every time.
    /// </summary>
    internal interface ILinkedSourceConfig
    {
        Type LinkedSourceType { get; }
        Type LinkedSourceModelType { get; }
    }
}
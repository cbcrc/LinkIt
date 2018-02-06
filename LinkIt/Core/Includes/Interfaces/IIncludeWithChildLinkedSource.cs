#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;

namespace LinkIt.Core.Includes.Interfaces
{
    internal interface IIncludeWithChildLinkedSource : IInclude
    {
        Type ChildLinkedSourceType { get; }
    }
}
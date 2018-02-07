// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;

namespace LinkIt.Core.Includes.Interfaces
{
    internal interface IIncludeWithChildLinkedSource : IInclude
    {
        Type ChildLinkedSourceType { get; }
    }
}
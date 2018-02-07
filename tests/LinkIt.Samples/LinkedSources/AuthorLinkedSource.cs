// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.PublicApi;
using LinkIt.Samples.Models;

namespace LinkIt.Samples.LinkedSources
{
    public class AuthorLinkedSource : ILinkedSource<Author>
    {
        public Image Image { get; set; }
        public Author Model { get; set; }
    }
}
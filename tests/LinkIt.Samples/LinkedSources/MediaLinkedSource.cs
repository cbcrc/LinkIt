// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using LinkIt.PublicApi;
using LinkIt.Samples.Models;

namespace LinkIt.Samples.LinkedSources
{
    public class MediaLinkedSource : ILinkedSource<Media>
    {
        public List<Tag> Tags { get; set; }
        public Media Model { get; set; }
    }
}
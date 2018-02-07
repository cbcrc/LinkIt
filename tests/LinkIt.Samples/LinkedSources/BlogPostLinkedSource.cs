// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using LinkIt.PublicApi;
using LinkIt.Samples.Models;

namespace LinkIt.Samples.LinkedSources
{
    public class BlogPostLinkedSource : ILinkedSource<BlogPost>
    {
        public List<Tag> Tags { get; set; }
        public AuthorLinkedSource Author { get; set; }
        public object MultimediaContent { get; set; }
        public BlogPost Model { get; set; }
    }
}
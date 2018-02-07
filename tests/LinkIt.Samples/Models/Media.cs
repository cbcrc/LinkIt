// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace LinkIt.Samples.Models
{
    public class Media
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> TagIds { get; set; } //Tag references
    }
}
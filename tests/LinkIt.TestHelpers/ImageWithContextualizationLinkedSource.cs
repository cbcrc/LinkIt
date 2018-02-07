// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.PublicApi;

namespace LinkIt.TestHelpers
{
    public class ImageWithContextualizationLinkedSource : IPolymorphicSource, ILinkedSource<Image>
    {
        public ContentContextualization ContentContextualization { get; set; }
        public Image Model { get; set; }
    }
}
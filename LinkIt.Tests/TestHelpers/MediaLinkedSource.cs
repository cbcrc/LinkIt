#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using LinkIt.PublicApi;

namespace LinkIt.Tests.TestHelpers
{
    public class MediaLinkedSource : ILinkedSource<Media> {
        public Media Model { get; set; }
        public Image SummaryImage { get; set; }
    }
}
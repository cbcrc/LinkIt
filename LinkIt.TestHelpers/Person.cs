#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace LinkIt.TestHelpers
{
    public class Person:IPolymorphicModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SummaryImageId { get; set; }
    }
}
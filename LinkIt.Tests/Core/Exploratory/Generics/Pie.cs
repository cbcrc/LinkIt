#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace LinkIt.Tests.Core.Exploratory.Generics
{
    public class Pie<T>
    {
        public Pie(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public string PieContent
        {
            get { return typeof (T).Name; }
        }
    }
}
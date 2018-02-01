#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace LinkIt.ConfigBuilders
{
    /// <summary>
    /// Interface for classes to configure a LoadLinkProtocol.
    /// </summary>
    public interface ILoadLinkProtocolConfig
    {
        /// <summary>
        /// Configure the LoadLinkProtocol using a builder
        /// </summary>
        void ConfigureLoadLinkProtocol(LoadLinkProtocolBuilder loadLinkProtocolBuilder);
    }
}
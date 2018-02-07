// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Responsible for creating a load linker for any root linked source type
    /// </summary>
    public interface ILoadLinkProtocol
    {
        ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>();
        LoadLinkProtocolStatistics Statistics { get; }
    }
}
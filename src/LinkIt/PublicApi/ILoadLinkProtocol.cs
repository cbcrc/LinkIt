// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Responsible for creating a load linker for any root linked source type
    /// </summary>
    public interface ILoadLinkProtocol
    {
        /// <summary>
        /// Create a load linker for a linked source type.
        /// </summary>
        ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>();

        /// <summary>
        /// Load a model by ID.
        /// </summary>
        IDataLoader<TModel> Load<TModel>();

        /// <summary>
        /// Stats for the <see cref="ILoadLinkProtocol"/>.
        /// </summary>
        LoadLinkProtocolStatistics Statistics { get; }
    }
}

// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Parent interface of <see cref="ILinkedSource{TModel}"/>.
    /// DO NOT implement directly, use <see cref="ILinkedSource{TModel}"/>.
    /// Exists only to help with intellisense and compilation.
    /// </summary>
    public interface ILinkedSource
    { }

    /// <summary>
    /// Responsible for defining the link targets of a model
    /// </summary>
    public interface ILinkedSource<TModel> : ILinkedSource
    {
        /// <summary>
        /// Model for the linked source
        /// </summary>
        TModel Model { get; set; }
    }
}
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;
using LinkIt.Shared;

namespace LinkIt.Conventions
{
    /// <summary>
    /// Extensions to build a <see cref="ILoadLinkProtocol"/> more easily.
    /// </summary>
    public static class LoadLinkProtocolBuilderExtensions
    {
        /// <summary>
        /// Build a new <see cref="ILoadLinkProtocol"/> using assemblies as an entry point
        /// to find <see cref="ILoadLinkProtocolConfig"/> and types for which to apply the <paramref name="conventions"/>.
        /// </summary>
        public static ILoadLinkProtocol Build(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            Func<IReferenceLoader> createReferenceLoader,
            IEnumerable<Assembly> assemblies,
            IList<ILoadLinkExpressionConvention> conventions)
        {
            loadLinkProtocolBuilder.ApplyConventions(
                assemblies,
                conventions
            );
            loadLinkProtocolBuilder.ApplyLoadLinkProtocolConfigs(assemblies);

            return loadLinkProtocolBuilder.Build(createReferenceLoader);
        }

        /// <summary>
        /// Build a new <see cref="ILoadLinkProtocol"/> using assemblies as an entry point
        /// to find <see cref="ILoadLinkProtocolConfig"/> and types for which to apply the <paramref name="conventions"/>.
        /// </summary>
        public static void ApplyConventions(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            IEnumerable<Assembly> assemblies,
            IList<ILoadLinkExpressionConvention> conventions)
        {
            if (loadLinkProtocolBuilder is null)
            {
                throw new ArgumentNullException(nameof(loadLinkProtocolBuilder));
            }

            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (conventions is null)
            {
                throw new ArgumentNullException(nameof(conventions));
            }

            var types = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .ToList();

            ApplyConventions(
                loadLinkProtocolBuilder,
                types,
                conventions
            );
        }

        /// <summary>
        /// Build a new <see cref="ILoadLinkProtocol"/> and apply the <paramref name="conventions"/> to the specified <paramref name="types"/>.
        /// </summary>
        public static void ApplyConventions(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            IList<Type> types,
            IList<ILoadLinkExpressionConvention> conventions)
        {
            if (loadLinkProtocolBuilder is null)
            {
                throw new ArgumentNullException(nameof(loadLinkProtocolBuilder));
            }

            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (conventions is null)
            {
                throw new ArgumentNullException(nameof(conventions));
            }

            EnsureConventionNamesAreUnique(conventions);

            var matches = new FindAllConventionMatchesQuery(types, conventions).Execute();
            var command = new ApplyLoadLinkConventionCommand(loadLinkProtocolBuilder, matches);
            command.Execute();
        }

        private static void EnsureConventionNamesAreUnique(IList<ILoadLinkExpressionConvention> conventions)
        {
            var notUniqueConventionNames = conventions.GetNotUniqueKey(convention => convention.Name);

            if (notUniqueConventionNames.Count > 0)
            {
                throw new LinkItException(
                   $"Cannot have more than one convention with the same name: {string.Join(",", notUniqueConventionNames)}"
               );
            }
        }
    }
}

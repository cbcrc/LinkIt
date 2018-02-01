#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

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
    public static class LoadLinkProtocolBuilderExtensions
    {
        public static ILoadLinkProtocol Build(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            Func<IReferenceLoader> createReferenceLoader,
            IEnumerable<Assembly> assemblies,
            List<ILoadLinkExpressionConvention> conventions)
        {
            loadLinkProtocolBuilder.ApplyConventions(
                assemblies,
                conventions
            );
            loadLinkProtocolBuilder.ApplyLoadLinkProtocolConfigs(assemblies);

            return loadLinkProtocolBuilder.Build(createReferenceLoader);
        }

        public static void ApplyConventions(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            IEnumerable<Assembly> assemblies,
            List<ILoadLinkExpressionConvention> conventions)
        {
            if (loadLinkProtocolBuilder == null) throw new ArgumentNullException(nameof(loadLinkProtocolBuilder));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            if (conventions == null) throw new ArgumentNullException(nameof(conventions));

            var types = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .ToList();

            ApplyConventions(
                loadLinkProtocolBuilder,
                types,
                conventions
            );
        }

        public static void ApplyConventions(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            List<Type> types,
            List<ILoadLinkExpressionConvention> conventions)
        {
            if (loadLinkProtocolBuilder == null) throw new ArgumentNullException(nameof(loadLinkProtocolBuilder));
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (conventions == null) throw new ArgumentNullException(nameof(conventions));
            EnsureConventionNamesAreUnique(conventions);

            var matches = new FindAllConventionMatchesQuery(types, conventions).Execute();
            var command = new ApplyLoadLinkConventionCommand(loadLinkProtocolBuilder, matches);
            command.Execute();
        }

        private static void EnsureConventionNamesAreUnique(List<ILoadLinkExpressionConvention> conventions)
        {
            var notUniqueConventionNames = conventions.GetNotUniqueKey(convention => convention.Name);

            if (notUniqueConventionNames.Any())
                throw new ArgumentException(
                    $"Cannot have many conventions with the same name: {string.Join(",", notUniqueConventionNames)}"
                );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions
{
    public static class LoadLinkProtocolBuilderExtensions
    {
        public static void ApplyConventions(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            IEnumerable<Assembly> assemblies,
            List<ILoadLinkExpressionConvention> conventions)
        {
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
            var matches = new FindAllConventionMatchesQuery(types, conventions).Execute();
            var command = new ApplyLoadLinkConventionCommand(loadLinkProtocolBuilder, matches);
            command.Execute();
        }

        public static List<ILoadLinkExpressionConvention> GetDefaultConventions(this LoadLinkProtocolBuilder loadLinkProtocolBuilder) {
            return new List<ILoadLinkExpressionConvention>{
                //stle: todo
            };
        }

    }
}

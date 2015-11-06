using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeterogeneousDataSource.Conventions.DefaultConventions;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions
{
    public static class LoadLinkProtocolBuilderExtensions
    {
        public static List<ILoadLinkExpressionConvention> GetDefaultConventions(this LoadLinkProtocolBuilder loadLinkProtocolBuilder) {
            return new List<ILoadLinkExpressionConvention>{
                new LoadLinkByNullableValueTypeIdWhenIdSuffixMatches(),
                new LoadLinkMultiValueWhenIdSuffixMatches(),
                new LoadLinkSingleValueWhenIdSuffixMatches(),
                new LoadLinkMultiValueSubLinkedSourceWhenNameMatches(),
                new LoadLinkSingleValueSubLinkedSourceWhenNameMatches(),
            };
        }

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
            EnsureConventionNamesAreUnique(conventions);

            var matches = new FindAllConventionMatchesQuery(types, conventions).Execute();
            var command = new ApplyLoadLinkConventionCommand(loadLinkProtocolBuilder, matches);
            command.Execute();
        }

        private static void EnsureConventionNamesAreUnique(List<ILoadLinkExpressionConvention> conventions) {
            var notUniqueConventionNames = conventions.GetNotUniqueKey(convention => convention.Name);

            if (notUniqueConventionNames.Any()) {
                throw new ArgumentException(
                    string.Format(
                        "Cannot have many conventions with the same name: {0}",
                        String.Join(",", notUniqueConventionNames)
                    )
                );
            }
        }
    }
}

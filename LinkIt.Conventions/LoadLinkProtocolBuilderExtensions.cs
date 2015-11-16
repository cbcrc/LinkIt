using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.Conventions.Interfaces;
using LinkIt.Shared;

namespace LinkIt.Conventions
{
    public static class LoadLinkProtocolBuilderExtensions
    {
        public static List<ILoadLinkExpressionConvention> GetDefaultConventions(this LoadLinkProtocolBuilder loadLinkProtocolBuilder) {
            return new List<ILoadLinkExpressionConvention>{
                new LoadLinkByNullableValueTypeIdWhenIdSuffixMatches(),
                new LoadLinkMultiValueWhenIdSuffixMatches(),
                new LoadLinkSingleValueWhenIdSuffixMatches(),
                new LoadLinkMultiValueNestedLinkedSourceFromModelWhenNameMatches(),
                new LoadLinkSingleValueNestedLinkedSourceFromModelWhenNameMatches(),
            };
        }

        public static void ApplyConventions(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            IEnumerable<Assembly> assemblies,
            List<ILoadLinkExpressionConvention> conventions)
        {
            if (loadLinkProtocolBuilder == null) { throw new ArgumentNullException("loadLinkProtocolBuilder"); }
            if (assemblies == null) { throw new ArgumentNullException("assemblies"); }
            if (conventions == null) { throw new ArgumentNullException("conventions"); }

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
            if (loadLinkProtocolBuilder == null) { throw new ArgumentNullException("loadLinkProtocolBuilder"); }
            if (types == null) { throw new ArgumentNullException("types"); }
            if (conventions == null) { throw new ArgumentNullException("conventions"); }
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

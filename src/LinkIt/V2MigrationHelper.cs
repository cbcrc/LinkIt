using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinkIt.PublicApi;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace LinkIt.PublicApi
{
    /// <summary>
    /// Helper to migrate from v1 to v2.
    /// Contains extension methods of deprected methods, with the <see cref="ObsoleteAttribute"/> explaining what to change.
    /// </summary>
    public static class V2LoadingContextMigrationHelper
    {
        [Obsolete("Deprecated. Use ReferenceTypes property.", true)]
        public static void GetReferenceTypes(this ILoadingContext context)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Deprecated. Use AddResults.", true)]
        public static void AddReferences<TReference, TId>(this ILoadingContext context, IEnumerable<TReference> references, Func<TReference, TId> getReferenceId)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Deprecated. Use AddResults.", true)]
        public static void AddReferences<TReference, TId>(this ILoadingContext context, IDictionary<TId, TReference> referencesById)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Deprecated. Use ReferenceIds.", true)]
        public static void GetReferenceIds<TId>(this ILoadingContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Obsolete("Deprecated. Use ILoadingContext.", true)]
    public abstract class ILoadedReferenceContext
    {}

    [Obsolete("Deprecated. Use ILoadingContext.", true)]
    public abstract class ILookupIdContext
    {}
}

namespace LinkIt.ConfigBuilders
{
    public static class V2LoadLinkProtocolForLinkedSourceBuilderMigrationHelper
    {
        [Obsolete("Deprecated. Use LoadLinkReferencesByIds (pluralized method name).", true)]
        public static LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TLinkedSource, TReference, TId>(
            this LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> builder,
            Func<TLinkedSource, TId?> getOptionalLookupId,
            Expression<Func<TLinkedSource, TReference>> getLinkTarget
        )
            where TId:struct where TLinkedSource : ILinkedSource
        {
            throw new NotImplementedException();
        }

        [Obsolete("Deprecated. Use LoadLinkNestedLinkedSourcesByIds (pluralized method name).", true)]
        public static LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TLinkedSource, TChildLinkedSource, TId>(
            this LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> builder,
            Func<TLinkedSource, List<TId>> getLookupIds,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> getLinkTarget,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource)
            where TLinkedSource : ILinkedSource
        {
            throw new NotImplementedException();
        }

        [Obsolete("Deprecated. Use LoadLinkNestedLinkedSourcesFromModels (pluralized method name).", true)]
        public static LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel>(
            this LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> builder,
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getNestedLinkedSourceModels,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> getLinkTarget
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
            where TLinkedSource : ILinkedSource
        {
            throw new NotImplementedException();
        }

        [Obsolete("Deprecated. Use LoadLinkPolymorphic.", true)]
        public static LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(
            this LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> builder,
            Func<TLinkedSource, TLink> getLink,
            Expression<Func<TLinkedSource, TAbstractLinkTarget>> getLinkTarget,
            Func<TLink, TDiscriminant> getDiscriminant,
            Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes)
            where TLinkedSource : ILinkedSource
        {
            throw new NotImplementedException();
        }

        [Obsolete("Deprecated. Use LoadLinkPolymorphicList.", true)]
        public static LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLinkForList<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(
            this LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> builder,
            Func<TLinkedSource, List<TLink>> getLinks,
            Expression<Func<TLinkedSource, List<TAbstractLinkTarget>>> getLinkTarget,
            Func<TLink, TDiscriminant> getDiscriminant,
            Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes)
            where TLinkedSource : ILinkedSource
        {
            throw new NotImplementedException();
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

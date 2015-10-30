using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources
{
    public class LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>
    {
        private readonly Action<ILoadLinkExpression> _addLoadLinkExpressionAction;

        public LoadLinkProtocolForLinkedSourceBuilder(Action<ILoadLinkExpression> addLoadLinkExpressionAction)
        {
            _addLoadLinkExpressionAction = addLoadLinkExpressionAction;
        }

        #region Reference
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TReference, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TReference>> linkTargetFunc)
        {
            return LoadLinkReference(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TReference, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TReference>>> linkTargetFunc)
        {
            return LoadLinkReference(
                LinkTargetFactory.Create(linkTargetFunc),
                getLookupIdsFunc 
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TReference, TId>(
           Func<TLinkedSource, TId?> getOptionalLookupIdFunc,
           Expression<Func<TLinkedSource, TReference>> linkTargetFunc
        )
            where TId:struct 
        {
            return LoadLinkReference(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForOptionalSingleValue(getOptionalLookupIdFunc)
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TTargetProperty, TId>(
            LinkTargetBase<TLinkedSource, TTargetProperty> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc) 
        {
            return AddNonPolymorphicLoadLinkExpression(
                linkTarget,
                getLookupIdsFunc,
                new ReferenceInclude<TTargetProperty, TId, TTargetProperty, TId>(
                    CreateIdentityFunc<TId>()
                )
            );
        }
        #endregion

        #region NestedLinkedSource
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc) 
        {
            return LoadLinkNestedLinkedSource(
                getLookupIdFunc,
                linkTargetFunc,
                NullInitChildLinkedSourceActionForSingleValue
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc,
           Action<TLinkedSource, TChildLinkedSource> initChildLinkedSourceAction) 
        {
            return LoadLinkNestedLinkedSource(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                (linkedSource, referenceIndex, childLinkedSource) =>
                    initChildLinkedSourceAction(linkedSource, childLinkedSource)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc)
        {
            return LoadLinkNestedLinkedSource(
                getLookupIdsFunc, 
                linkTargetFunc, 
                NullInitChildLinkedSourceAction
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction)
        {
            return LoadLinkNestedLinkedSource(
                LinkTargetFactory.Create(linkTargetFunc),
                getLookupIdsFunc,
                initChildLinkedSourceAction
            );
        }
        
        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TTargetProperty, TId>(
            LinkTargetBase<TLinkedSource, TTargetProperty> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Action<TLinkedSource, int, TTargetProperty> initChildLinkedSourceAction)
        {
            var include = LinkedSourceConfigs.GetConfigFor<TTargetProperty>()
                .CreateNestedLinkedSourceInclude<TLinkedSource, TTargetProperty, TId, TId>(
                    CreateIdentityFunc<TId>(),
                    initChildLinkedSourceAction
                );

            return AddNonPolymorphicLoadLinkExpression(
                linkTarget,
                getLookupIdsFunc,
                include
            );
        }

        private void NullInitChildLinkedSourceActionForSingleValue<TChildLinkedSource>(
            TLinkedSource linkedsource,
            TChildLinkedSource childLinkedSource) 
        {
            //using null causes problem with generic type inference, 
            //using a special value work around this limitation of generics
        }

        private void NullInitChildLinkedSourceAction<TChildLinkedSource>(
            TLinkedSource linkedsource,
            int referenceIndex,
            TChildLinkedSource childLinkedSource) {
            //using null causes problem with generic type inference, 
            //using a special value work around this limitation of generics
        }
        #endregion

        #region SubLinkedSource
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkSubLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, TChildLinkedSourceModel> getSubLinkedSourceModelsFunc,
            Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            return LoadLinkSubLinkedSource(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForSingleValue(getSubLinkedSourceModelsFunc)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkSubLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getSubLinkedSourceModelsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
        {
            return LoadLinkSubLinkedSource(
                LinkTargetFactory.Create(linkTargetFunc),
                getSubLinkedSourceModelsFunc
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkSubLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
            LinkTargetBase<TLinkedSource, TChildLinkedSource> linkTarget,
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getSubLinkedSourceModelsFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            return AddNonPolymorphicLoadLinkExpression(
                linkTarget,
                getSubLinkedSourceModelsFunc,
                new SubLinkedSourceInclude<TChildLinkedSource, TChildLinkedSourceModel, TChildLinkedSource, TChildLinkedSourceModel>(
                    null
                )
            );
        }

        #endregion

        #region Polymorphic
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TIChildLinkedSource, TLink, TDiscriminant>(
           Func<TLinkedSource, TLink> getLinkFunc,
           Expression<Func<TLinkedSource, TIChildLinkedSource>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes) 
        {
            return PolymorphicLoadLink(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForSingleValue(getLinkFunc),
                getDiscriminantFunc,
                includes
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLinkForList<TIChildLinkedSource, TLink, TDiscriminant>(
           Func<TLinkedSource, List<TLink>> getLinksFunc,
           Expression<Func<TLinkedSource, List<TIChildLinkedSource>>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes)
        {
            return PolymorphicLoadLink(
                LinkTargetFactory.Create(linkTargetFunc),
                getLinksFunc,
                getDiscriminantFunc,
                includes
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TTargetProperty, TLink, TDiscriminant>(
            LinkTargetBase<TLinkedSource, TTargetProperty> linkTarget,
            Func<TLinkedSource, List<TLink>> getLinksFunc,
            Func<TLink, TDiscriminant> getDiscriminantFunc,
            Action<IncludeTargetConcreteTypeBuilder<TLinkedSource, TTargetProperty, TLink, TDiscriminant>> includes) 
        {
            var includeBuilder = new IncludeTargetConcreteTypeBuilder<TLinkedSource, TTargetProperty, TLink, TDiscriminant>();
            includes(includeBuilder);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TTargetProperty, TLink, TDiscriminant>(
                linkTarget,
                getLinksFunc,
                getDiscriminantFunc,
                includeBuilder.Build()
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }
        #endregion

        #region Shared
        private static Func<TLinkedSource, List<TId>> ToGetLookupIdsFuncForSingleValue<TId>(
            Func<TLinkedSource, TId> getLookupIdFunc) 
        {
            return linkedSource => new List<TId> { getLookupIdFunc(linkedSource) };
        }

        private static Func<TLinkedSource, List<TId>> ToGetLookupIdsFuncForOptionalSingleValue<TId>(
            Func<TLinkedSource, TId?> getOptionalLookupIdFunc
        ) 
            where TId:struct
        {
            return linkedSource => OptionalIdToList(getOptionalLookupIdFunc(linkedSource));
        }

        private static List<TId> OptionalIdToList<TId>(TId? optionalId)
            where TId : struct
        {
            return optionalId.HasValue
                ? new List<TId> { optionalId.Value }
                : new List<TId>();
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> AddLoadLinkExpression(
           ILoadLinkExpression loadLinkExpression) {
            _addLoadLinkExpressionAction(loadLinkExpression);
            return this;
        }

        public static Func<T, T> CreateIdentityFunc<T>() {
            return x => x;
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> AddNonPolymorphicLoadLinkExpression<TTargetProperty, TId>(
            LinkTargetBase<TLinkedSource, TTargetProperty> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            IInclude include) {
            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TTargetProperty, TId, bool>(
                linkTarget,
                getLookupIdsFunc,
                link => true,
                new Dictionary<bool, IInclude>
                {
                    {
                        true, //always one include when not polymorphic
                        include
                    }
                }
            );

            return AddLoadLinkExpression(loadLinkExpression);

        }
        #endregion
    }
}
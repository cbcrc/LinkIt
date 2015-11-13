﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinkIt.LinkedSources;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.LinkTargets;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.LoadLinkExpressions;
using LinkIt.LoadLinkExpressions.Includes;
using LinkIt.LoadLinkExpressions.Includes.Interfaces;

namespace LinkIt.ConfigBuilders
{
    public class LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>
    {
        private readonly Action<ILoadLinkExpression> _addLoadLinkExpressionAction;

        public LoadLinkProtocolForLinkedSourceBuilder(Action<ILoadLinkExpression> addLoadLinkExpressionAction)
        {
            _addLoadLinkExpressionAction = addLoadLinkExpressionAction;
        }

        #region ReferenceById
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TReference>> linkTargetFunc)
        {
            return LoadLinkReferenceById(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TReference>>> linkTargetFunc)
        {
            return LoadLinkReferenceById(
                LinkTargetFactory.Create(linkTargetFunc),
                getLookupIdsFunc 
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
           Func<TLinkedSource, TId?> getOptionalLookupIdFunc,
           Expression<Func<TLinkedSource, TReference>> linkTargetFunc
        )
            where TId:struct 
        {
            return LoadLinkReferenceById(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForOptionalSingleValue(getOptionalLookupIdFunc)
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
            ILinkTarget<TLinkedSource, TReference> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc) 
        {
            return AddNonPolymorphicLoadLinkExpression(
                linkTarget,
                getLookupIdsFunc,
                new IncludeReferenceById<TReference, TId, TReference, TId>(
                    CreateIdentityFunc<TId>()
                )
            );
        }
        #endregion

        #region NestedLinkedSourceById
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc) 
        {
            return LoadLinkNestedLinkedSourceById(
                getLookupIdFunc,
                linkTargetFunc,
                NullInitChildLinkedSourceActionForSingleValue
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc,
           Action<TLinkedSource, TChildLinkedSource> initChildLinkedSourceAction) 
        {
            return LoadLinkNestedLinkedSourceById(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                (linkedSource, referenceIndex, childLinkedSource) =>
                    initChildLinkedSourceAction(linkedSource, childLinkedSource)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId?> getOptionalLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc
        )
            where TId : struct
        {
            return LoadLinkNestedLinkedSourceById(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForOptionalSingleValue(getOptionalLookupIdFunc),
                NullInitChildLinkedSourceAction
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId?> getOptionalLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc,
           Action<TLinkedSource, TChildLinkedSource> initChildLinkedSourceAction
        )
            where TId : struct 
        {
            return LoadLinkNestedLinkedSourceById(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForOptionalSingleValue(getOptionalLookupIdFunc),
                (linkedSource, referenceIndex, childLinkedSource) =>
                    initChildLinkedSourceAction(linkedSource, childLinkedSource)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc)
        {
            return LoadLinkNestedLinkedSourceById(
                getLookupIdsFunc, 
                linkTargetFunc, 
                NullInitChildLinkedSourceAction
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction)
        {
            return LoadLinkNestedLinkedSourceById(
                LinkTargetFactory.Create(linkTargetFunc),
                getLookupIdsFunc,
                initChildLinkedSourceAction
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            ILinkTarget<TLinkedSource, TChildLinkedSource> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction)
        {
            var include = LinkedSourceConfigs.GetConfigFor<TChildLinkedSource>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TChildLinkedSource, TId, TId>(
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

        #region NestedLinkedSourceFromModel
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, TChildLinkedSourceModel> getNestedLinkedSourceModel,
            Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            return LoadLinkNestedLinkedSourceFromModel(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForSingleValue(getNestedLinkedSourceModel)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getNestedLinkedSourceModel,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
        {
            return LoadLinkNestedLinkedSourceFromModel(
                LinkTargetFactory.Create(linkTargetFunc),
                getNestedLinkedSourceModel
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel>(
            ILinkTarget<TLinkedSource, TChildLinkedSource> linkTarget,
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getNestedLinkedSourceModel
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            return AddNonPolymorphicLoadLinkExpression(
                linkTarget,
                getNestedLinkedSourceModel,
                new IncludeNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel, TChildLinkedSource, TChildLinkedSourceModel>(
                    CreateIdentityFunc<TChildLinkedSourceModel>()
                )
            );
        }

        #endregion

        #region Polymorphic
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TAbstractLinkTarget, TLink, TDiscriminant>(
           Func<TLinkedSource, TLink> getLinkFunc,
           Expression<Func<TLinkedSource, TAbstractLinkTarget>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes) 
        {
            return PolymorphicLoadLink(
                LinkTargetFactory.Create(linkTargetFunc),
                ToGetLookupIdsFuncForSingleValue(getLinkFunc),
                getDiscriminantFunc,
                includes
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLinkForList<TAbstractLinkTarget, TLink, TDiscriminant>(
           Func<TLinkedSource, List<TLink>> getLinksFunc,
           Expression<Func<TLinkedSource, List<TAbstractLinkTarget>>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes)
        {
            return PolymorphicLoadLink(
                LinkTargetFactory.Create(linkTargetFunc),
                getLinksFunc,
                getDiscriminantFunc,
                includes
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TAbstractLinkTarget, TLink, TDiscriminant>(
            ILinkTarget<TLinkedSource, TAbstractLinkTarget> linkTarget,
            Func<TLinkedSource, List<TLink>> getLinksFunc,
            Func<TLink, TDiscriminant> getDiscriminantFunc,
            Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes) 
        {
            var includeBuilder = new IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(linkTarget);
            includes(includeBuilder);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(
                linkTarget,
                getLinksFunc,
                includeBuilder.Build(getDiscriminantFunc)
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
            ILinkTarget<TLinkedSource, TTargetProperty> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            IInclude include) {
            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TTargetProperty, TId, bool>(
                linkTarget,
                getLookupIdsFunc,
                new IncludeSet<TLinkedSource, TTargetProperty, TId, bool>(
                    new Dictionary<bool, IInclude>
                    {
                        {
                            true, //always one include when not polymorphic
                            include
                        }
                    },
                    link => true
                )
            );

            return AddLoadLinkExpression(loadLinkExpression);

        }
        #endregion
    }
}
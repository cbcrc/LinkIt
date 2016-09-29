#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinkIt.Core;
using LinkIt.Core.Includes;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.Core.Interfaces;
using LinkIt.LinkTargets;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.PublicApi;

namespace LinkIt.ConfigBuilders
{
    public class LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>
    {
        private readonly Action<ILoadLinkExpression> _addLoadLinkExpression;

        public LoadLinkProtocolForLinkedSourceBuilder(Action<ILoadLinkExpression> addLoadLinkExpression)
        {
            _addLoadLinkExpression = addLoadLinkExpression;
        }

        #region ReferenceById
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
           Func<TLinkedSource, TId> getLookupId,
           Expression<Func<TLinkedSource, TReference>> getLinkTarget)
        {
            if (getLookupId == null) { throw new ArgumentNullException("getLookupId"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkReferenceById(
                ToGetLookupIdsForSingleValue(getLookupId), 
                LinkTargetFactory.Create(getLinkTarget)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
            Func<TLinkedSource, List<TId>> getLookupIds,
            Expression<Func<TLinkedSource, List<TReference>>> getLinkTarget)
        {
            if (getLookupIds == null) { throw new ArgumentNullException("getLookupIds"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkReferenceById(
                getLookupIds, 
                LinkTargetFactory.Create(getLinkTarget)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
           Func<TLinkedSource, TId?> getOptionalLookupId,
           Expression<Func<TLinkedSource, TReference>> getLinkTarget
        )
            where TId:struct 
        {
            if (getOptionalLookupId == null) { throw new ArgumentNullException("getOptionalLookupId"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkReferenceById(
                ToGetLookupIdsForOptionalSingleValue(getOptionalLookupId), 
                LinkTargetFactory.Create(getLinkTarget)
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
            Func<TLinkedSource, List<TId>> getLookupIds, 
            ILinkTarget<TLinkedSource, TReference> linkTarget) 
        {
            return AddNonPolymorphicLoadLinkExpression(
                getLookupIds,
                linkTarget, 
                new IncludeReferenceById<TReference, TId, TReference, TId>(
                    CreateIdentityFunc<TId>()
                )
            );
        }
        #endregion

        #region NestedLinkedSourceById
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId> getLookupId,
           Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget) 
        {
            if (getLookupId == null) { throw new ArgumentNullException("getLookupId"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkNestedLinkedSourceById(
                getLookupId,
                getLinkTarget,
                NullInitChildLinkedSourceForSingleValue
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId> getLookupId,
           Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget,
           Action<TLinkedSource, TChildLinkedSource> initChildLinkedSource) 
        {
            if (getLookupId == null) { throw new ArgumentNullException("getLookupId"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }
            if (initChildLinkedSource == null) { throw new ArgumentNullException("initChildLinkedSource"); }

            return LoadLinkNestedLinkedSourceById(
                ToGetLookupIdsForSingleValue(getLookupId),
                LinkTargetFactory.Create(getLinkTarget), 
                (linkedSource, referenceIndex, childLinkedSource) =>
                    initChildLinkedSource(linkedSource, childLinkedSource)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId?> getOptionalLookupId,
           Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget
        )
            where TId : struct
        {
            if (getOptionalLookupId == null) { throw new ArgumentNullException("getOptionalLookupId"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkNestedLinkedSourceById(
                ToGetLookupIdsForOptionalSingleValue(getOptionalLookupId),
                LinkTargetFactory.Create(getLinkTarget), 
                NullInitChildLinkedSource
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId?> getOptionalLookupId,
           Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget,
           Action<TLinkedSource, TChildLinkedSource> initChildLinkedSource
        )
            where TId : struct 
        {
            if (getOptionalLookupId == null) { throw new ArgumentNullException("getOptionalLookupId"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }
            if (initChildLinkedSource == null) { throw new ArgumentNullException("initChildLinkedSource"); }

            return LoadLinkNestedLinkedSourceById(
                ToGetLookupIdsForOptionalSingleValue(getOptionalLookupId),
                LinkTargetFactory.Create(getLinkTarget), 
                (linkedSource, referenceIndex, childLinkedSource) =>
                    initChildLinkedSource(linkedSource, childLinkedSource)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIds,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> getLinkTarget)
        {
            if (getLookupIds == null) { throw new ArgumentNullException("getLookupIds"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkNestedLinkedSourceById(
                getLookupIds, 
                getLinkTarget, 
                NullInitChildLinkedSource
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIds,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> getLinkTarget,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource)
        {
            if (getLookupIds == null) { throw new ArgumentNullException("getLookupIds"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }
            if (initChildLinkedSource == null) { throw new ArgumentNullException("initChildLinkedSource"); }

            return LoadLinkNestedLinkedSourceById(
                getLookupIds,
                LinkTargetFactory.Create(getLinkTarget), 
                initChildLinkedSource
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIds, 
            ILinkTarget<TLinkedSource, TChildLinkedSource> linkTarget, 
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource)
        {
            var include = LinkedSourceConfigs.GetConfigFor<TChildLinkedSource>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TChildLinkedSource, TId, TId>(
                    CreateIdentityFunc<TId>(),
                    initChildLinkedSource
                );

            return AddNonPolymorphicLoadLinkExpression(
                getLookupIds,
                linkTarget, 
                include
            );
        }

        private void NullInitChildLinkedSourceForSingleValue<TChildLinkedSource>(
            TLinkedSource linkedsource,
            TChildLinkedSource childLinkedSource) 
        {
            //using null causes problem with generic type inference, 
            //using a special value work around this limitation of generics
        }

        private void NullInitChildLinkedSource<TChildLinkedSource>(
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
            Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            if (getNestedLinkedSourceModel == null) { throw new ArgumentNullException("getNestedLinkedSourceModel"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkNestedLinkedSourceFromModel(
                getNestedLinkedSourceModel, 
                getLinkTarget,
                NullInitChildLinkedSourceForSingleValue
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, TChildLinkedSourceModel> getNestedLinkedSourceModel,
            Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget,
            Action<TLinkedSource, TChildLinkedSource> initChildLinkedSource
        ) where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            if (getNestedLinkedSourceModel == null) { throw new ArgumentNullException("getNestedLinkedSourceModel"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }
            if (initChildLinkedSource == null) { throw new ArgumentNullException("initChildLinkedSource"); }

            return LoadLinkNestedLinkedSourceFromModel(
                ToGetLookupIdsForSingleValue(getNestedLinkedSourceModel),
                LinkTargetFactory.Create(getLinkTarget),
                (linkedSource, referenceIndex, childLinkedSource) =>
                    initChildLinkedSource(linkedSource, childLinkedSource)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getNestedLinkedSourceModels,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> getLinkTarget
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
        {
            if (getNestedLinkedSourceModels == null) { throw new ArgumentNullException("getNestedLinkedSourceModels"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkNestedLinkedSourceFromModel(
                getNestedLinkedSourceModels, 
                getLinkTarget,
                NullInitChildLinkedSource
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel>(
           Func<TLinkedSource, List<TChildLinkedSourceModel>> getNestedLinkedSourceModels,
           Expression<Func<TLinkedSource, List<TChildLinkedSource>>> getLinkTarget,
           Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource
       )
           where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() {
            if (getNestedLinkedSourceModels == null) { throw new ArgumentNullException("getNestedLinkedSourceModels"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }
            if (initChildLinkedSource == null) { throw new ArgumentNullException("initChildLinkedSource"); }

            return LoadLinkNestedLinkedSourceFromModel(
                getNestedLinkedSourceModels,
                LinkTargetFactory.Create(getLinkTarget),
                initChildLinkedSource
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getNestedLinkedSourceModel, 
            ILinkTarget<TLinkedSource, TChildLinkedSource> linkTarget,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            var include = new IncludeNestedLinkedSourceFromModel<TLinkedSource,TChildLinkedSource, TChildLinkedSourceModel, TChildLinkedSource, TChildLinkedSourceModel>(
                    CreateIdentityFunc<TChildLinkedSourceModel>(),
                    initChildLinkedSource
            );

            return AddNonPolymorphicLoadLinkExpression(
                getNestedLinkedSourceModel,
                linkTarget, 
                include
            );
        }

        #endregion

        #region Polymorphic
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TAbstractLinkTarget, TLink, TDiscriminant>(
           Func<TLinkedSource, TLink> getLink,
           Expression<Func<TLinkedSource, TAbstractLinkTarget>> getLinkTarget,
           Func<TLink, TDiscriminant> getDiscriminant,
           Action<IncludeSetBuilder<TLinkedSource, 
           TAbstractLinkTarget, TLink, TDiscriminant>> includes) 
        {
            if (getLink == null) { throw new ArgumentNullException("getLink"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }
            if (getDiscriminant == null) { throw new ArgumentNullException("getDiscriminant"); }
            if (includes == null) { throw new ArgumentNullException("includes"); }

            return PolymorphicLoadLink(
                ToGetLookupIdsForSingleValue(getLink),
                LinkTargetFactory.Create(getLinkTarget),
                getDiscriminant, 
                includes
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLinkForList<TAbstractLinkTarget, TLink, TDiscriminant>(
           Func<TLinkedSource, List<TLink>> getLinks,
           Expression<Func<TLinkedSource, List<TAbstractLinkTarget>>> getLinkTarget,
           Func<TLink, TDiscriminant> getDiscriminant,
           Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes)
        {
            if (getLinks == null) { throw new ArgumentNullException("getLinks"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }
            if (getDiscriminant == null) { throw new ArgumentNullException("getDiscriminant"); }
            if (includes == null) { throw new ArgumentNullException("includes"); }

            return PolymorphicLoadLink(
                getLinks,
                LinkTargetFactory.Create(getLinkTarget),
                getDiscriminant, 
                includes
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TAbstractLinkTarget, TLink, TDiscriminant>(
            Func<TLinkedSource, List<TLink>> getLinks, 
            ILinkTarget<TLinkedSource, TAbstractLinkTarget> linkTarget, 
            Func<TLink, TDiscriminant> getDiscriminant, 
            Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes) 
        {
            var includeBuilder = new IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(linkTarget);
            includes(includeBuilder);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(
				getLinks,
                linkTarget, 
				includeBuilder.Build(getDiscriminant)
			);

            return AddLoadLinkExpression(loadLinkExpression);
        }
        #endregion

        #region Shared
        private static Func<TLinkedSource, List<TId>> ToGetLookupIdsForSingleValue<TId>(
            Func<TLinkedSource, TId> getLookupId) 
        {
            return linkedSource => new List<TId> { getLookupId(linkedSource) };
        }

        private static Func<TLinkedSource, List<TId>> ToGetLookupIdsForOptionalSingleValue<TId>(
            Func<TLinkedSource, TId?> getOptionalLookupId
        ) 
            where TId:struct
        {
            return linkedSource => OptionalIdToList(getOptionalLookupId(linkedSource));
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
            _addLoadLinkExpression(loadLinkExpression);
            return this;
        }

        public static Func<T, T> CreateIdentityFunc<T>() {
            return x => x;
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> AddNonPolymorphicLoadLinkExpression<TTargetProperty, TId>(
            Func<TLinkedSource, List<TId>> getLookupIds, 
            ILinkTarget<TLinkedSource, TTargetProperty> linkTarget, 
            IInclude include) 
        {
            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TTargetProperty, TId, bool>(
                getLookupIds,
                linkTarget, 
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
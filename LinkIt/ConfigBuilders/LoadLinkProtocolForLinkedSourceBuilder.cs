using System;
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
                LinkTargetFactory.Create(getLinkTarget),
                ToGetLookupIdsForSingleValue(getLookupId)
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
            Func<TLinkedSource, List<TId>> getLookupIds,
            Expression<Func<TLinkedSource, List<TReference>>> getLinkTarget)
        {
            if (getLookupIds == null) { throw new ArgumentNullException("getLookupIds"); }
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkReferenceById(
                LinkTargetFactory.Create(getLinkTarget),
                getLookupIds 
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
                LinkTargetFactory.Create(getLinkTarget),
                ToGetLookupIdsForOptionalSingleValue(getOptionalLookupId)
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
            ILinkTarget<TLinkedSource, TReference> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIds) 
        {
            return AddNonPolymorphicLoadLinkExpression(
                linkTarget,
                getLookupIds,
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
                LinkTargetFactory.Create(getLinkTarget),
                ToGetLookupIdsForSingleValue(getLookupId),
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
                LinkTargetFactory.Create(getLinkTarget),
                ToGetLookupIdsForOptionalSingleValue(getOptionalLookupId),
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
                LinkTargetFactory.Create(getLinkTarget),
                ToGetLookupIdsForOptionalSingleValue(getOptionalLookupId),
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
            if (getLinkTarget == null) { throw new ArgumentNullException("getLinkTarget"); }

            return LoadLinkNestedLinkedSourceById(
                LinkTargetFactory.Create(getLinkTarget),
                getLookupIds,
                initChildLinkedSource
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            ILinkTarget<TLinkedSource, TChildLinkedSource> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIds,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource)
        {
            var include = LinkedSourceConfigs.GetConfigFor<TChildLinkedSource>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TChildLinkedSource, TId, TId>(
                    CreateIdentityFunc<TId>(),
                    initChildLinkedSource
                );

            return AddNonPolymorphicLoadLinkExpression(
                linkTarget,
                getLookupIds,
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
                LinkTargetFactory.Create(getLinkTarget),
                ToGetLookupIdsForSingleValue(getNestedLinkedSourceModel)
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
                LinkTargetFactory.Create(getLinkTarget),
                getNestedLinkedSourceModels
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
                LinkTargetFactory.Create(getLinkTarget),
                ToGetLookupIdsForSingleValue(getLink),
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
                LinkTargetFactory.Create(getLinkTarget),
                getLinks,
                getDiscriminant,
                includes
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TAbstractLinkTarget, TLink, TDiscriminant>(
            ILinkTarget<TLinkedSource, TAbstractLinkTarget> linkTarget,
            Func<TLinkedSource, List<TLink>> getLinks,
            Func<TLink, TDiscriminant> getDiscriminant,
            Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes) 
        {
            var includeBuilder = new IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(linkTarget);
            includes(includeBuilder);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(
                linkTarget,
                getLinks,
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
            ILinkTarget<TLinkedSource, TTargetProperty> linkTarget,
            Func<TLinkedSource, List<TId>> getLookupIds,
            IInclude include) {
            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TTargetProperty, TId, bool>(
                linkTarget,
                getLookupIds,
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
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinkIt.Core;
using LinkIt.Core.Includes;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.LinkTargets;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.PublicApi;

namespace LinkIt.ConfigBuilders
{
    /// <summary>
    /// Builder to help configure the <see cref="ILoadLinkProtocol"/> for a linked source type.
    /// </summary>
    public class LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>
    {
        private readonly Action<ILoadLinkExpression> _addLoadLinkExpression;

        internal LoadLinkProtocolForLinkedSourceBuilder(Action<ILoadLinkExpression> addLoadLinkExpression)
        {
            _addLoadLinkExpression = addLoadLinkExpression;
        }

        #region ReferenceById

        /// <summary>
        /// Load a reference by ID.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
            Func<TLinkedSource, TId> getLookupId,
            Expression<Func<TLinkedSource, TReference>> getLinkTarget)
        {
            if (getLookupId == null) throw new ArgumentNullException(nameof(getLookupId));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));

            return LoadLinkReferencesByIds(
                ToGetLookupIdsForSingleValue(getLookupId),
                LinkTargetFactory.Create(getLinkTarget)
            );
        }

        /// <summary>
        /// Load a reference by ID.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferencesByIds<TReference, TId>(
            Func<TLinkedSource, IEnumerable<TId>> getLookupIds,
            Expression<Func<TLinkedSource, IList<TReference>>> getLinkTarget)
        {
            if (getLookupIds == null) throw new ArgumentNullException(nameof(getLookupIds));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));

            return LoadLinkReferencesByIds(
                getLookupIds,
                LinkTargetFactory.Create(getLinkTarget)
            );
        }

        /// <summary>
        /// Load a reference by ID.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferenceById<TReference, TId>(
            Func<TLinkedSource, TId?> getOptionalLookupId,
            Expression<Func<TLinkedSource, TReference>> getLinkTarget
        )
            where TId : struct
        {
            if (getOptionalLookupId == null) throw new ArgumentNullException(nameof(getOptionalLookupId));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));

            return LoadLinkReferencesByIds(
                ToGetLookupIdsForOptionalSingleValue(getOptionalLookupId),
                LinkTargetFactory.Create(getLinkTarget)
            );
        }

        /// <summary>
        /// Load a reference by ID.
        /// </summary>
        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReferencesByIds<TReference, TId>(
            Func<TLinkedSource, IEnumerable<TId>> getLookupIds,
            ILinkTarget<TLinkedSource, TReference> linkTarget)
        {
            return AddNonPolymorphicLoadLinkExpression(
                getLookupIds,
                linkTarget,
                new IncludeReferenceById<TReference, TId, TReference, TId>(id => id)
            );
        }

        #endregion

        #region NestedLinkedSourceById

        /// <summary>
        /// Load a nested linked source by ID.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            Func<TLinkedSource, TId> getLookupId,
            Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget,
            Action<TLinkedSource, TChildLinkedSource> initChildLinkedSource = null)
        {
            if (getLookupId == null) throw new ArgumentNullException(nameof(getLookupId));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));

            return LoadLinkNestedLinkedSourcesByIds(
                ToGetLookupIdsForSingleValue(getLookupId),
                LinkTargetFactory.Create(getLinkTarget),
                (linkedSource, referenceIndex, childLinkedSource) => initChildLinkedSource?.Invoke(linkedSource, childLinkedSource)
            );
        }

        /// <summary>
        /// Load a nested linked source by ID.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceById<TChildLinkedSource, TId>(
            Func<TLinkedSource, TId?> getOptionalLookupId,
            Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget,
            Action<TLinkedSource, TChildLinkedSource> initChildLinkedSource = null
        )
            where TId : struct
        {
            if (getOptionalLookupId == null) throw new ArgumentNullException(nameof(getOptionalLookupId));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));

            return LoadLinkNestedLinkedSourcesByIds(
                ToGetLookupIdsForOptionalSingleValue(getOptionalLookupId),
                LinkTargetFactory.Create(getLinkTarget),
                (linkedSource, referenceIndex, childLinkedSource) => initChildLinkedSource?.Invoke(linkedSource, childLinkedSource)
            );
        }

        /// <summary>
        /// Load nested linked sources by IDs.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourcesByIds<TChildLinkedSource, TId>(
            Func<TLinkedSource, IEnumerable<TId>> getLookupIds,
            Expression<Func<TLinkedSource, IList<TChildLinkedSource>>> getLinkTarget,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource = null)
        {
            if (getLookupIds == null) throw new ArgumentNullException(nameof(getLookupIds));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));

            return LoadLinkNestedLinkedSourcesByIds(
                getLookupIds,
                LinkTargetFactory.Create(getLinkTarget),
                initChildLinkedSource
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourcesByIds<TChildLinkedSource, TId>(
            Func<TLinkedSource, IEnumerable<TId>> getLookupIds,
            ILinkTarget<TLinkedSource, TChildLinkedSource> linkTarget,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource = null)
        {
            var include = LinkedSourceConfigs.GetConfigFor<TChildLinkedSource>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TChildLinkedSource, TId, TId>(
                    id => id,
                    initChildLinkedSource
                );

            return AddNonPolymorphicLoadLinkExpression(
                getLookupIds,
                linkTarget,
                include
            );
        }

        #endregion

        #region NestedLinkedSourceFromModel

        /// <summary>
        /// Load a nested linked source using a model from its parent linked source.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourceFromModel<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, TChildLinkedSourceModel> getNestedLinkedSourceModel,
            Expression<Func<TLinkedSource, TChildLinkedSource>> getLinkTarget,
            Action<TLinkedSource, TChildLinkedSource> initChildLinkedSource = null
        ) where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
        {
            if (getNestedLinkedSourceModel == null) throw new ArgumentNullException(nameof(getNestedLinkedSourceModel));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));

            return LoadLinkNestedLinkedSourcesFromModels(
                ToGetLookupIdsForSingleValue(getNestedLinkedSourceModel),
                LinkTargetFactory.Create(getLinkTarget),
                (linkedSource, referenceIndex, childLinkedSource) => initChildLinkedSource?.Invoke(linkedSource, childLinkedSource)
            );
        }

        /// <summary>
        /// Load nested linked sources using models from the parent linked source.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourcesFromModels<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, IEnumerable<TChildLinkedSourceModel>> getNestedLinkedSourceModels,
            Expression<Func<TLinkedSource, IList<TChildLinkedSource>>> getLinkTarget,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource = null
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
        {
            if (getNestedLinkedSourceModels == null) throw new ArgumentNullException(nameof(getNestedLinkedSourceModels));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));

            return LoadLinkNestedLinkedSourcesFromModels(
                getNestedLinkedSourceModels,
                LinkTargetFactory.Create(getLinkTarget),
                initChildLinkedSource
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSourcesFromModels<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, IEnumerable<TChildLinkedSourceModel>> getNestedLinkedSourceModels,
            ILinkTarget<TLinkedSource, TChildLinkedSource> linkTarget,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSource
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
        {
            var include = new IncludeNestedLinkedSourceFromModel<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel, TChildLinkedSource, TChildLinkedSourceModel>(
                childLinkedSource => childLinkedSource,
                initChildLinkedSource
            );

            return AddNonPolymorphicLoadLinkExpression(
                getNestedLinkedSourceModels,
                linkTarget,
                include
            );
        }

        #endregion

        #region Polymorphic

        /// <summary>
        /// Load and link a polymorphic reference or linked source.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkPolymorphic<TAbstractLinkTarget, TLink, TDiscriminant>(
            Func<TLinkedSource, TLink> getLink,
            Expression<Func<TLinkedSource, TAbstractLinkTarget>> getLinkTarget,
            Func<TLink, TDiscriminant> getDiscriminant,
            Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes)
        {
            if (getLink == null) throw new ArgumentNullException(nameof(getLink));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));
            if (getDiscriminant == null) throw new ArgumentNullException(nameof(getDiscriminant));
            if (includes == null) throw new ArgumentNullException(nameof(includes));

            return LoadLinkPolymorphic(
                ToGetLookupIdsForSingleValue(getLink),
                LinkTargetFactory.Create(getLinkTarget),
                getDiscriminant,
                includes
            );
        }

        /// <summary>
        /// Load and link polymorphic references or linked sources.
        /// </summary>
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkPolymorphicList<TAbstractLinkTarget, TLink, TDiscriminant>(
            Func<TLinkedSource, IEnumerable<TLink>> getLinks,
            Expression<Func<TLinkedSource, IList<TAbstractLinkTarget>>> getLinkTarget,
            Func<TLink, TDiscriminant> getDiscriminant,
            Action<IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>> includes)
        {
            if (getLinks == null) throw new ArgumentNullException(nameof(getLinks));
            if (getLinkTarget == null) throw new ArgumentNullException(nameof(getLinkTarget));
            if (getDiscriminant == null) throw new ArgumentNullException(nameof(getDiscriminant));
            if (includes == null) throw new ArgumentNullException(nameof(includes));

            return LoadLinkPolymorphic(
                getLinks,
                LinkTargetFactory.Create(getLinkTarget),
                getDiscriminant,
                includes
            );
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkPolymorphic<TAbstractLinkTarget, TLink, TDiscriminant>(
            Func<TLinkedSource, IEnumerable<TLink>> getLinks,
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
            where TId : struct
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
            ILoadLinkExpression loadLinkExpression)
        {
            _addLoadLinkExpression(loadLinkExpression);
            return this;
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> AddNonPolymorphicLoadLinkExpression<TTargetProperty, TId>(
            Func<TLinkedSource, IEnumerable<TId>> getLookupIds,
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
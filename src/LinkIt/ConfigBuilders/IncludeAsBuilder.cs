// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using LinkIt.Core;
using LinkIt.Core.Includes;
using LinkIt.PublicApi;

namespace LinkIt.ConfigBuilders
{
    /// <summary>
    /// Builder to configure a linked source by specifying nested linked sources and reference.
    /// </summary>
    public class IncludeAsBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant, TLinkTarget>
        where TLinkTarget : TAbstractLinkTarget
        where TLinkedSource: ILinkedSource
    {
        private readonly IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> _includeSetBuilder;

        internal IncludeAsBuilder(IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> includeSetBuilder)
        {
            _includeSetBuilder = includeSetBuilder;
        }

        /// <summary>
        /// Load and link a nested linked source by ID when the polymorphic link matches the <paramref name="discriminantValue"/>.
        /// </summary>
        public IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsNestedLinkedSourceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupId,
            Action<TLinkedSource, int, TLinkTarget> initChildLinkedSource = null)
        {
            if (getLookupId is null)
            {
                throw new ArgumentNullException(nameof(getLookupId));
            }

            var include = LinkedSourceConfigs.GetConfigFor<TLinkTarget>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TAbstractLinkTarget, TLink, TId>(
                    getLookupId,
                    initChildLinkedSource
                );

            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                include
            );

            return _includeSetBuilder;
        }

        /// <summary>
        /// Load and link a nested linked source by ID when the polymorphic link matches the <paramref name="discriminantValue"/>.
        /// </summary>
        public IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsNestedLinkedSourceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupId,
            Action<TLink, TLinkTarget> initChildLinkedSource
        )
        {
            if (getLookupId == null)
            {
                throw new ArgumentNullException(nameof(getLookupId));
            }

            var include = LinkedSourceConfigs.GetConfigFor<TLinkTarget>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TAbstractLinkTarget, TLink, TId>(
                    getLookupId,
                    initChildLinkedSource
                );

            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                include
            );

            return _includeSetBuilder;
        }

        /// <summary>
        /// Load and link a nested linked source using a model from the parent linked source when the polymorphic link matches the <paramref name="discriminantValue"/>.
        /// </summary>
        public IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsNestedLinkedSourceFromModel<TChildLinkedSourceModel>(
            TDiscriminant discriminantValue,
            Expression<Func<TLink, TChildLinkedSourceModel>> getNestedLinkedSourceModel,
            Action<TLinkedSource, int, TLinkTarget> initChildLinkedSource = null)
        {
            if (getNestedLinkedSourceModel is null)
            {
                throw new ArgumentNullException(nameof(getNestedLinkedSourceModel));
            }

            var include = LinkedSourceConfigs.GetConfigFor<TLinkTarget>()
                .CreateIncludeNestedLinkedSourceFromModel<TLinkedSource, TAbstractLinkTarget, TLink, TChildLinkedSourceModel>(
                    getNestedLinkedSourceModel,
                    _includeSetBuilder.LinkTarget,
                    initChildLinkedSource
                );

            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                include
            );

            return _includeSetBuilder;
        }

        /// <summary>
        /// Load and link a nested linked source using a model from the parent linked source when the polymorphic link matches the <paramref name="discriminantValue"/>.
        /// </summary>
        public IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsNestedLinkedSourceFromModel<TChildLinkedSourceModel>(
            TDiscriminant discriminantValue,
            Expression<Func<TLink, TChildLinkedSourceModel>> getNestedLinkedSourceModel,
            Action<TLink, TLinkTarget> initChildLinkedSource)
        {
            if (getNestedLinkedSourceModel is null)
            {
                throw new ArgumentNullException(nameof(getNestedLinkedSourceModel));
            }

            var include = LinkedSourceConfigs.GetConfigFor<TLinkTarget>()
                .CreateIncludeNestedLinkedSourceFromModel<TLinkedSource, TAbstractLinkTarget, TLink, TChildLinkedSourceModel>(
                    getNestedLinkedSourceModel,
                    _includeSetBuilder.LinkTarget,
                    initChildLinkedSource
                );

            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                include
            );

            return _includeSetBuilder;
        }

        /// <summary>
        /// Load and link a reference by ID when the polymorphic link matches the <paramref name="discriminantValue"/>.
        /// </summary>
        public IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsReferenceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupId
        )
        {
            if (getLookupId is null)
            {
                throw new ArgumentNullException(nameof(getLookupId));
            }

            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                new IncludeReferenceById<TAbstractLinkTarget, TLink, TLinkTarget, TId>(
                    getLookupId
                )
            );

            return _includeSetBuilder;
        }
    }
}

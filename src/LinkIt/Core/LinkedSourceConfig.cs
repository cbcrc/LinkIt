// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinkIt.Core.Includes;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.PublicApi;
using LinkIt.ReadableExpressions;
using LinkIt.ReadableExpressions.Extensions;
using LinkIt.Shared;

namespace LinkIt.Core
{
    /// <inheritdoc />
    internal class LinkedSourceConfig<TLinkedSource, TLinkedSourceModel> : IGenericLinkedSourceConfig<TLinkedSource>
        where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
    {
        public LinkedSourceConfig()
        {
            LinkedSourceType = typeof(TLinkedSource);
            LinkedSourceModelType = typeof(TLinkedSourceModel);
        }

        public Type LinkedSourceType { get; }
        public Type LinkedSourceModelType { get; }

        public ILoadLinker<TLinkedSource> CreateLoadLinker(Func<IReferenceLoader> createReferenceLoader,
            IReadOnlyList<IReadOnlyList<Type>> referenceTypeToBeLoadedForEachLoadingLevel,
            LoadLinkProtocol loadLinkProtocol)
        {
            return new LoadLinkerWrapper<TLinkedSource, TLinkedSourceModel>(createReferenceLoader, referenceTypeToBeLoadedForEachLoadingLevel, loadLinkProtocol);
        }

        public IInclude CreateIncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupId,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSource)
        {
            EnsureIsAssignableFrom<TAbstractChildLinkedSource, TLinkedSource>();

            return new IncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel, TId>(
                getLookupId,
                initChildLinkedSource
            );
        }

        public IInclude CreateIncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupId,
            Action<TLink, TLinkedSource> initChildLinkedSource)
        {
            EnsureIsAssignableFrom<TAbstractChildLinkedSource, TLinkedSource>();

            return new IncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel, TId>(
                getLookupId,
                initChildLinkedSource
            );
        }

        public IInclude CreateIncludeNestedLinkedSourceFromModel<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TChildLinkedSourceModel>(
            Expression<Func<TLink, TChildLinkedSourceModel>> getNestedLinkedSourceModel,
            ILinkTarget linkTarget,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSource)
        {
            EnsureGetNestedLinkedSourceModelReturnsTheExpectedType<TLink, TChildLinkedSourceModel>(linkTarget, getNestedLinkedSourceModel);
            EnsureIsAssignableFrom<TAbstractChildLinkedSource, TLinkedSource>();

            return new IncludeNestedLinkedSourceFromModel<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel>(
                WrapGetNestedLinkedSourceModel(getNestedLinkedSourceModel.Compile()),
                initChildLinkedSource
            );
        }

        public IInclude CreateIncludeNestedLinkedSourceFromModel<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TChildLinkedSourceModel>(
            Expression<Func<TLink, TChildLinkedSourceModel>> getNestedLinkedSourceModel,
            ILinkTarget linkTarget,
            Action<TLink, TLinkedSource> initChildLinkedSource)
        {
            EnsureGetNestedLinkedSourceModelReturnsTheExpectedType<TLink, TChildLinkedSourceModel>(linkTarget, getNestedLinkedSourceModel);
            EnsureIsAssignableFrom<TAbstractChildLinkedSource, TLinkedSource>();

            return new IncludeNestedLinkedSourceFromModel<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel>(
                WrapGetNestedLinkedSourceModel(getNestedLinkedSourceModel.Compile()),
                initChildLinkedSource
            );
        }

        private static Func<TLink, TLinkedSourceModel> WrapGetNestedLinkedSourceModel<TLink, TChildLinkedSourceModel>(
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel)
        {
            return link => (TLinkedSourceModel) (object) getNestedLinkedSourceModel(link);
        }

        private static void EnsureIsAssignableFrom<TAbstract, TConcrete>()
        {
            var abstractType = typeof(TAbstract);
            var concreteType = typeof(TConcrete);

            if (!abstractType.IsAssignableFrom(concreteType))
            {
                throw new LinkItException(
                   $"{abstractType} is not assignable from {concreteType}."
               );
            }
        }

        private void EnsureGetNestedLinkedSourceModelReturnsTheExpectedType<TLink, TChildLinkedSourceModel>(
            ILinkTarget linkTarget,
            Expression<Func<TLink, TChildLinkedSourceModel>> getNestedLinkedSourceModel)
        {
            if (!LinkedSourceModelType.IsAssignableFrom(typeof(TChildLinkedSourceModel)))
            {
                throw new LinkItException(
                   $"Invalid configuration for linked source {typeof(TLinkedSource).GetFriendlyName()}, polymorphic list target {{{linkTarget.Expression}}}: "
                    + $"in AsNestedLinkedSourceFromModel(), expression supplied to getNestedLinkedSourceModel ({getNestedLinkedSourceModel.ToReadableString()}) returns an invalid type. "
                    + $"Expected {LinkedSourceModelType.GetFriendlyName()}, but received {typeof(TChildLinkedSourceModel).GetFriendlyName()}."
               );
            }
        }
    }
}

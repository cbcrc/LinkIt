#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using LinkIt.Core.Includes;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.Core.Interfaces;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.PublicApi;
using LinkIt.Shared;

namespace LinkIt.Core
{
    //See IGenericLinkedSourceConfig and ILinkedSourceConfig
    public class LinkedSourceConfig<TLinkedSource, TLinkedSourceModel>:IGenericLinkedSourceConfig<TLinkedSource> 
        where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
    {
        public LinkedSourceConfig(){
            LinkedSourceType = typeof (TLinkedSource);
            LinkedSourceModelType = typeof (TLinkedSourceModel);
        }

        public Type LinkedSourceType { get; private set; }
        public Type LinkedSourceModelType { get; set; }

        public ILoadLinker<TLinkedSource> CreateLoadLinker(
            IReferenceLoader referenceLoader, 
            List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, 
            LoadLinkProtocol loadLinkProtocol)
        {
            return new LoadLinker<TLinkedSource, TLinkedSourceModel>(referenceLoader, referenceTypeToBeLoadedForEachLoadingLevel, loadLinkProtocol);
        }


        public IInclude CreateIncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupId,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSource)
        {
            AssumeClassIsAssignableFrom<TAbstractChildLinkedSource, TLinkedSource>();

            return new IncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel, TId>(
                getLookupId,
                initChildLinkedSource
            );
        }

        public IInclude CreateIncludeNestedLinkedSourceFromModel<TAbstractChildLinkedSource, TLink, TChildLinkedSourceModel>(Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel, ILinkTarget linkTarget)
        {
            EnsureGetNestedLinkedSourceModelReturnsTheExpectedType<TChildLinkedSourceModel>(linkTarget);
            AssumeClassIsAssignableFrom<TAbstractChildLinkedSource, TLinkedSource>();

            return new IncludeNestedLinkedSourceFromModel<TAbstractChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel>(
                WrapGetNestedLinkedSourceModel(getNestedLinkedSourceModel)
            );
        }

        private Func<TLink, TLinkedSourceModel> WrapGetNestedLinkedSourceModel<TLink, TChildLinkedSourceModel>(
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel)
        {
            return link => (TLinkedSourceModel) (object) getNestedLinkedSourceModel(link);
        }

        private void AssumeClassIsAssignableFrom<TAbstract,TConcrete>(){
            var abstractType = typeof (TAbstract);
            var concreteType = typeof(TConcrete);

            if (!abstractType.IsAssignableFrom(concreteType)){
                throw new AssumptionFailed(
                    string.Format(
                        "{0} is not assignable from {1}.",
                        abstractType,
                        concreteType
                    )
                );
            }
        }

        private void EnsureGetNestedLinkedSourceModelReturnsTheExpectedType<TChildLinkedSourceModel>(ILinkTarget linkTarget) {
            if (!LinkedSourceModelType.IsAssignableFrom(typeof(TChildLinkedSourceModel))) {
                throw new ArgumentException(
                    string.Format(
                        "{0}: getNestedLinkedSourceModel returns an invalid type. {1} is not assignable from {2}.",
                        linkTarget.Id,
                        LinkedSourceModelType,
                        typeof(TChildLinkedSourceModel)
                    )
                );
            }
        }

    }
}
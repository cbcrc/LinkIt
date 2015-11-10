using System;
using System.Collections.Generic;
using HeterogeneousDataSources.LinkTargets;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.Shared;

namespace HeterogeneousDataSources.LinkedSources
{
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
            LoadLinkConfig config)
        {
            return new LoadLinker<TLinkedSource, TLinkedSourceModel>(referenceLoader, referenceTypeToBeLoadedForEachLoadingLevel, config);
        }


        public IInclude CreateIncludeNestedLinkedSourceById<TLinkTargetOwner, TIChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSourceAction)
        {
            AssumeClassIsAssignableFrom<TIChildLinkedSource, TLinkedSource>();

            return new IncludeNestedLinkedSourceById<TLinkTargetOwner, TIChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel, TId>(
                getLookupIdFunc,
                initChildLinkedSourceAction
            );
        }

        public IInclude CreateIncludeNestedLinkedSourceFromModel<TIChildLinkedSource, TLink, TChildLinkedSourceModel>(Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel, ILinkTarget linkTarget)
        {
            EnsureGetNestedLinkedSourceModelReturnsTheExpectedType<TChildLinkedSourceModel>(linkTarget);
            AssumeClassIsAssignableFrom<TIChildLinkedSource, TLinkedSource>();

            return new IncludeNestedLinkedSourceFromModel<TIChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel>(
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
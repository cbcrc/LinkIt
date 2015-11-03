using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources
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


        public IInclude CreateNestedLinkedSourceInclude<TLinkTargetOwner, TIChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSourceAction)
        {
            AssumeClassIsAssignableFrom<TIChildLinkedSource, TLinkedSource>();

            return new NestedLinkedSourceInclude<TLinkTargetOwner, TIChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel, TId>(
                getLookupIdFunc,
                initChildLinkedSourceAction
            );
        }

        public IInclude CreateSubLinkedSourceInclude<TIChildLinkedSource, TLink, TChildLinkedSourceModel>(
            Func<TLink, TChildLinkedSourceModel> getSubLinkedSourceModel)
        {
            AssumeClassIsAssignableFrom<TIChildLinkedSource, TLinkedSource>();

            return new SubLinkedSourceInclude<TIChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel>(
                WrapGetSubLinkedSourceModel(getSubLinkedSourceModel)
            );
        }

        private Func<TLink, TLinkedSourceModel> WrapGetSubLinkedSourceModel<TLink, TChildLinkedSourceModel>(
            Func<TLink, TChildLinkedSourceModel> getSubLinkedSourceModel)
        {
            if(getSubLinkedSourceModel==null){ return null; }

            //stle: error identification: if wrong model type
            return link => (TLinkedSourceModel) (object) getSubLinkedSourceModel(link);
        }

        private void AssumeClassIsAssignableFrom<TAbstract,TConcrete>(){
            var abstractType = typeof (TAbstract);
            var concreteType = typeof(TConcrete);

            if (!abstractType.IsAssignableFrom(concreteType)){
                throw new NotImplementedException(
                    string.Format(
                        "{0} is not assignable from {1}.",
                        abstractType,
                        concreteType
                    )
                );
            }
        }
    }
}
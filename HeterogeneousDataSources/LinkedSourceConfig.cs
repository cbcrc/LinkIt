using System;
using System.Collections.Generic;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources
{
    public class LinkedSourceConfig<TLinkedSource, TLinkedSourceModel>:ILinkedSourceConfig<TLinkedSource> 
        where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
    {
        public LinkedSourceConfig(){
            LinkedSourceType = typeof (TLinkedSource);
            LinkedSourceModelType = typeof (TLinkedSourceModel);
        }

        public Type LinkedSourceType { get; private set; }
        private Type LinkedSourceModelType { get; set; }

        public ILoadLinker<TLinkedSource> CreateLoadLinker(
            IReferenceLoader referenceLoader, 
            List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, 
            LoadLinkConfig config)
        {
            return new LoadLinker<TLinkedSource, TLinkedSourceModel>(referenceLoader, referenceTypeToBeLoadedForEachLoadingLevel, config);
        }


        public IInclude CreateNestedLinkedSourceInclude<TLinkTargetOwner, TIChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSourceAction = null
        )//stle: can make it mandatory?
        {
            //stle: ensure TChildLinkedSource == TLinkedSource
            //stle: ensure TLinkedSource impl TIChildLinkedSource

            return new NestedLinkedSourceInclude<TLinkTargetOwner, TIChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel, TId>(
                getLookupIdFunc,
                initChildLinkedSourceAction
            );
        }

        public IInclude CreateSubLinkedSourceInclude<TIChildLinkedSource, TLink, TChildLinkedSourceModel>(
            Func<TLink, TChildLinkedSourceModel> getSubLinkedSourceModel)
        {
            //stle: ensure TLinkedSource impl TIChildLinkedSource
            //stle: ensure TChildLinkedSourceModel == TLinkedSourceModel

            //stle: what to do about null vs special value for func?
            return new SubLinkedSourceInclude<TIChildLinkedSource, TLink, TLinkedSource, TLinkedSourceModel>(
                WrapGetSubLinkedSourceModel(getSubLinkedSourceModel)
            );
        }

        private Func<TLink, TLinkedSourceModel> WrapGetSubLinkedSourceModel<TLink, TChildLinkedSourceModel>(
            Func<TLink, TChildLinkedSourceModel> getSubLinkedSourceModel)
        {
            if(getSubLinkedSourceModel==null){ return null; }

            return link => (TLinkedSourceModel) (object) getSubLinkedSourceModel(link);
        }
    }
}
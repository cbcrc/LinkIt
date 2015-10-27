using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public class LinkedSourceConfig<TLinkedSource, TLinkedSourceModel>:ILinkedSourceConfig<TLinkedSource> 
        where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new() 
    {
        private readonly List<List<Type>> _referenceTypeToBeLoadedForEachLoadingLevel;
        private readonly LoadLinkConfig _config;

        public LinkedSourceConfig(List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkConfig config){
            _referenceTypeToBeLoadedForEachLoadingLevel = referenceTypeToBeLoadedForEachLoadingLevel;
            _config = config;
            LinkedSourceType = typeof (TLinkedSource);
            LinkedSourceModelType = typeof (TLinkedSourceModel);
        }

        public Type LinkedSourceType { get; private set; }
        //stle: required?
        public Type LinkedSourceModelType { get; private set; }

        public ILoadLinker<TLinkedSource> CreateLoadLinker(IReferenceLoader referenceLoader)
        {
            return new LoadLinker<TLinkedSource, TLinkedSourceModel>(referenceLoader, _referenceTypeToBeLoadedForEachLoadingLevel, _config);
        }
    }
}
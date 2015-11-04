using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public class SubLinkedSourceInclude<TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel>: 
        IIncludeWithCreateSubLinkedSource<TIChildLinkedSource,TLink>, 
        IIncludeWithChildLinkedSource
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TChildLinkedSourceModel> _getSubLinkedSourceModel;

        public SubLinkedSourceInclude(Func<TLink, TChildLinkedSourceModel> getSubLinkedSourceModel)
        {
            _getSubLinkedSourceModel = getSubLinkedSourceModel;
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ChildLinkedSourceType { get; private set; }

        public TIChildLinkedSource CreateSubLinkedSource(TLink link, LoadedReferenceContext loadedReferenceContext)
        {
            var childLinkSourceModel = _getSubLinkedSourceModel(link);

            //stle: move double cast to loadedReferenceContext
            //stle: dry with nested linked source
            return (TIChildLinkedSource) (object) loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(childLinkSourceModel);
        }

        public void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkConfig config) {
            config.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, parent);
        }
    }
}
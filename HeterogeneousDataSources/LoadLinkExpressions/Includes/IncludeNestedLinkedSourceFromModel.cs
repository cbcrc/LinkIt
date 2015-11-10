using System;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.ReferenceTrees;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public class IncludeNestedLinkedSourceFromModel<TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel>: 
        IIncludeWithCreateNestedLinkedSourceFromModel<TIChildLinkedSource,TLink>, 
        IIncludeWithChildLinkedSource
        where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        private readonly Func<TLink, TChildLinkedSourceModel> _getNestedLinkedSourceModel;

        public IncludeNestedLinkedSourceFromModel(Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel)
        {
            _getNestedLinkedSourceModel = getNestedLinkedSourceModel;
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ChildLinkedSourceType { get; private set; }

        public TIChildLinkedSource CreateNestedLinkedSourceFromModel(TLink link, LoadedReferenceContext loadedReferenceContext)
        {
            var childLinkSourceModel = _getNestedLinkedSourceModel(link);

            return (TIChildLinkedSource) (object) loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(childLinkSourceModel);
        }

        public void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkConfig config) {
            config.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, parent);
        }
    }
}
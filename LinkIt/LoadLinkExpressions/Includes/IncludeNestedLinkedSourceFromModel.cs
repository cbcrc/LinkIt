using System;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.LoadLinkExpressions.Includes.Interfaces;
using LinkIt.Protocols;
using LinkIt.ReferenceTrees;

namespace LinkIt.LoadLinkExpressions.Includes
{
    public class IncludeNestedLinkedSourceFromModel<TAbstractChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel>: 
        IIncludeWithCreateNestedLinkedSourceFromModel<TAbstractChildLinkedSource,TLink>, 
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

        public TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(TLink link, LoadedReferenceContext loadedReferenceContext)
        {
            var childLinkSourceModel = _getNestedLinkedSourceModel(link);

            return (TAbstractChildLinkedSource) (object) loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(childLinkSourceModel);
        }

        public void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkConfig config) {
            config.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, parent);
        }
    }
}
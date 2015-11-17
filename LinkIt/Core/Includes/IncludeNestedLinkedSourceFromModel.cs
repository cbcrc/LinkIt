using System;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Includes
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

        public TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(TLink link, LoadedReferenceContext loadedReferenceContext, LoadLinkProtocol loadLinkProtocol)
        {
            var childLinkSourceModel = _getNestedLinkedSourceModel(link);

            return (TAbstractChildLinkedSource) (object) loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(childLinkSourceModel, loadLinkProtocol, null);
        }

        public void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol) {
            loadLinkProtocol.AddReferenceTreeForEachLinkTarget(ChildLinkedSourceType, parent);
        }
    }
}
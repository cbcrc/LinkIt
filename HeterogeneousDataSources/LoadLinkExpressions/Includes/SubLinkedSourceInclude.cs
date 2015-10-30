using System;
using System.Collections.Generic;
using System.Linq;

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
            var childLinkSourceModel = _getSubLinkedSourceModel!=null
                ? _getSubLinkedSourceModel(link)
                : UseLinkAsSubLinkedSourceModel(link);

            //stle: move double cast to loadedReferenceContext
            //stle: dry with nested linked source
            return (TIChildLinkedSource) (object) loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(childLinkSourceModel);
        }

        private TChildLinkedSourceModel UseLinkAsSubLinkedSourceModel(object link)
        {
            if (!(link is TChildLinkedSourceModel)){
                //stle: identifiy expression with id
                throw new Exception(
                    string.Format(
                        "Please provide a getSubLinkedSourceModel function in order to create a sub linked source model of type {0} from a link of type {1}.",
                        typeof(TChildLinkedSource),
                        typeof(TLink)
                    )
                );
            }

            return (TChildLinkedSourceModel)link;
        }
    }
}
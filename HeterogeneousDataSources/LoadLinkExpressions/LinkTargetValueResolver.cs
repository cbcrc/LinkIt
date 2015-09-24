using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    //stle: TIChildLinkedSource is not more of a TTarget
    public class LinkTargetValueResolver<TIChildLinkedSource, TLink, TInclude>
    {
        private readonly List<TLink> _links;
        private readonly Func<TLink, TInclude> _getInclude;
        private readonly Func<LinkWithIndexAndInclude<TLink, TInclude>, TIChildLinkedSource> _getLinkTargetValueForLink;

        public LinkTargetValueResolver(List<TLink> links, Func<TLink, TInclude> getInclude, Func<LinkWithIndexAndInclude<TLink, TInclude>, TIChildLinkedSource> getLinkTargetValueForLink)
        {
            _links = links;
            _getInclude = getInclude;
            _getLinkTargetValueForLink = getLinkTargetValueForLink;
        }

        public List<LinkTargetValueWithIndex<TIChildLinkedSource>> Resolve() 
        {
            return GetListOfLinkWithIndexAndInclude()
                .Select(CreateLinkTargetValueWithIndex)
                .ToList();
        }

        private List<LinkWithIndexAndInclude<TLink, TInclude>> GetListOfLinkWithIndexAndInclude() {
            return _links
                .Select(CreateLinkWithIndexAndInclude)
                .Where(linkWithIndexAndInclude => linkWithIndexAndInclude != null)
                .ToList();
        }

        private LinkWithIndexAndInclude<TLink, TInclude> CreateLinkWithIndexAndInclude(TLink link, int index) {
            if (link == null) { return null; }

            var include = _getInclude(link);
            if (include == null) { return null; }

            return new LinkWithIndexAndInclude<TLink, TInclude>(
                link,
                index,
                include
            );
        }

        private LinkTargetValueWithIndex<TIChildLinkedSource> CreateLinkTargetValueWithIndex(LinkWithIndexAndInclude<TLink, TInclude> linkWithIndexAndInclude)
        {
            return new LinkTargetValueWithIndex<TIChildLinkedSource>(
                _getLinkTargetValueForLink(linkWithIndexAndInclude),
                linkWithIndexAndInclude.Index
            );
        }


    }
}
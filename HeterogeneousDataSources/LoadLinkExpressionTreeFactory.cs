using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{
    public class LoadLinkExpressionTreeFactory
    {
        private readonly List<ILoadLinkExpression> _loadLinkExpressions;

        public LoadLinkExpressionTreeFactory(List<ILoadLinkExpression> loadLinkExpressions){
            _loadLinkExpressions = loadLinkExpressions;
        }

        public Tree<ILoadLinkExpression> Create(ILoadLinkExpression node)
        {
            var childLoadLinkExpressions = GetChildLoadLinkExpressions(node);
            var children = childLoadLinkExpressions
                .Select(Create)
                .ToList();
            return new Tree<ILoadLinkExpression>(
                node,
                children
            );
        }

        private List<ILoadLinkExpression> GetChildLoadLinkExpressions(ILoadLinkExpression node)
        {
            var nodeAsNestedLoadLinkExpression = node as INestedLoadLinkExpression;
            if (nodeAsNestedLoadLinkExpression == null) { return new List<ILoadLinkExpression>(); }

            return _loadLinkExpressions
                .Where(loadLinkExpression => 
                    loadLinkExpression.LinkedSourceType == nodeAsNestedLoadLinkExpression.ChildLinkedSourceType)
                .ToList();
        }
    }
}
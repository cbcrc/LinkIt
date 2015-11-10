using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Protocols;

namespace HeterogeneousDataSources.ConfigBuilders {
    public class LoadLinkProtocolBuilder
    {
        private readonly Dictionary<string, ILoadLinkExpression> _loadLinkExpressionsById = 
            new Dictionary<string, ILoadLinkExpression>();

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> For<TLinkedSource>(){
            return new LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>(AddLoadLinkExpression);
        }

        private void AddLoadLinkExpression(ILoadLinkExpression loadLinkExpression)
        {
            _loadLinkExpressionsById[loadLinkExpression.LinkTargetId] = loadLinkExpression;
        }

        public LoadLinkProtocol Build(IReferenceLoader referenceLoader)
        {
            if (referenceLoader == null) { throw new ArgumentNullException("referenceLoader"); }

            return new LoadLinkProtocol(
                referenceLoader,
                new LoadLinkConfig(GetLoadLinkExpressions())
            );
        }

        public List<ILoadLinkExpression> GetLoadLinkExpressions(){
            return _loadLinkExpressionsById.Values.ToList();
        }
    }
}

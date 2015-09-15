using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkProtocolBuilder
    {
        private readonly List<ILoadLinkExpression> _loadLinkExpressions = new List<ILoadLinkExpression>();

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> For<TLinkedSource>(){
            return new LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>(AddLoadLinkExpression);
        }

        private void AddLoadLinkExpression(ILoadLinkExpression loadLinkExpression)
        {
            _loadLinkExpressions.Add(loadLinkExpression);
        }

        public LoadLinkProtocol Build(IReferenceLoader referenceLoader)
        {
            if (referenceLoader == null) { throw new ArgumentNullException("referenceLoader"); }

            return new LoadLinkProtocol(
                referenceLoader, 
                new LoadLinkConfig(_loadLinkExpressions)
            );
        }

        public List<ILoadLinkExpression> GetLoadLinkExpressions(){
            return _loadLinkExpressions.ToList();
        }
    }
}

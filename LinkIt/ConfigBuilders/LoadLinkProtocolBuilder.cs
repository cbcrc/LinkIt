using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Core;
using LinkIt.Core.Interfaces;
using LinkIt.PublicApi;

namespace LinkIt.ConfigBuilders {
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

        public ILoadLinkProtocol Build(Func<IReferenceLoader> createReferenceLoader)
        {
            if (createReferenceLoader == null) { throw new ArgumentNullException("createReferenceLoader"); }

            return new LoadLinkProtocol(
                _loadLinkExpressionsById.Values.ToList(), 
                createReferenceLoader
            );
        }
    }
}

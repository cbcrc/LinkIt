﻿using System;
using System.Collections.Generic;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkProtocolBuilder
    {
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<ILoadLinkExpression> _loadLinkExpressions = new List<ILoadLinkExpression>();

        public LoadLinkProtocolBuilder(IReferenceLoader referenceLoader)
        {
            if (referenceLoader == null) { throw new ArgumentNullException("referenceLoader");}
            _referenceLoader = referenceLoader;
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> For<TLinkedSource>(){
            return new LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>(AddLoadLinkExpression);
        }

        private void AddLoadLinkExpression(ILoadLinkExpression loadLinkExpression)
        {
            _loadLinkExpressions.Add(loadLinkExpression);
        }

        public LoadLinkProtocol Build()
        {
            return new LoadLinkProtocol(_referenceLoader, new LoadLinkConfig(_loadLinkExpressions));
        }
    }
}
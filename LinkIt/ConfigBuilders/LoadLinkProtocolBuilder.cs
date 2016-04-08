using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.Core;
using LinkIt.Core.Interfaces;
using LinkIt.PublicApi;

namespace LinkIt.ConfigBuilders {
    public class LoadLinkProtocolBuilder
    {
        private readonly Dictionary<string, ILoadLinkExpression> _loadLinkExpressionsById = 
            new Dictionary<string, ILoadLinkExpression>();

        public void ApplyLoadLinkProtocolConfigs(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null) { throw new ArgumentNullException("assemblies"); }

            var loadLinkProtocolConfigTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterface("LinkIt.ConfigBuilders.ILoadLinkProtocolConfig")!=null)
                .ToList();

            var loadLinkProtocolConfigs = loadLinkProtocolConfigTypes
                .Select(Activator.CreateInstance)
                .Cast<ILoadLinkProtocolConfig>()
                .ToList();

            foreach (var loadLinkProtocolConfig in loadLinkProtocolConfigs){
                loadLinkProtocolConfig.ConfigureLoadLinkProtocol(this);
            }
        }

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

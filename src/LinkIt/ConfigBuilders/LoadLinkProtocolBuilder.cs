// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.Core;
using LinkIt.Core.Interfaces;
using LinkIt.PublicApi;
using LinkIt.Shared;

namespace LinkIt.ConfigBuilders
{
    /// <summary>
    /// Builder to help with the configuration of the load link protocol.
    /// </summary>
    public class LoadLinkProtocolBuilder
    {
        private readonly Dictionary<string, ILoadLinkExpression> _loadLinkExpressionsById =
            new Dictionary<string, ILoadLinkExpression>();

        public void ApplyLoadLinkProtocolConfigs(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            var loadLinkProtocolConfigTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterface(typeof(ILoadLinkProtocolConfig).FullName) != null)
                .ToList();

            var loadLinkProtocolConfigs = loadLinkProtocolConfigTypes
                .Select(Activator.CreateInstance)
                .Cast<ILoadLinkProtocolConfig>()
                .ToList();

            foreach (var loadLinkProtocolConfig in loadLinkProtocolConfigs) loadLinkProtocolConfig.ConfigureLoadLinkProtocol(this);
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> For<TLinkedSource>()
        {
            if (!typeof(TLinkedSource).IsLinkedSource())
            {
                throw new ArgumentException($"Generic type parameter {typeof(TLinkedSource).Name} must implement ILinkedSource<>.", nameof(TLinkedSource));
            }

            return new LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>(AddLoadLinkExpression);
        }

        private void AddLoadLinkExpression(ILoadLinkExpression loadLinkExpression)
        {
            _loadLinkExpressionsById[loadLinkExpression.LinkTargetId] = loadLinkExpression;
        }

        public ILoadLinkProtocol Build(Func<IReferenceLoader> createReferenceLoader)
        {
            if (createReferenceLoader == null) throw new ArgumentNullException(nameof(createReferenceLoader));

            return new LoadLinkProtocol(
                _loadLinkExpressionsById.Values.ToList(),
                createReferenceLoader
            );
        }
    }
}
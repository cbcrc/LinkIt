// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    /// <summary>
    /// Responsible for creating linked sources.
    /// </summary>
    internal class Linker
    {
        private readonly List<object> _linkedSourcesToBeBuilt = new List<object>();
        private readonly LoadLinkProtocol _loadLinkProtocol;
        private readonly DataStore _dataStore;

        public Linker(LoadLinkProtocol loadLinkProtocol, DataStore dataStore)
        {
            _loadLinkProtocol = loadLinkProtocol;
            _dataStore = dataStore;
        }

        internal IReadOnlyList<object> LinkedSourcesToBeBuilt => _linkedSourcesToBeBuilt.ToList();

        public TLinkedSource CreatePartiallyBuiltLinkedSource<TLinkedSource, TLinkedSourceModel, TModelId>(TModelId id, Action<TLinkedSource> init)
            where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
        {
            var linkedSourceModel = _dataStore.GetReference<TLinkedSourceModel, TModelId>(id);
            return CreatePartiallyBuiltLinkedSource(linkedSourceModel, init);
        }

        public TLinkedSource CreatePartiallyBuiltLinkedSource<TLinkedSource, TLinkedSourceModel>(TLinkedSourceModel model, Action<TLinkedSource> init)
            where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
        {
            if ((object) model == null)
            {
                return null;
            }

            var linkedSource = new TLinkedSource { Model = model };
            init?.Invoke(linkedSource); //Important: Init before LinkNestedLinkedSourcesFromModel

            LinkNestedLinkedSourcesFromModel(linkedSource, _loadLinkProtocol);

            _linkedSourcesToBeBuilt.Add(linkedSource);

            return linkedSource;
        }

        private void LinkNestedLinkedSourcesFromModel(object linkedSource, LoadLinkProtocol loadLinkProtocol)
        {
            var loadLinkExpressions = loadLinkProtocol.GetLoadLinkExpressions(linkedSource);
            foreach (var loadLinkExpression in loadLinkExpressions)
            {
                loadLinkExpression.LinkNestedLinkedSourceFromModel(
                    linkedSource,
                    this,
                    loadLinkProtocol
                );
            }
        }

        public void LinkNestedLinkedSourcesById(IReadOnlyList<Type> referenceTypesToBeLoaded)
        {
            foreach (var linkedSource in LinkedSourcesToBeBuilt)
            {
                LinkNestedLinkedSourcesById(linkedSource, referenceTypesToBeLoaded);
            }
        }

        private void LinkNestedLinkedSourcesById(object linkedSource, IReadOnlyList<Type> referenceTypesToBeLoaded)
        {
            var loadLinkExpressionsForLinkedSource = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource);
            foreach (var referenceType in referenceTypesToBeLoaded)
            {
                var loadLinkExpressionsForReference = loadLinkExpressionsForLinkedSource.Where(e => e.ReferenceTypes.Contains(referenceType));
                foreach (var loadLinkExpression in loadLinkExpressionsForReference)
                {
                    loadLinkExpression.LinkNestedLinkedSourceById(linkedSource, this, referenceType, _loadLinkProtocol);
                }
            }
        }

        public void LinkReferences()
        {
            foreach (var linkedSource in LinkedSourcesToBeBuilt)
            {
                foreach (var loadLinkExpression in _loadLinkProtocol.GetLoadLinkExpressions(linkedSource))
                {
                    loadLinkExpression.LinkReference(linkedSource, _dataStore);
                }
            }
        }
    }
}

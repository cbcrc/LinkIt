// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    /// <summary>
    /// Responsible for giving access to the loaded references of a root linked source.
    /// </summary>
    internal class Linker
    {
        private readonly List<object> _linkedSourcesToBeBuilt = new List<object>();
        private readonly Dictionary<Type, object> _loadedReferencesByType = new Dictionary<Type, object>();

        internal IReadOnlyList<object> LinkedSourcesToBeBuilt => _linkedSourcesToBeBuilt.ToList();

        private IDictionary<TId, TReference> GetReferenceDictionary<TId, TReference>()
        {
            var tReference = typeof(TReference);
            if (!_loadedReferencesByType.ContainsKey(tReference))
            {
                return null;
            }

            return (IDictionary<TId, TReference>) _loadedReferencesByType[tReference];
        }

        public void AddReferences<TReference, TId>(IDictionary<TId, TReference> referencesById)
        {
            if (referencesById == null) throw new ArgumentNullException(nameof(referencesById));

            var dictionary = GetReferenceDictionary<TId, TReference>() ?? new Dictionary<TId, TReference>();
            foreach (var reference in referencesById)
            {
                dictionary[reference.Key] = reference.Value;
            }

            _loadedReferencesByType[typeof(TReference)] = dictionary;
        }

        public TReference GetOptionalReference<TReference, TId>(TId lookupId)
        {
            if (lookupId == null) return default;

            var referenceDictionnary = GetReferenceDictionary<TId, TReference>();
            if (referenceDictionnary == null) {
                throw new InvalidOperationException(
                    $"References of type {typeof(TReference).Name} were not loaded. Note that the implementation of IReferenceLoader must invoke {nameof(ILoadingContext)}.{nameof(ILoadingContext.AddReferences)} with an empty set if none of the ids provided in the LoadingContext for a specific reference type can be loaded."
                );
            }

            return referenceDictionnary.ContainsKey(lookupId)
                ? referenceDictionnary[lookupId]
                : default;
        }

        public IReadOnlyList<TReference> GetOptionalReferences<TReference,TId>(IEnumerable<TId> lookupIds)
        {
            return lookupIds
                .Select(GetOptionalReference<TReference, TId>)
                .ToList();
        }

        public TLinkedSource CreatePartiallyBuiltLinkedSource<TLinkedSource, TLinkedSourceModel>(TLinkedSourceModel model, LoadLinkProtocol loadLinkProtocol, Action<TLinkedSource> init)
            where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
        {
            if (model == null) return null;

            var linkedSource = new TLinkedSource { Model = model };
            init?.Invoke(linkedSource); //Important: Init before LinkNestedLinkedSourcesFromModel

            LinkNestedLinkedSourcesFromModel(linkedSource, loadLinkProtocol);

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
    }
}
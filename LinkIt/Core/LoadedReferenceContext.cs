#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    /// <summary>
    /// In addition to the responsiblies of ILoadedReferenceContext,
    /// responsible for giving access to the loaded references of a root linked source.
    /// </summary>
    public class LoadedReferenceContext : ILoadedReferenceContext
    {
        private readonly List<object> _linkedSourcesToBeBuilt = new List<object>();
        private readonly Dictionary<Type, object> _referenceDictionaryByReferenceType = new Dictionary<Type, object>();

        internal IEnumerable<object> GetLinkedSourcesToBeBuilt() => _linkedSourcesToBeBuilt.ToList();

        public void AddReferences<TReference, TId>(IEnumerable<TReference> references, Func<TReference, TId> getReferenceId)
        {
            if (references == null) throw new ArgumentNullException(nameof(references));
            if (getReferenceId == null) throw new ArgumentNullException(nameof(getReferenceId));

            var referenceDictionary = references.ToDictionary(
                getReferenceId,
                reference => reference
            );

            AddReferences(referenceDictionary);
        }

        public void AddReferences<TReference, TId>(IDictionary<TId, TReference> referencesById)
        {
            if (referencesById == null) throw new ArgumentNullException(nameof(referencesById));

            var tReference = typeof(TReference);
            if (_referenceDictionaryByReferenceType.ContainsKey(tReference))
            {
                throw new InvalidOperationException(
                    $"All references of the same type ({tReference.Name}) must be loaded at the same time."
                );
            }

            _referenceDictionaryByReferenceType.Add(tReference, referencesById);
        }

        private IDictionary<TId, TReference> GetReferenceDictionary<TReference, TId>()
        {
            var tReference = typeof(TReference);
            if (!_referenceDictionaryByReferenceType.ContainsKey(tReference))
            {
                throw new InvalidOperationException(
                    $"References of type {tReference.Name} were not loaded. Note that the implementation of IReferenceLoader must invoke LoadedReferenceContext.AddReferences with an empty set if none of the ids provided in the LookupIdContext for a specific reference type can be loaded."
                );
            }

            return (IDictionary<TId, TReference>) _referenceDictionaryByReferenceType[tReference];
        }

        public TReference GetOptionalReference<TReference, TId>(TId lookupId)
        {
            if (lookupId == null) return default;

            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();

            return referenceDictionnary.ContainsKey(lookupId) ? referenceDictionnary[lookupId] : default;
        }

        public IReadOnlyList<TReference> GetOptionalReferences<TReference, TId>(IEnumerable<TId> lookupIds)
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
                loadLinkExpression.LinkNestedLinkedSourceFromModel(
                    linkedSource,
                    this,
                    loadLinkProtocol
                );
        }
    }
}
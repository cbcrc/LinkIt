using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public class ReferenceTypeConfig<TReference, TId> : IReferenceTypeConfig
    {
        private readonly Func<List<TId>, List<TReference>> _loadReferencesFunc;

        public ReferenceTypeConfig(
            Func<TReference, TId> getReferenceIdFunc, 
            Func<List<TId>, List<TReference>> loadReferencesFunc)
        {
            GetReferenceIdFunc = getReferenceIdFunc;
            _loadReferencesFunc = loadReferencesFunc;
        }

        public Func<TReference, TId> GetReferenceIdFunc { get; private set; }

        public Func<List<TId>, List<TReference>> LoadReferencesFunc
        {
            get { return LoadReferencesWithCleanedIds; } 
        }

        private List<TReference> LoadReferencesWithCleanedIds(List<TId> ids)
        {
            var cleanedIds = ids
                .Where(id => id != null)
                .Distinct()
                .ToList();

            return _loadReferencesFunc(cleanedIds);
        }

        public void LoadReferences(object linkedSource, List<ILoadLinkExpression> loadExpressions, LoadedReferenceContext loadedReferenceContext)
        {
            EnsureTIdIsUsedForLookupIds(loadExpressions);

            var loadExpressionsOfTReference = GetLoadExpressionsOfTReference(loadExpressions);

            List<TId> lookupIds = loadExpressionsOfTReference
                .SelectMany(loadExpression => loadExpression.GetLookupIds(linkedSource))
                .ToList();

            var cleanedIds = CleanedIds(lookupIds);

            var references = LoadReferencesFunc(cleanedIds);

            loadedReferenceContext.AddReferences(references, GetReferenceIdFunc);
        }

        private List<ILoadLinkExpression<TId>> GetLoadExpressionsOfTReference(List<ILoadLinkExpression> loadExpressions) {
            return GetLoadExpressionsOfTReferenceUncasted(loadExpressions)
                .Cast<ILoadLinkExpression<TId>>()
                .ToList();
        }

        private void EnsureTIdIsUsedForLookupIds(List<ILoadLinkExpression> loadExpressions) {
            if (GetLoadExpressionsOfTReferenceUncasted(loadExpressions)
                .Any(loadExpression => !(loadExpression is ILoadLinkExpression<TId>)))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "All load expressions for the reference type {0} must use {1} for their lookup ids.", 
                        typeof(TReference).Name,
                        typeof(TId).Name
                    )
                );
            }
        }

        private List<ILoadLinkExpression> GetLoadExpressionsOfTReferenceUncasted(List<ILoadLinkExpression> loadExpressions)
        {
            return loadExpressions
                .Where(loadExpression => loadExpression.ReferenceType == typeof (TReference))
                .ToList();
        }

        private static List<TId> CleanedIds(List<TId> lookupIds)
        {
            return lookupIds
                .Where(id => id != null)
                .Distinct()
                .ToList();
        }
    }
}
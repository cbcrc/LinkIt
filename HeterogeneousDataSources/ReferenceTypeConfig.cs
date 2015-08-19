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

        public void LoadReferences(object linkedSource, List<ILoadLinkExpression> loadExpressions, DataContext dataContext)
        {
            var loadExpressionsOfTReference = loadExpressions
                .Where(loadExpression => loadExpression.ReferenceType == typeof(TReference))
                //stle: write an explicit preconditions here
                .Cast<ILoadLinkExpression<TId>>()
                .ToList();

            List<TId> lookupIds = loadExpressionsOfTReference
                .SelectMany(loadExpression => loadExpression.GetLookupIds(linkedSource))
                .ToList();

            var cleanedIds = CleanedIds(lookupIds);

            var references = LoadReferencesFunc(cleanedIds);

            dataContext.Append(references, GetReferenceIdFunc);
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
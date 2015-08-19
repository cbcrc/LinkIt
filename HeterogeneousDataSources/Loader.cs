using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public class Loader
    {
        public DataContext Load<TLinkedSource, TReference, TId>(
            TLinkedSource linkedSource, 
            List<ILoadExpression<TLinkedSource, TId>> loadExpressions,
            ReferenceTypeConfig<TReference, TId> referenceTypeConfig)
        {
            var dataContext = new DataContext();
            List<TId> referenceIds = loadExpressions
                .SelectMany(loadExpression => loadExpression.GetLookupIds(linkedSource))
                .ToList();

            var references = referenceTypeConfig.LoadReferencesFunc(referenceIds);

            var referencesAsTReference = references.ToList();

            dataContext.Append(referencesAsTReference, referenceTypeConfig.GetReferenceIdFunc);

            return dataContext;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests
{
    public class Loader
    {
        private readonly Dictionary<Type, IReferenceLoader> _referenceLoaders = new Dictionary<Type, IReferenceLoader>
        {
            {typeof (Image), new ImageRepository()}
        };

        public DataContext Load<TLinkedSource, TReference>(
            TLinkedSource linkedSource, 
            List<ILoadExpression<TLinkedSource>> loadExpressions,
            FutureReferenceLoader<TReference> futureReferenceLoader)
        {
            var tReference = typeof(TReference);
            var dataContext = new DataContext();
            foreach (var loadExpression in loadExpressions)
            {
                if (!_referenceLoaders.ContainsKey(tReference)){
                    throw new InvalidOperationException(string.Format("No reference loader exists for {0}",  tReference.Name));
                }
                var referenceLoader = _referenceLoaders[tReference];

                var referenceIds = loadExpression.GetLookupIds(linkedSource);
                var references = referenceLoader.LoadReferences(referenceIds);

                var referencesAsTReference = references
                    .Cast<TReference>()
                    .ToList();

                dataContext.Append(referencesAsTReference, futureReferenceLoader.GetReferenceIdFunc);
            }

            return dataContext;
        }
    }
}
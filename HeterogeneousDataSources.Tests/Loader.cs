using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.Tests
{
    public class Loader
    {
        private readonly Dictionary<Type, IReferenceLoader> _referenceLoaders = new Dictionary<Type, IReferenceLoader>
        {
            {typeof (Image), new ImageRepository()}
        };

        public DataContext Load(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions)
        {
            var dataContext = new DataContext();
            foreach (var loadLinkExpression in loadLinkExpressions)
            {
                if (!_referenceLoaders.ContainsKey(loadLinkExpression.ReferenceType)){
                    throw new InvalidOperationException(string.Format("No reference loader exists for {0}", loadLinkExpression.ReferenceType.Name));
                }
                var referenceLoader = _referenceLoaders[loadLinkExpression.ReferenceType];

                var referenceIds = loadLinkExpression.GetLookupIds(linkedSource);
                var references = referenceLoader.LoadReferences(referenceIds);

                dataContext.Append(references, loadLinkExpression);
            }

            return dataContext;
        }
    }
}
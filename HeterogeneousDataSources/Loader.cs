using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public class Loader
    {
        public void Load(
            object linkedSource, 
            List<IReferenceTypeConfig> referenceTypeConfigs, 
            List<ILoadLinkExpression> loadExpressions, 
            DataContext dataContext)
        {
            foreach (var referenceTypeConfig in referenceTypeConfigs) {
                referenceTypeConfig.LoadReferences(linkedSource, loadExpressions, dataContext);
            }
        }
    }
}
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public class Linker
    {
        public void Link(DataContext dataContext, object linkedSource, List<ILoadLinkExpression> linkExpressions)
        {
            foreach (var linkExpression in linkExpressions)
            {
                linkExpression.Link(linkedSource, dataContext);
            }
        }

    }
}
using System.Collections.Generic;

namespace HeterogeneousDataSources.Tests
{
    public class Linker
    {
        public void Link(DataContext dataContext, ContentLinkedSource linkedSource, List<ILoadLinkExpression> loadLinkExpressions)
        {
            foreach (var loadLinkExpression in loadLinkExpressions)
            {
                loadLinkExpression.Link(linkedSource, dataContext);
            }
        }

    }
}
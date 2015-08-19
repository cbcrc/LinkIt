using System.Collections.Generic;

namespace HeterogeneousDataSources.Tests
{
    public class Linker
    {
        public void Link<TLinkedSource>(DataContext dataContext, TLinkedSource linkedSource, List<ILinkExpression<TLinkedSource>> linkExpressions)
        {
            foreach (var linkExpression in linkExpressions)
            {
                linkExpression.Link(linkedSource, dataContext);
            }
        }

    }
}
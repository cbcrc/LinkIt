using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeSet
    {
        List<TInclude> GetIncludes<TInclude>() 
            where TInclude : class,IInclude;
    }
}
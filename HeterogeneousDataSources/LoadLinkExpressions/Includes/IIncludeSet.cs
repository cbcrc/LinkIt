using System.Collections.Generic;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface IIncludeSet
    {
        List<TInclude> GetIncludes<TInclude>() 
            where TInclude : class,IInclude;
    }
}
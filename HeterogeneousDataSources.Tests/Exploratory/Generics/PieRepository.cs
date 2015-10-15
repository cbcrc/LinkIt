using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests.Exploratory.Generics
{
    public class PieRepository<T>
    {
        public List<Pie<T>> GetByPieContentIds(List<string> ids)
        {
            return ids
                .Select(id => new Pie<T>(id))
                .ToList();
        }
    }
}
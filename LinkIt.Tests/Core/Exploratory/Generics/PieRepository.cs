using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Tests.Core.Exploratory.Generics
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
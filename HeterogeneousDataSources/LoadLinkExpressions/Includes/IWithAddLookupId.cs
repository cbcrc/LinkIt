using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes {

    //stle: use func instead of one method interface
    public interface IWithAddLookupId<TLink> {
        Type ReferenceType { get; }
        void AddLookupId(TLink link, LookupIdContext lookupIdContext);
    }
}

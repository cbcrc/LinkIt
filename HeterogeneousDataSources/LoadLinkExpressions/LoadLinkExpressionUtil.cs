using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    //stle: refactor that
    //stle: hey you and your inheritance crap! Try a functional approach
    internal static class LoadLinkExpressionUtil {
        internal static TNestedLinkedSource CreateNestedLinkedSource<TNestedLinkedSource, TNestedLinkedSourceModel>(List<TNestedLinkedSourceModel> references)
            where TNestedLinkedSource : ILinkedSource<TNestedLinkedSourceModel>, new()
        {
            var model = references.SingleOrDefault();

            if (model == null) { return default(TNestedLinkedSource); }

            return new TNestedLinkedSource{
                Model = model
            };
        }
    }
}

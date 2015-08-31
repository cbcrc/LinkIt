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
            where TNestedLinkedSource : class, ILinkedSource<TNestedLinkedSourceModel>, new()
        {
            var model = references.SingleOrDefault();

            if (model == null) { return null; }

            return new TNestedLinkedSource{
                Model = model
            };
        }
    }
}

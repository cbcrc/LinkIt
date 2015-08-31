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

            return CreateLinkedSource<TNestedLinkedSource, TNestedLinkedSourceModel>(model);
        }

        internal static TLinkedSource CreateLinkedSource<TLinkedSource, TLinkedSourceModel>(TLinkedSourceModel model)
            where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new() 
        {
            if (model == null) { return null; }

            return new TLinkedSource {
                Model = model
            };
        }

        internal static void EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(object linkedSource) {
            if (!(linkedSource is TLinkedSource)) {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot invoke load-link expression for {0} with linked source of type {1}",
                        typeof(TLinkedSource).Name,
                        linkedSource != null
                            ? linkedSource.GetType().Name
                            : "Null"
                        )
                    );
            }
        }


    }
}

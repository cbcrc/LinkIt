using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    //stle: refactor that
    //stle: hey you and your inheritance crap! Try a functional approach
    internal static class LoadLinkExpressionUtil {
        internal static List<TLinkedSource> CreateLinkedSource<TLinkedSource, TLinkedSourceModel>(TLinkedSourceModel model)
            where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
        {
            return CreateLinkedSources<TLinkedSource, TLinkedSourceModel>(
                new List<TLinkedSourceModel> {model}
            );
        }

        internal static List<TLinkedSource> CreateLinkedSources<TLinkedSource, TLinkedSourceModel>(List<TLinkedSourceModel> models)
            where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
        {
            if (models == null) { return new List<TLinkedSource>(); }

            return models
                .Where(model => model != null)
                .Select(model => new TLinkedSource {Model = model})
                .ToList();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    //stle: refactor that
    //stle: hey you and your inheritance crap! Try a functional approach
    internal static class LoadLinkExpressionUtil {
        internal static List<TLinkedSource> CreateLinkedSources<TLinkedSource, TLinkedSourceModel>(List<TLinkedSourceModel> models, LoadedReferenceContext loadedReferenceContext)
            where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new()
        {
            if (models == null) { return new List<TLinkedSource>(); }

            var linkedSources = models
                .Where(model => model != null)
                .Select(model => new TLinkedSource {Model = model})
                .ToList();

            loadedReferenceContext.AddLinkedSourcesToBeBuilt(linkedSources);

            return linkedSources;
        }

        internal static void EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(object linkedSource) {
            if (!(linkedSource is TLinkedSource)) {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot invoke load-link expression for {0} with linked source of type {1}",
                        typeof(TLinkedSource),
                        linkedSource != null
                            ? linkedSource.GetType().ToString()
                            : "Null"
                        )
                    );
            }
        }

        //stle: consider using extensions methods instead of inheritance
        internal static void EnsureIsOfReferenceType(ILoadLinkExpression loadLinkExpression, Type referenceType) {
            if (!loadLinkExpression.ReferenceTypes.Contains(referenceType)) {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot invoke this load link expression for reference type {0}. Supported reference types are {1}. This load link expression is for {2}.",
                        referenceType,
                        String.Join(",",loadLinkExpression.ReferenceTypes),
                        loadLinkExpression.LinkedSourceType
                    )
                );
            }
        }

        //stle: functional approach is coming!
        internal static List<TId> GetCleanedLookupIds<TId>(List<TId> lookupIds){
            if (lookupIds == null) { return new List<TId>(); }

            return lookupIds
                .Where(id => id != null)
                .ToList();
        }

    }
}

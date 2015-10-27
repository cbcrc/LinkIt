using System;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    //stle: refactor that
    //stle: hey you and your inheritance crap! Try a functional approach
    internal static class LoadLinkExpressionUtil {

        internal static TLinkedSource CreateLinkedSource<TLinkedSource, TLinkedSourceModel>(TLinkedSourceModel model, LoadedReferenceContext loadedReferenceContext)
            where TLinkedSource : class, ILinkedSource<TLinkedSourceModel>, new() 
        {
            if (model == null) { return null; }

            var linkedSource = new TLinkedSource {Model = model};

            loadedReferenceContext.AddLinkedSourceToBeBuilt(linkedSource);

            return linkedSource;
        }
    }
}

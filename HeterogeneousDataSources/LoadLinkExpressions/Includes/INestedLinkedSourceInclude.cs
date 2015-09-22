namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface INestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink>: IInclude
    {
        void AddLookupId(TLink link, LookupIdContext lookupIdContext);

        TIChildLinkedSource CreateChildLinkedSource(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex
        );
        
    }
}
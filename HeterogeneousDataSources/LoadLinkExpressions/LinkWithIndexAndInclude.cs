namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public class LinkWithIndexAndInclude<TLink,TInclude>{
        //stle: include default is temp
        public LinkWithIndexAndInclude(TLink link, int index, TInclude include){
            Link = link;
            Index = index;
            Include = include;
        }

        public TLink Link { get; private set; }
        public int Index { get; private set; }
        public TInclude Include { get; private set; }
    }
}
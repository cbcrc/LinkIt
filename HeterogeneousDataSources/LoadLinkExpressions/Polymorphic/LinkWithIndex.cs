namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public class LinkWithIndex<TLink>{
        public LinkWithIndex(TLink link, int index){
            Link = link;
            Index = index;
        }

        public TLink Link { get; private set; }
        public int Index { get; private set; }
    }
}
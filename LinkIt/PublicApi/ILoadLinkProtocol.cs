namespace LinkIt.PublicApi
{
    //Responsible for creating load linkers
    public interface ILoadLinkProtocol
    {
        ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>();
    }
}
namespace LinkIt.PublicApi
{
    //Responsible for creating a load linker for any root linked source type
    public interface ILoadLinkProtocol
    {
        ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>();
        LoadLinkProtocolStatistics Statistics { get; }
    }
}
namespace LinkIt.PublicApi
{
    public interface ILoadLinkProtocol
    {
        ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>();
    }
}
namespace LinkIt.PublicApi {
    public interface ILinkedSource<TModel> {
        TModel Model { get; set; }
    }
}

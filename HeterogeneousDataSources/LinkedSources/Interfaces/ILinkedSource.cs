namespace HeterogeneousDataSources.LinkedSources {
    public interface ILinkedSource<TModel> {
        TModel Model { get; set; }
    }
}

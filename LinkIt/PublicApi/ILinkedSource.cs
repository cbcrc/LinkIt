namespace LinkIt.PublicApi {
    //Responsible for defining the link targets of a model
    public interface ILinkedSource<TModel> {
        TModel Model { get; set; }
    }
}

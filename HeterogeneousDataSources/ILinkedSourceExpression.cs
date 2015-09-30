namespace HeterogeneousDataSources
{
    public interface ILinkedSourceExpression<TLinkedSource>
    {
        TLinkedSource CreateLinkedSource(object model, LoadedReferenceContext loadedReferenceContext);
    }
}
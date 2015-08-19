namespace HeterogeneousDataSources
{
    public interface ILinkExpression<TLinkedSource> {
        void Link(TLinkedSource linkedSource, DataContext dataContext);
    }
}
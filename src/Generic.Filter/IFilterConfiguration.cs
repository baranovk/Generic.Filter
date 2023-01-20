namespace Generic.Filter
{
    public interface IFilterConfiguration<TItem, TFilter> where TFilter : GenericFilter<TItem, TFilter>
    {
        TFilter CreateFilter();
    }
}

namespace Generic.Filter
{
    public interface IFilterConfiguration<TItem, TFilter> where TFilter : GenericFilter<TItem, TFilter>
    {
        TFilter CreateFilter();

        IFilterConfiguration<TItem, TFilter> ForMember();
    }
}

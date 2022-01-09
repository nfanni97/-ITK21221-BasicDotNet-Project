namespace RegistryApp.Filters
{
    public class GenericQueryOption<TFilter>
    {
        public TFilter Filter { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }
}
namespace RegistryApp.Filters
{
    public class ProductFilter
    {
        public string NameTerm { get; set; } = "";
        public string InDescription { get; set; } = "";
        public int? MinPriceHuf { get; set; } = 0;
        public int MaxPriceHuf { get; set; } = Int32.MaxValue;
    }
}
namespace RegistryApp.Vms
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int priceHuf { get; set; }
        public List<string> CategoryNames { get; set; }
    }
}
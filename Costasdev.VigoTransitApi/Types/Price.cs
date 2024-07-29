namespace Costasdev.VigoTransitApi.Types
{
    public class Price
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public Price(string name, string description, decimal amount)
        {
            Name = name;
            Description = description;
            Amount = amount;
        }
    }
}
namespace Costasdev.VigoTransitApi.Types;

public class Atm
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int PostCode { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public Atm(int id, string name, string description, int postCode, string city, string province, decimal latitude, decimal longitude)
    {
        Id = id;
        Name = name;
        Description = description;
        PostCode = postCode;
        City = city;
        Province = province;
        Latitude = latitude;
        Longitude = longitude;
    }
}
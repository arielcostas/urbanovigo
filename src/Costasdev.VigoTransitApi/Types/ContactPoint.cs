namespace Costasdev.VigoTransitApi.Types;

public class ContactPoint
{
    public int Id { get; set; }
    public string Telephone { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ContactPoint(int id, string telephone, string name, string description)
    {
        Id = id;
        Telephone = telephone;
        Name = name;
        Description = description;
    }
}
namespace Costasdev.VigoTransitApi.InternalTypes;

public class LineaServicio
{
    public string Linea { get; set; }
    public string? Color { get; set; }
    public string Tipo { get; set; }
    public string Subtipo { get; set; }
    public string Descripcion { get; set; }
    public string Imagen { get; set; }

    public LineaServicio(string linea, string? color, string tipo, string subtipo, string descripcion, string imagen)
    {
        Linea = linea;
        Color = color;
        Tipo = tipo;
        Subtipo = subtipo;
        Descripcion = descripcion;
        Imagen = imagen;
    }
}

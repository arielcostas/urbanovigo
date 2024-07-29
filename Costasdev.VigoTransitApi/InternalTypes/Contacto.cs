using System.Text.Json.Serialization;

namespace Costasdev.VigoTransitApi.InternalTypes
{
    public class Contacto
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("telefono")] public string Telefono { get; set; }

        [JsonPropertyName("nombre_es")] public string NombreEs { get; set; }
        [JsonPropertyName("nombre_ga")] public string NombreGa { get; set; }
        [JsonPropertyName("nombre_en")] public string NombreEn { get; set; }

        [JsonPropertyName("descripcion_es")] public string DescripcionEs { get; set; }
        [JsonPropertyName("descripcion_gl")] public string DescripcionGl { get; set; }
        [JsonPropertyName("descripcion_en")] public string DescripcionEn { get; set; }

        public Contacto(int id, string telefono, string nombreEs, string nombreGa, string nombreEn,
            string descripcionEs,
            string descripcionGl, string descripcionEn)
        {
            Id = id;
            Telefono = telefono;
            NombreEs = nombreEs;
            NombreGa = nombreGa;
            NombreEn = nombreEn;
            DescripcionEs = descripcionEs;
            DescripcionGl = descripcionGl;
            DescripcionEn = descripcionEn;
        }
    }
}
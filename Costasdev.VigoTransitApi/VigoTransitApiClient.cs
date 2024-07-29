using System.Text.Encodings.Web;
using Costasdev.VigoTransitApi.InternalTypes;
using Costasdev.VigoTransitApi.Types;

namespace Costasdev.VigoTransitApi
{
    public class VigoTransitApiClient
    {
        private readonly DataSource _dataSource;

        public VigoTransitApiClient()
        {
            _dataSource = new DataSource(new HttpClient());
        }

        public VigoTransitApiClient(HttpClient httpClient)
        {
            _dataSource = new DataSource(httpClient);
        }

        /// <summary>
        /// Gets the prices for the transportation service
        /// </summary>
        /// <param name="language">The language to get the prices in</param>
        /// <returns>A list of the prices for the transportation service</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the language is not supported</exception>
        public async Task<List<Price>> GetTransportPrices(Languages language)
        {
            var languageType = language switch
            {
                Languages.Es => "TRANSPORTE_TARIFAS_ES",
                Languages.Gl => "TRANSPORTE_TARIFAS_GL",
                Languages.En => "TRANSPORTE_TARIFAS_EN",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            };

            return await _dataSource.GetData<List<Price>>(languageType) ?? [];
        }

        /// <summary>
        /// Gets the contact points for the transportation service
        /// </summary>
        /// <param name="language">The language to get the contact point description in</param>
        /// <returns>A list of contact points</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the language is not supported</exception>
        /// <remarks>The contact points are retrieved via TRANSPORTE_CONTACTO but we return only the fields for the language requested</remarks>
        public async Task<List<ContactPoint>> GetContactInformation(Languages language)
        {
            var rawInfo = await _dataSource.GetData<List<InternalTypes.Contacto>>("TRANSPORTE_CONTACTO") ?? [];
            return rawInfo.Select(x =>
            {
                var name = language switch
                {
                    Languages.Es => x.NombreEs,
                    Languages.Gl => x.NombreGa,
                    Languages.En => x.NombreEn,
                    _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
                };
                var description = language switch
                {
                    Languages.Es => x.DescripcionEs,
                    Languages.Gl => x.DescripcionGl,
                    Languages.En => x.DescripcionEn,
                    _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
                };

                return new ContactPoint(x.Id, x.Telefono, name, description);
            }).ToList();
        }

        /// <summary>
        /// Gets the lines and services for the transportation service
        /// </summary>
        /// <param name="language">The language to get the services in. English is not supported.</param>
        /// <returns>A list of the lines and services</returns>
        /// <exception cref="NotImplementedException">If the language is English or an invalid value</exception>
        public async Task<List<LineService>> GetLinesAndServices(Languages language)
        {
            if (language is Languages.En)
                throw new ArgumentOutOfRangeException(nameof(language), "English is not supported for this method");

            var rawInfo = await _dataSource.GetData<List<LineaServicio>>("TRANSPORTE_LINEAS_SERVICIOS") ?? [];

            return rawInfo.Select(x =>
            {
                var type = x.Tipo switch
                {
                    "AEROPUERTO" => LineServiceType.Airport,
                    "AUDITORIO" => LineServiceType.Auditorium,
                    "CIRCULAR" => LineServiceType.Circular,
                    "CITEXVI" => LineServiceType.Citexvi,
                    "EST-BUSES" => LineServiceType.BusStation,
                    "HOSPITALES" => LineServiceType.Hospitals,
                    "MARITIMA" => LineServiceType.MaritimeStation,
                    "NOCTURNA" => LineServiceType.Night,
                    "PLAYAS" => LineServiceType.Beaches,
                    "PTL" => LineServiceType.Ptl,
                    "RENFE" => LineServiceType.TrainStation,
                    "UNIVERSIDAD" => LineServiceType.University,
                    _ => LineServiceType.Unknown
                };

                return new LineService(
                    x.Linea,
                    x.Color ?? "#000000",
                    type,
                    x.Subtipo,
                    x.Descripcion,
                    new Uri($"{LineService.ImageBaseUri}{x.Imagen}")
                );
            }).ToList();
        }
        
        
    }
}
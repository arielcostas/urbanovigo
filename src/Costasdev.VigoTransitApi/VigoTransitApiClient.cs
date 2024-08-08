using System.Text.Encodings.Web;
using Costasdev.VigoTransitApi.InternalTypes;
using Costasdev.VigoTransitApi.Types;

// ReSharper disable UnusedMember.Global
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
            var rawInfo = await _dataSource.GetData<List<Contacto>>("TRANSPORTE_CONTACTO") ?? [];
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
        /// Get the day type for the transportation service
        /// </summary>
        /// <returns>The day type for the transportation service today</returns>
        /// <exception cref="InvalidOperationException">Thrown when no day types are found</exception>
        /// <exception cref="NotImplementedException">The day type is not implemented. This method is WIP</exception>
        public async Task<DayType> GetDayType()
        {
            var tiposDias = await _dataSource.GetData<List<TipoDia>>("TRANSPORTE_TIPO_DIA");
            if (tiposDias == null || !tiposDias.Any())
                throw new InvalidOperationException("No day types were found");

            return tiposDias.First().Tipo switch
            {
                "L" => DayType.Weekday,
                _ => throw new NotImplementedException()
            };
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
                    language switch
                    {
                        Languages.Es => x.SubtipoEs,
                        Languages.Gl => x.SubtipoGl,
                        _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
                    },
                    x.Descripcion,
                    new Uri($"{LineService.ImageBaseUri}{x.Imagen}")
                );
            }).ToList();
        }

        /// <summary>
        /// Gets the next arrivals for a stop by its ID
        /// </summary>
        /// <param name="stopId">The ID of the stop to get the estimates for</param>
        /// <returns>The stop information and the estimates for the stop</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the stop ID is less than or equal to 0</exception>
        /// <exception cref="InvalidOperationException">Thrown when no stop information is found</exception>
        public async Task<StopEstimateResponse> GetStopEstimates(int stopId)
        {
            if (stopId <= 0)
                throw new ArgumentOutOfRangeException(nameof(stopId), "The stop ID must be greater than 0");

            var queryParams = new Dictionary<string, string>
            {
                { "tipo", "TRANSPORTE-ESTIMACION-PARADA" },
                { "id", stopId.ToString() },
                { "ttl", "1" }
            };

            var rawInfo = await _dataSource.GetDataWithParams<EstimacionesParadaResponse>(queryParams);

            var stop = rawInfo?.Parada.FirstOrDefault();

            if (stop is null)
                throw new InvalidOperationException("No stop information was found. The stop ID may not exist");

            return new StopEstimateResponse()
            {
                Stop = new StopEstimateResponse.StopInfo
                {
                    Name = stop.Nombre,
                    Id = stop.Id,
                    Latitude = stop.Latitud,
                    Longitude = stop.Longitud
                },
                Estimates = rawInfo?.Estimaciones.Select(x => new StopEstimate(x.Linea, x.Ruta, x.Minutos, x.Metros))
                    .ToList() ?? []
            };
        }

        /// <summary>
        /// Gets the ATMs that support charging the transportation card
        /// </summary>
        /// <returns>A list of ATMs that support charging the transportation card</returns>
        public async Task<List<Atm>> GetAtms()
        {
            var rawData = await _dataSource.GetData<List<Cajero>>("TRANSPORTE_CAJEROS") ?? [];

            return rawData.Select(x => new Atm(x.Id, x.Name, x.Descripcion, x.CodigoPostal, x.Poblacion, x.Provincia,
                x.Latitude, x.Longitude)).ToList();
        }

        /// <summary>
        /// Gets the stop by its ID
        /// </summary>
        /// <param name="id">The ID of the stop to get</param>
        /// <returns>The stop information</returns>
        public async Task<Stop> GetStopById(int id)
        {
            var dict = new Dictionary<string, string>
            {
                { "tipo", "TRANSPORTE_PARADA_ID" },
                { "id", id.ToString() }
            };
            var res = await _dataSource.GetDataWithParams<List<Parada>>(dict);
            if (res == null || !res.Any())
                throw new InvalidOperationException("No stop information was found. The stop ID may not exist");

            return res.Select(p => new Stop()
            {
                InternalStopId = p.StopId,
                StopId = p.Id,
                Name = p.Nombre,
                Latitude = p.Lat,
                Longitude = p.Lon
            }).First();
        }

        public async Task<List<Stop>> GetStops()
        {
            var res = await _dataSource.GetData<List<Parada>>("TRANSPORTE_PARADAS");
            if (res == null || !res.Any())
                throw new InvalidOperationException("No stop information was found");

            return res.Select(p => new Stop()
            {
                InternalStopId = p.StopId,
                StopId = p.Id,
                Name = p.Nombre,
                Latitude = p.Lat,
                Longitude = p.Lon,
                Lines = p.Lineas?.Split(',').Select(s => s.Trim()).ToList()
            }).ToList();
        }
    }
}
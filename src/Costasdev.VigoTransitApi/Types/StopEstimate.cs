namespace Costasdev.VigoTransitApi.Types;

public class StopEstimateResponse
{
       public StopInfo Stop { get; set; }
       public List<StopEstimate> Estimates { get; set; }

       public class StopInfo
       {
              public string Name { get; set; }
              public int Id { get; set; }
              public decimal Latitude { get; set; }
              public decimal Longitude { get; set; }
       }
}

public class StopEstimate
{
       public string Line { get; set; }
       public string Route { get; set; }
       public int Minutes { get; set; }
       public int Meters { get; set; }

       public StopEstimate(string line, string route, int minutes, int meters)
       {
              Line = line;
              Route = route;
              Minutes = minutes;
              Meters = meters;
       }
}
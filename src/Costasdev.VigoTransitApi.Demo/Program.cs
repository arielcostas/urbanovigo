using Costasdev.VigoTransitApi;
using Costasdev.VigoTransitApi.Types;

VigoTransitApiClient client = new();

var prices = await client.GetTransportPrices(Languages.Es);
var contactPoints = await client.GetContactInformation(Languages.Es);
var stopEstimates = await client.GetStopEstimates(1400);

Console.WriteLine("Prices:");
foreach (var price in prices)
{
    Console.WriteLine($"- {price.Name}: {price.Amount}");
}

Console.WriteLine("\nContact Points:");
foreach (var contactPoint in contactPoints)
{
    Console.WriteLine($"- {contactPoint.Name}: {contactPoint.Telephone}");
}

Console.WriteLine($"\nStop Estimates for stop {stopEstimates.Stop.Name}:");
foreach (var estimate in stopEstimates.Estimates)
{
    Console.WriteLine($"- {estimate.Line} ({estimate.Route}): {estimate.Minutes} minutes, {estimate.Meters} meters away");
}






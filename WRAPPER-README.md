# VigoTransitApi

This is a simple .NET library that allows you to interact with Vigo's urban transit API hosted
by the local government. It is a simple wrapper around the API that allows you to easily query
bus stops, lines, schedules and more.

## Installation

You can install this library via NuGet. Just search for `Costasdev.VigoTransitApi` and install it.

## Usage

You need to create an instance of the `VigoTransitApiClient` class and then you can start querying
the API. Here is an example:

```csharp
using System;
using System.Threading.Tasks;
using Costasdev.VigoTransitApi;

namespace VigoTransitApiExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new VigoTransitApiClient();
            var stops = await client.GetStops();
            foreach (var stop in stops)
            {
                Console.WriteLine(stop.Name);
            }
        }
    }
}
```

## License

This library is licenced under the BSD 3-Clause Licence. You can find the full text of the licence
in the `LICENSE` file.
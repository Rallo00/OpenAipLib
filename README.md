# OpenAipLib

This C# Library allows users to get information on the desidered airport by using OpenAIP provided APIs.

---- REQUISITES ----

Requires Newtonsoft.Json.dll to parse JSON information, get it from NuGet into Visual Studio.

---- HOW TO USE ----
- Add the provided DLL file as Reference into your C# project.
- Add `using OpenAipLib;` at the top of your Project code-behind file.
- Create a free token here → https://www.openaip.net/users/clients (need to register)

Call the APIs by doing so:
```cs
OpenAipLib.Airport airport = OpenAipLib.GetAirportInfo(airportICAOcode, API_TOKEN);
```

To avoid thread blocking I recommend to use async methods like so:
```cs
public async void GetAirportData(string ICAO, string API_TOKEN)
{
  OpenAipLib.Airport airport = await OpenAipLib.GetAirportInfo(airportICAOcode, API_TOKEN);
  //...other stuff
}
```

---- PROVIDED INFORMATION ----
- Name of the airport;
- ICAO code;
- IATA code;
- Country;
- Elevation (in meters);
- Latitude (°N);
- Longitude (°E);
- Radio frequencies (Type of radio frequency, Name of radio frequency, Radio Frequency);
- Runways (Runway number, True heading , Length of the runway, Width of the runway);

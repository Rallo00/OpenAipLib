using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAipLibTest
{
    internal class Program
    {
        static string API_TOKEN = "1eba052dd328ac3b9894d0c3d62678a6";
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Cerca un aeroporto (ICAO): ");
            string airportIcao = Console.ReadLine();
            GetAirportData(airportIcao.ToUpper());
            Console.ReadKey();
        }
        static async void GetAirportData(string icao)
        {
            OpenAipLib.Airport airport = await OpenAipLib.GetAirportInfoAsync(icao, API_TOKEN);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(">> AIRPORT RESULT");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"City: {airport.Name}");
            Console.WriteLine($"Country: {airport.Country}");
            Console.WriteLine($"ICAO: {airport.ICAOCode}");
            Console.WriteLine($"IATA: {airport.IATACode}");
            Console.WriteLine($"Coordinates: {airport.Latitude}°N, {airport.Longitude}°E");
            Console.WriteLine($"Elevation: {airport.Elevation} m");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Frequencies:");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (OpenAipLib.Frequency f in airport.Frequencies)
                Console.WriteLine($"{f.Name}\t{f.Type}\t{f.Value}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Runway:");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (OpenAipLib.Runway r in airport.Runways)
                Console.WriteLine($"{r.Identification1}\\{r.Identification2}\t{r.Bearing1}°\\{r.Bearing2}°\t{r.Length} m\t{r.Width} m\t{r.Surface}");
            Console.WriteLine("*************************************************************");
        }
    }
}

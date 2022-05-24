using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


//URL DI ESEMPIO: 
//https://api.core.openaip.net/api/airports?fields=_id%2Cname%2CicaoCode%2CiataCode%2Ccountry%2Cgeometry%2Celevation%2Cfrequencies%2Crunways%2Ccontact%2Cremarks&sortDesc=true&approved=true&searchOptLwc=true&search=lipr&apiKey=1eba052dd328ac3b9894d0c3d62678a6
//DOC:
//https://docs.openaip.net/#/Airports/get_airports

public static class OpenAipLib
{
    public static async Task<Airport> GetAirportInfo(string icao, string API_TOKEN)
    {
        string URI = $"https://api.core.openaip.net/api/airports?page=1&limit=1&fields=_id%2Cname%2CicaoCode%2CiataCode%2Ccountry%2Cgeometry%2Celevation%2Cfrequencies%2Crunways%2Ccontact%2Cremarks&sortDesc=true&approved=true&searchOptLwc=true&search={icao.ToUpper()}&apiKey={API_TOKEN}";
        string response = await Http_GetRequest(URI);
        //Converting single result (for sure it's single because prompted in query string as ICAO code is unique)
        OpenAipResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<OpenAipResult>(response);
        JsonAirport airport = result.AirportData[0];

        Airport convertedAirport = new Airport();
        convertedAirport.Name = airport.Name;
        convertedAirport.City = "";
        convertedAirport.ICAOCode = airport.Icao;
        convertedAirport.IATACode = airport.Iata;
        convertedAirport.Country = airport.Country;
        convertedAirport.Elevation = airport.Elevation.Value;
        convertedAirport.Frequencies = airport.Frequencies;
        //Fix Runway
        List<Runway> runways = new List<Runway>();
        for(int i = 0; i < airport.Runways.Count; i+=2)
        {
            string id1 = $"{airport.Runways[i].RunwayNumber}";
            string id2 = $"{ airport.Runways[i + 1].RunwayNumber}";
            string b1 = $"{airport.Runways[i].TrueHeading}";
            string b2 = $"{airport.Runways[i + 1].TrueHeading}";
            runways.Add(new Runway(id1, id2, b1, b2, airport.Runways[i].Dimensions.RwyLength.Value, airport.Runways[i].Dimensions.RwyWidth.Value, airport.Runways[i].Surface.MainComposite));
        }
        convertedAirport.Runways = runways;
        //Fix Surface
        foreach (Runway r in convertedAirport.Runways)
            switch(r.Surface)
            {
                case "0": r.Surface = "Asphalt"; break;
                case "1": r.Surface = "Concrete"; break;
                case "2": r.Surface = "Grass"; break;
                case "3": r.Surface = "Sand"; break;
                case "4": r.Surface = "Water"; break;
                case "5": r.Surface = "Tarmac"; break;
                case "6": r.Surface = "Brick"; break;
                case "7": r.Surface = "Macadam or tarmac"; break;
                case "8": r.Surface = "Stone"; break;
                case "9": r.Surface = "Coral"; break;
                case "10": r.Surface= "Clay"; break;
                case "11": r.Surface= "Laterite"; break;
                case "12": r.Surface= "Gravel"; break;
                case "13": r.Surface= "Earth"; break;
                case "14": r.Surface= "Ice"; break;
                case "15": r.Surface= "Snow"; break;
                case "16": r.Surface= "Protective laminate (Rubber)"; break;
                case "17": r.Surface= "Metal"; break;
                case "18": r.Surface= "Landing mat portable system (Aluminium)"; break;
                case "19": r.Surface= "Pierced steel planking"; break;
                case "20": r.Surface= "Wood"; break;
                case "21": r.Surface= "Non Bituminous mix"; break;
                case "22": r.Surface= "Unknown"; break;
            }
        //Fix Frequencies
        foreach(Frequency f in airport.Frequencies)
            switch (f.Type)
            {
                case "0": f.Type = "Approach"; break;
                case "1": f.Type = "APRON"; break;
                case "2": f.Type = "Arrival"; break;
                case "3": f.Type = "Center"; break;
                case "4": f.Type = "CTAF"; break;
                case "5": f.Type = "Delivery"; break;
                case "6": f.Type = "Departure"; break;
                case "7": f.Type = "FIS"; break;
                case "8": f.Type = "Gliding"; break;
                case "9": f.Type = "Ground"; break;
                case "10": f.Type = "Info"; break;
                case "11": f.Type = "Multicom"; break;
                case "12": f.Type = "Unicom"; break;
                case "13": f.Type = "Radar"; break;
                case "14": f.Type = "Tower"; break;
                case "15": f.Type = "ATIS"; break;
                case "16": f.Type = "Radio"; break;
                case "17": f.Type = "Other"; break;
                case "18": f.Type = "AIRMET"; break;
                case "19": f.Type = "AWOS"; break;
                case "20": f.Type = "Lights"; break;
                case "21": f.Type = "VOLMET"; break;
                default: f.Type = "Not available"; break;
            }

        //Fix Coordinates
        convertedAirport.Latitude = airport.Geometry.Coordinates[0];
        convertedAirport.Longitude = airport.Geometry.Coordinates[1];

        return convertedAirport;
    }
    private static async Task<string> Http_GetRequest(string URI)
    {
        string result;
        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URI);
        using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)await request.GetResponseAsync())
        using (System.IO.Stream stream = response.GetResponseStream())
        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            result = await reader.ReadToEndAsync();
        return result;
    }
    public class Airport
    {
        public string Name;
        public string City;
        public string ICAOCode;
        public string IATACode;
        public string Country;
        public double Elevation;
        public string Latitude;
        public string Longitude;
        public List<Frequency> Frequencies;
        public List<Runway> Runways;
        public Airport() { }
    }
    public class Elevation
    {
        [Newtonsoft.Json.JsonProperty("value")]     //OpenAIP gives back always meters
        public double Value;
        public Elevation(double v)
        {
            Value = v;
        }
    }
    public class Frequency
    {
        //"frequencies":[{"value":"121.600","unit":2,"type":9,"name":"RIMINI GND","primary":false,"publicUse":true,"_id":"62614c185e9ded571044b44e"},
        [Newtonsoft.Json.JsonProperty("value")]
        public string Value;
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name;
        [Newtonsoft.Json.JsonProperty("type")]
        public string Type;
        public Frequency(string v, string n, string t)
        {
            Value = v; Name = n;
        }
    }
    public class Runway
    {
        public string Identification1;
        public string Identification2;
        public string Bearing1;
        public string Bearing2;
        public string Length;           //OpenAIP gives back always METERS
        public string Width;            //
        public string Surface;
        public Runway(string id1, string id2, string b1, string b2, string l, string w, string s)
        {
            Identification1 = (id1 != null) ? id1 : "Not available";
            Identification2 = (id2 != null) ? id2 : "Not available";
            Bearing1 = (b1 != null) ? b1 : "Not available";
            Bearing2 = (b2 != null) ? b2 : "Not available";
            Length = (l != null) ? l : "Not available"; 
            Width = (w != null) ? w : "Not available";
            Surface = (s != null) ? s : "Not available";
        }
    }

    //PRIVATE CLASSES FOR INTERNAL USE AND AIRPORT STRUCTURE CONVERSION
    protected internal class OpenAipResult
    {
        [Newtonsoft.Json.JsonProperty("items")]
        protected internal List<JsonAirport> AirportData;

        [JsonConstructor]
        private OpenAipResult(List<JsonAirport> result)
        {
            AirportData = result;
        }
    }
    protected internal class JsonAirport
    {
        [Newtonsoft.Json.JsonProperty("name")]
        protected internal string Name;
        [Newtonsoft.Json.JsonProperty("icaoCode")]
        protected internal string Icao;
        [Newtonsoft.Json.JsonProperty("iataCode")]
        protected internal string Iata;
        [Newtonsoft.Json.JsonProperty("country")]
        protected internal string Country;
        [Newtonsoft.Json.JsonProperty("elevation")]
        protected internal Elevation Elevation;
        [Newtonsoft.Json.JsonProperty("frequencies")]
        protected internal List<Frequency> Frequencies;
        [Newtonsoft.Json.JsonProperty("runways")]
        protected internal List<JsonRunway> Runways;
        [Newtonsoft.Json.JsonProperty("geometry")]
        protected internal JsonGeometry Geometry;

        [JsonConstructor]
        private JsonAirport(string n, string icao, string iata, string c, /*string coord,*/ Elevation elev, List<Frequency> freq)
        {
            Name = n; Icao = icao; Iata = iata; Country = c; /*Coordinates = coord;*/ Elevation = elev; Frequencies = freq;
        }
    }
    protected internal class JsonRunway
    {
        [Newtonsoft.Json.JsonProperty("designator")]
        protected internal string RunwayNumber;
        [Newtonsoft.Json.JsonProperty("trueHeading")]
        protected internal string TrueHeading;
        [Newtonsoft.Json.JsonProperty("dimension")]
        protected internal JsonRwyDimensions Dimensions;
        [Newtonsoft.Json.JsonProperty("surface")]
        protected internal JsonRwySurface Surface;


        [JsonConstructor]
        private JsonRunway(string r, string th, JsonRwyDimensions rd) { RunwayNumber = r; TrueHeading = th; Dimensions = rd; }
    }
    protected internal class JsonRwyDimensions
    {
        [Newtonsoft.Json.JsonProperty("length")]
        protected internal JsonRwyLength RwyLength;
        [Newtonsoft.Json.JsonProperty("width")]
        protected internal JsonRwyWidth RwyWidth;
    }
    protected internal class JsonRwySurface
    {
        [Newtonsoft.Json.JsonProperty("mainComposite")]
        protected internal string MainComposite;
    }
    protected internal class JsonRwyLength
    {
        [Newtonsoft.Json.JsonProperty("value")]
        protected internal string Value;
        [Newtonsoft.Json.JsonProperty("unit")]
        protected internal string Unit;
    }
    protected internal class JsonRwyWidth
    {
        [Newtonsoft.Json.JsonProperty("value")]
        protected internal string Value;
        [Newtonsoft.Json.JsonProperty("unit")]
        protected internal string Unit;
    }
    protected internal class JsonGeometry
    {
        [Newtonsoft.Json.JsonProperty("type")]
        protected internal string Type;
        [Newtonsoft.Json.JsonProperty("coordinates")]
        protected internal List<string> Coordinates;
    }
}
//static class that represents the weather forecast object.
// The object has properties that are used by the weather forcast controller. 
//
//iterator 

namespace BookStoreApp.API
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}

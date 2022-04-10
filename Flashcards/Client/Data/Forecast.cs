namespace Flashcards.Client.Data
{
    public class Forecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string? Summary { get; set; }
    }
}

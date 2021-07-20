namespace FlightPlanner.Models
{
    public class SearchFlightsRequest
    {
        public string From { set; get; }
        public string To { set; get; }
        public string DepartureDate { set; get; }
    }
}
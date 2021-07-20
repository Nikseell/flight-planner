using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace FlightPlanner.Models
{
    public class FlightStorage : ApiController
    {
        public static List<Airport> foundAirpots = new List<Airport>();
        public static List<string> checkNames = new List<string>();
        public static List<Flight> allFlights = new List<Flight>();
        public static List<Flight> foundByReq = new List<Flight>();
        private static int _id;

        public static Flight AddFlight(Flight flight)
        {
            flight.Id = _id;
            _id++;
            allFlights.Add(flight);

            return flight;
        }

        public static Flight FindFlight(int id)
        {
            return allFlights.FirstOrDefault(x => x.Id == id);
        }

        public static List<Airport> SearchAirports(string name)
        {
            foreach (var f in allFlights)
            {
                if (f.From.AirportName.ToLower().Trim().Contains(name.ToLower().Trim())
                )
                {
                    if (!(checkNames.Contains(f.From.AirportName.ToLower().Trim())))
                    {
                        foundAirpots.Add(f.From);
                        checkNames.Add(f.From.AirportName.ToLower().Trim());
                    }
                }

                if (f.To.AirportName.ToLower().Trim().Contains(name.ToLower().Trim())
                    )
                {
                    if (!(checkNames.Contains(f.To.AirportName.ToLower().Trim())))
                    {
                        foundAirpots.Add(f.To);
                        checkNames.Add(f.To.AirportName.ToLower().Trim());
                    }
                }
            }

            return foundAirpots;
        }

        public static List<Flight> SearchFlights(SearchFlightsRequest searchFlightsRequest)
        {
            foreach(var f in allFlights)
            {
                if(f.From.AirportName == searchFlightsRequest.From &&
                    f.To.AirportName == searchFlightsRequest.To &&
                    f.DepartureTime.Substring(0 ,10) == searchFlightsRequest.DepartureDate)
                {
                    foundByReq.Add(f);
                }
            }

            return foundByReq;
        }

        public static bool CheckIfNotNullAndIfCorrectFromTo(SearchFlightsRequest searchFlightsRequest)
        {
            if(searchFlightsRequest == null)
            {
                return true;
            }
            if(searchFlightsRequest.From == null || searchFlightsRequest.To == null || searchFlightsRequest.DepartureDate == null)
            {
                return true;
            }
            if (searchFlightsRequest.To == searchFlightsRequest.From)
            {
                return true;
            }

            return false;
        }
    }
}

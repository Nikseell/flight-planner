using FlightPlanner.DbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
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

            using (var ctx = new FlightPlannerDbContext())
            {
                ctx.Flights.Add(flight);
                ctx.SaveChanges();
            }

            return flight;
        }

        public static Flight FindFlight(int id)
        {
            using (var ctx = new FlightPlannerDbContext())
            {
                return ctx.Flights.Include(x => x.To).Include(x => x.From).SingleOrDefault(x => x.Id == id);
            }
        }

        public static void DeleteFlight(int id)
        {
            using (var ctx = new FlightPlannerDbContext())
            {
                var flight = FindFlight(id);
                if (flight != null)
                {
                    ctx.Flights.Attach(flight);
                    ctx.Flights.Remove(flight);
                    ctx.SaveChanges();
                }
            }
        }

        public static List<Airport> SearchAirports(string name)
        {
            using (var ctx = new FlightPlannerDbContext())
            {
                foreach (var f in ctx.Flights.Include(x => x.To).Include(x => x.From))
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
        }


        public static List<Flight> SearchFlights(SearchFlightsRequest searchFlightsRequest)
        {
            using (var ctx = new FlightPlannerDbContext())
            {

                foreach (var f in ctx.Flights.Include(x => x.From).Include(x => x.To))
                {
                    if (f.From.AirportName == searchFlightsRequest.From &&
                        f.To.AirportName == searchFlightsRequest.To &&
                        f.DepartureTime.Substring(0, 10) == searchFlightsRequest.DepartureDate)
                    {
                        foundByReq.Add(f);
                    }
                }

                return foundByReq;
            }

        }

        public static bool CheckIfNotNullAndIfCorrectFromTo(SearchFlightsRequest searchFlightsRequest)
        {
            if (searchFlightsRequest == null)
                {
                    return true;
                }
                if (searchFlightsRequest.From == null || searchFlightsRequest.To == null || searchFlightsRequest.DepartureDate == null)
                {
                    return true;
                }
                if (searchFlightsRequest.To == searchFlightsRequest.From)
                {
                    return true;
                }

                return false;

            }

            public static bool DoesContainWrongValues(AddFlightRequest flight)
            {
                if (flight.From == null ||
                    flight.To == null ||
                    string.IsNullOrEmpty(flight.Carrier) ||
                    string.IsNullOrEmpty(flight.DepartureTime) ||
                    string.IsNullOrEmpty(flight.ArrivalTime) ||
                    string.IsNullOrEmpty(flight.To.Country) ||
                    string.IsNullOrEmpty(flight.To.City) ||
                    string.IsNullOrEmpty(flight.To.AirportName) ||
                    string.IsNullOrEmpty(flight.From.Country) ||
                    string.IsNullOrEmpty(flight.From.City) ||
                    string.IsNullOrEmpty(flight.From.AirportName)
                )
                {
                    return true;
                }

                return false;
            }

            public static bool IsTheSameFlight(AddFlightRequest flight)
            {
                using (var ctx = new FlightPlannerDbContext())
                {
                    if (ctx.Flights.Any(x =>
                        flight.From.Country == x.From.Country &&
                        flight.From.City == x.From.City &&
                        flight.From.AirportName == x.From.AirportName &&
                        flight.To.Country == x.To.Country &&
                        flight.To.City == x.To.City &&
                        flight.To.AirportName == x.To.AirportName &&
                        flight.Carrier == x.Carrier &&
                        flight.DepartureTime == x.DepartureTime &&
                        flight.ArrivalTime == x.ArrivalTime))
                    {
                        return true;
                    }

                    return false;
                }
            }

            public static bool HaveSameAirports(AddFlightRequest flight)
            {
                if (flight.From.Country.ToLower().Trim() == flight.To.Country.ToLower().Trim() &&
                    flight.From.City.ToLower().Trim() == flight.To.City.ToLower().Trim() &&
                    flight.From.AirportName.ToLower().Trim() == flight.To.AirportName.ToLower().Trim())
                {
                    return true;
                }

                return false;
            }
            public static bool IsCorrectDate(AddFlightRequest flight)
            {
                DateTime arrivalTime = DateTime.Parse(flight.ArrivalTime);
                DateTime departureTime = DateTime.Parse(flight.DepartureTime);

                if (departureTime > arrivalTime || departureTime == arrivalTime)
                {
                    return true;
                }

                return false;
            }
        }
    }


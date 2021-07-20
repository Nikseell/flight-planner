using System;
using System.Web.Http;
using FlightPlanner.Attributes;
using FlightPlanner.Models;

namespace FlightPlanner.Controllers
{
    [BasicAuthentication]
    public class AdminApiController : ApiController
    {
        private static readonly object _locker = new object();

        [Route("admin-api/flights/{id}")]
        public IHttpActionResult GetFlights(int id)
        {
            lock (_locker)
            {
                var flight = FlightStorage.FindFlight(id);
                if (flight == null)
                {
                    return NotFound();
                }

                return Ok();
            }
        }

        [Route("admin-api/flights/{id}"), HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            lock (_locker)
            {
                var flight = FlightStorage.FindFlight(id);
                if (flight != null)
                {
                    FlightStorage.allFlights.Remove(flight);
                }

                return Ok();
            }
        }

        [Route("admin-api/flights")]
        public IHttpActionResult PutFlight(AddFlightRequest flight)
        {
            lock (_locker)
            {
                if (DoesContainWrongValues(flight) || HaveSameAirports(flight) || IsCorrectDate(flight))
                {
                    return BadRequest();
                }

                if (IsTheSameFlight(flight))
                {
                    return Conflict();
                }

                Flight result = new Flight
                {
                    From = flight.From,
                    To = flight.To,
                    Carrier = flight.Carrier,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime
                };

                FlightStorage.AddFlight(result);

                return Created("", result);
            }
        }

        private bool DoesContainWrongValues(AddFlightRequest flight)
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

        private bool IsTheSameFlight(AddFlightRequest flight)
        {
            foreach (var f in FlightStorage.allFlights)
            {
                if (flight.From.Country == f.From.Country &&
                    flight.From.City == f.From.City &&
                    flight.From.AirportName == f.From.AirportName &&
                    flight.To.Country == f.To.Country &&
                    flight.To.City == f.To.City &&
                    flight.To.AirportName == f.To.AirportName &&
                    flight.Carrier == f.Carrier &&
                    flight.DepartureTime == f.DepartureTime &&
                    flight.ArrivalTime == f.ArrivalTime
                )
                {
                    return true;
                }
            }
            return false;
        }

        private bool HaveSameAirports(AddFlightRequest flight)
        {
            if (flight.From.Country.ToLower().Trim() == flight.To.Country.ToLower().Trim() &&
                flight.From.City.ToLower().Trim() == flight.To.City.ToLower().Trim() &&
                flight.From.AirportName.ToLower().Trim() == flight.To.AirportName.ToLower().Trim())
            {
                return true;
            }

            return false;
        }
        private bool IsCorrectDate(AddFlightRequest flight)
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
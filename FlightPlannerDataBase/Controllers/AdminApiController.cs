using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using FlightPlanner.Attributes;
using FlightPlanner.DbContext;
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
                FlightStorage.DeleteFlight(id);
                return Ok();
            }
        }

        [Route("admin-api/flights")]
        public IHttpActionResult PutFlight(AddFlightRequest flight)
        {
            lock (_locker)
            {
                if (FlightStorage.DoesContainWrongValues(flight) || FlightStorage.HaveSameAirports(flight) || FlightStorage.IsCorrectDate(flight))
                {
                    return BadRequest();
                }

                if (FlightStorage.IsTheSameFlight(flight))
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
    }
}
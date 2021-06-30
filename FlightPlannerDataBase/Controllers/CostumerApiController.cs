using System.Web.Http;
using FlightPlanner.DbContext;
using FlightPlanner.Models;
using Microsoft.Ajax.Utilities;

namespace FlightPlanner.Controllers
{
    public class CostumerApiController : ApiController
    {
        private static readonly object _locker = new object();

        [Route("api/airports"), HttpGet]
        public IHttpActionResult SearchAirports(string search)
        {
            lock (_locker)
            {
                var result = FlightStorage.SearchAirports(search);


                return Ok(result);
            }

        }

        [Route("api/flights/search"), HttpPost]
        public IHttpActionResult SearchFlightsByReq(SearchFlightsRequest searchFlightsRequest)
        {
            lock (_locker)
            {
                if (FlightStorage.CheckIfNotNullAndIfCorrectFromTo(searchFlightsRequest))
                {
                    return BadRequest();
                }

                var result = FlightStorage.SearchFlights(searchFlightsRequest);

                var pageResult = new PageResult(result);

                return Ok(pageResult);
            }
        }

        [Route("api/flights/{id}"), HttpGet]
        public IHttpActionResult FindFlightById(int id)
        {
            lock (_locker)
            {
                var res = FlightStorage.FindFlight(id);

                if (res == null)
                {
                    return NotFound();
                }

                return Ok(res);
            }
        }
    }
}

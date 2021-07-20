using System.Web.Http;
using FlightPlanner.Models;

namespace FlightPlanner.Controllers
{
    public class CostumerApiController : ApiController
    {
        [Route("api/airports"), HttpGet]
        public IHttpActionResult SearchAirports(string search)
        {
            FlightStorage.SearchAirports(search);
            var found = FlightStorage.foundAirpots;

            return Ok(found);
        }
        
        [Route("api/flights/search"), HttpPost]
        public IHttpActionResult SearchFlights(SearchFlightsRequest searchFlightsRequest)
        {
            if (FlightStorage.CheckIfNotNullAndIfCorrectFromTo(searchFlightsRequest))
            {
                return BadRequest();
            }
            
            var res = FlightStorage.SearchFlights(searchFlightsRequest);
            var found = FlightStorage.foundByReq;

            var pageResult = new PageResult(found);

            return Ok(pageResult);
        }

        [Route("api/flights/{id}"), HttpGet]
        public IHttpActionResult FindFlightById(int id)
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

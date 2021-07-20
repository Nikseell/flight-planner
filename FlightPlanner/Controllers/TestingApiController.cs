using System.Web.Http;
using FlightPlanner.Models;

namespace FlightPlanner.Controllers
{
    public class TestingApiController : ApiController
    {
        [Route("testing-api/clear"), HttpPost]
        public IHttpActionResult Clear()
        {
            FlightStorage.foundAirpots.Clear();
            FlightStorage.checkNames.Clear();
            FlightStorage.foundAirpots.Clear();
            FlightStorage.foundByReq.Clear();
            FlightStorage.allFlights.Clear();

            return Ok();
        }
    }
}

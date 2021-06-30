using System;
using System.Collections.Generic;

namespace FlightPlanner.Models
{
    public class PageResult
    {
        public int Page;
        public int TotalItems;
        public List<Flight> Items = new List<Flight>();

        public PageResult(List<Flight> flights)
        {
            Page = (int)Math.Ceiling((double)flights.Count / 10);
            TotalItems = flights.Count;
            Items = flights;
        }
        
    }
}
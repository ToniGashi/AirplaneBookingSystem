using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirplaneBookingSystem.Models
{
    public class UserFlights
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public int FlightId { get; set; }
        
        public Flight Flight { get; set; }
    }
}

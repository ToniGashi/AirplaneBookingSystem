using Microsoft.AspNetCore.Identity;
using System;


namespace AirplaneBookingSystem.Models
{
    public class OverbookedUser
    {
        public Flight Flight { get; set; }
        public int FlightId { get; set; }
        public String Email { get; set; }

    }
}

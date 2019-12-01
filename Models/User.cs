using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace AirplaneBookingSystem.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsAdmin { get; set; }
        public IList<UserFlights> UserFlights { get; set; }

        public IList<Feedback> Feedback { get; set; }

        public IList<OverbookedUser> OverbookedUsers { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace AirplaneBookingSystem.Models
{
    public class Flight
    {
        public int FlightId { get; set; }

        [DisplayName("Flight Number")]
        [Required]
        public string FlightNumber { get; set; }

        [Required]
        public string Departure { get; set; }

        [Required]
        public string Arrival { get; set; }

        [DisplayName("Departure Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime DepartureTime { get; set; }

        [DisplayName("Arrival Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime ArrivalTime { get; set; }


        public int FreeSeats { get; set; }

        [DisplayName("Number of Seats")]
        [Required]
        public int TotalSeats { get; set; }

        public bool IsCanceled { get; set; }

        public IList<UserFlights> UserFlights { get; set; }

        public IList<OverbookedUser> OverbookedUsers { get; set; }
    }
}

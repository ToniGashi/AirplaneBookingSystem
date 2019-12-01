using System.ComponentModel.DataAnnotations;


namespace AirplaneBookingSystem.ViewModel
{
    public class FlightListViewModel
    {
        public string Departure { get; set; }
        public string Arrival { get; set; }

        public string FlightNumber { get; set; }

        [DataType(DataType.Date)]
        public string DepartureTime { get; set; }

        [DataType(DataType.Date)]
        public string ArrivalTime { get; set; }
    }
}

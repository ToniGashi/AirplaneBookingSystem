using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.Data;
using AirplaneBookingSystem.Models;
using System.Security.Claims;
using System.Linq;
using System;
using System.Collections.Generic;

namespace AirplaneBookingSystem.Controllers
{
    public class FlightController : Controller
    {
        private readonly Db_Context ctx;

        public FlightController(Db_Context dbContext) {
            this.ctx = dbContext;         
        }

        [HttpPost]
        public async Task<IActionResult> MoveToNextFlight(int id)
        {
            var currentFlight = await ctx.Flights.FindAsync(id);
            Flight nextFlight = null;

            // Check if there is a flight with the same dep and dest + a later departure date
            foreach (var flight in ctx.Flights) {
                if (flight.Departure == currentFlight.Departure && flight.Arrival == currentFlight.Arrival && flight.DepartureTime > currentFlight.DepartureTime)
                {
                    nextFlight = flight;
                    break;
                }
            }

            if (nextFlight != null) {
                // check if the next flight is already overbooked
                if (nextFlight.FreeSeats < 0 || nextFlight.FreeSeats < ctx.GetOverbookedUsersFromFlight(currentFlight).Count)
                    return View("/Views/Errors/NextFlightOverbookedError.cshtml");
                else
                { // if not, move the overbooked users there

                    List<string> overbookedEmails = new List<string>();

                    // list of overbooked users from this flight
                    var overbookedUsers = ctx.GetOverbookedUsersFromFlight(currentFlight); 

                    // Removes all overbooked users and replaces them to the next available flight
                    for (int i = 0; i < overbookedUsers.Count; i++)
                    {
                        overbookedEmails.Add(overbookedUsers.ElementAt(i).Email);
                        ctx.UserFlights.Add(new UserFlights { User = ctx.GetUserFromEmail(overbookedUsers.ElementAt(i).Email), Flight = nextFlight });
                        --nextFlight.FreeSeats;


                        ctx.UserFlights.Remove(ctx.GetSpecificUserFlight(currentFlight, ctx.GetUserFromEmail(overbookedUsers.ElementAt(i).Email)));
                        ctx.OverbookedUsers.Remove(overbookedUsers.ElementAt(i));
                       
                        ++currentFlight.FreeSeats;
                        await ctx.SaveChangesAsync();
                    }

                    return RedirectToAction("SendEmail", "Email", new { emails = overbookedEmails });
                }
            }
            else
            {
                return View("/Views/Errors/NoNextFlightError.cshtml");
            }

          
        }
  
        [HttpGet]
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) {
                return View("Views/Errors/MustBeLoggedIn.cshtml");
            }else if (IsAdmin()) {
                return View();
            } else
            {
                return View("Views/Errors/MustBeAdmin.cshtml");
            }
            
        }

        // Creates a flight (you need to be admin)
        [HttpPost]
       public async Task<IActionResult> Create([Bind("FlightId, FlightNumber, Departure, Arrival, DepartureTime, ArrivalTime, TotalSeats")] Flight flight)
        {
            
                if (ModelState.IsValid && IsTimeValid(flight.DepartureTime, flight.ArrivalTime))
                {               // Check validation 

                
                ctx.Flights.Add(flight);            // Add the flight
                flight.FreeSeats = flight.TotalSeats;

                await ctx.SaveChangesAsync();       // Save changes
                    return RedirectToAction("index", "flight");
                }

                return View(flight);
            

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return View("Views/Errors/UserNotFound.cshtml");
            else if (IsAdmin())
                ViewData["isAdmin"] = true;
            else
                ViewData["isAdmin"] = false;

            return View(await ctx.Flights.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int flightId) {

                var currentFlight = await ctx.Flights.FindAsync(flightId);
                List<OverbookedUser> oUsers = ctx.GetOverbookedUsersFromFlight(currentFlight);
                 ctx.Flights.Remove(currentFlight);

                if (oUsers.Count() > 0) {
                    foreach (var oUser in oUsers) {
                    ctx.OverbookedUsers.Remove(oUser);
                    }
                }    
                await ctx.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
                    
        }

      
        [HttpGet]
        public async Task<IActionResult> Edit(int? id) {
            if (!User.Identity.IsAuthenticated) {
                return View("Views/Errors/MustBeLoggedIn.cshtml");
            } else if (IsAdmin()) {

                if (id == null)
                    return NotFound();

                var currentFlight = await ctx.Flights.FindAsync(id);

                if (currentFlight == null)
                    return NotFound();

                return View(currentFlight);

            } else
            {
                return View("Views/Errors/MustBeAdmin.cshtml");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlightId, FlightNumber, Departure, Arrival, DepartureTime, ArrivalTime, TotalSeats")] Flight currentFlight) {

            if (id != currentFlight.FlightId)
                return NotFound();

            

            if (ModelState.IsValid && IsTimeValid(currentFlight.DepartureTime, currentFlight.ArrivalTime)) {
                try {
                    ctx.Update(currentFlight);
                    await ctx.SaveChangesAsync();

                } catch (DbUpdateConcurrencyException) {
                    if (FlightExists(id))
                        throw;
                    else
                        return NotFound();
                }

                if (IsAdmin())
                    ViewData["isAdmin"] = true;
                else
                    ViewData["isAdmin"] = false;

                return RedirectToAction(nameof(Index));
            }

            return View(currentFlight);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int? id) {

            if (id == null)
                return NotFound();

            
            // get the current flight and current user
            var currentUser = await ctx.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var currentFlight = await ctx.Flights.FindAsync(id);
          
            // Create the Booking
            var userFlight = new UserFlights {             
                User = currentUser,         
                Flight = currentFlight
            };

            if (userFlight != null)
            {   
                
                ctx.UserFlights.Add(userFlight);  // add the user flight to the db
                --currentFlight.FreeSeats;        // assign the seat as reserved
                

                // Set the user as overbooked
                if (currentFlight.FreeSeats < 0) {
                    var overbookedUser = new OverbookedUser { Flight = currentFlight, Email = currentUser.Email };
               
                    ctx.OverbookedUsers.Add(overbookedUser);
                }

              
                await ctx.SaveChangesAsync();     // save changes to db
            }
                

            return View(userFlight);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id) {

            if (id == null) {
                return NotFound();
            }

            // get current flight
            var flight = await ctx.Flights.FindAsync(id);
            ViewData["isBooked"] = false;
            if (IsAdmin())
            {
                ViewData["isAdmin"] = true;
                List<String> overbookedUserEmails = new List<String>();
                foreach (var overbooked in ctx.OverbookedUsers) {
                    if (overbooked.Flight == flight)
                        overbookedUserEmails.Add(overbooked.Email);
                        
                }
                ViewData["OverBookedUsers"] = overbookedUserEmails;
            }
            else
            {
                ViewData["isAdmin"] = false;
            }

            // check if this flight is booked by this user
            foreach (var usrFlight in ctx.UserFlights) {
                if (usrFlight.FlightId == flight.FlightId && usrFlight.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    ViewData["isBooked"] = true;
                    break;
                }
                
            }

            if (flight == null) {
                return NotFound();
            }

            return View(flight);
        }

        private bool IsTimeValid(DateTime departure, DateTime arrival)
        {
            return departure <= arrival && departure > DateTime.Now;
        }

        private bool IsAdmin() {

            var currentUser =  ctx.Users.Find(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (currentUser.IsAdmin)
                return true;

            return false;
        }

        private bool FlightExists(int id) {
            return ctx.Flights.Any(e => e.FlightId == id);
        }

        public async Task<IActionResult> CancelBooking(int? id)
        {

            if (id == null)
                return NotFound();


            // get the current flight and current user
            var currentUser = await ctx.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var currentFlight = await ctx.Flights.FindAsync(id);

            // Create a User Flight so we know who had this flight for the View

            var userFlight = ctx.GetSpecificUserFlight(currentFlight, currentUser);

            if (userFlight != null)
            {
                // Remove overbooked user
                foreach(var overUser in ctx.OverbookedUsers)
                {
                    if(overUser.Email==currentUser.Email)
                    {
                        ctx.OverbookedUsers.Remove(ctx.GetOverbookedUserFromEmailAndFlight(currentUser.Email, currentFlight));
                    }
                }

                ctx.UserFlights.Remove(userFlight);  // removes the user flight from the db
                ++currentFlight.FreeSeats;        // assigns the seat as free
                await ctx.SaveChangesAsync();     // save changes to db
            }

            


            return View(userFlight);
        }

    }

}
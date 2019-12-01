using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AirplaneBookingSystem.Models;
using System.Collections.Generic;

namespace AirplaneBookingSystem.Data
{
    public class Db_Context : IdentityDbContext
    {
        public Db_Context(DbContextOptions<Db_Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserFlights>().HasKey(sc => new { sc.UserId, sc.FlightId });
            modelBuilder.Entity<OverbookedUser>().HasKey(sc => new { sc.FlightId, sc.Email});

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<OverbookedUser> OverbookedUsers { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<UserFlights> UserFlights { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }

        public UserFlights GetSpecificUserFlight(Flight flight, User user) {
            foreach (var userFlight in this.UserFlights) {
                if (flight.FlightId == userFlight.FlightId && user.Id == userFlight.UserId) {
                    return userFlight;
                }
            }

            return null;
        }
        public User GetUserFromEmail(string em) {
            foreach (var user in this.Users) {
                if (user.Email == em)
                    return user;
            }
            return null;
        }

        public OverbookedUser GetOverbookedUserFromEmailAndFlight(string Email, Flight flight) {
            foreach (var oUser in OverbookedUsers) {
                if (Email.Equals(oUser.Email) && flight.Equals(oUser.Flight)) {
                    return oUser;
                }
            }

            return null;
        }

        public List<OverbookedUser> GetOverbookedUsersFromFlight(Flight flight) {
            List<OverbookedUser> list = new List<OverbookedUser>();
            foreach (var oU in this.OverbookedUsers) {
                if (oU.Flight == flight)
                    list.Add(oU);
            }

            return list;
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.Models;
using System.Security.Claims;
using AirplaneBookingSystem.Data;
using Microsoft.EntityFrameworkCore;


namespace AirplaneBookingSystem.Controllers
{
    public class FeedbackController : Controller
    {

        private readonly Db_Context ctx;

        public FeedbackController(Db_Context c) {
            ctx = c;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            if (!User.Identity.IsAuthenticated)
                return View("Views/Errors/MustBeLoggedIn.cshtml");
            else if (IsAdmin())
                return View(await ctx.Feedback.ToListAsync());
            else
                return View("Views/Errors/MustBeAdmin.cshtml");
        }

        [HttpGet]
        public IActionResult Create()
        {

            if (!User.Identity.IsAuthenticated)
                return View("Views/Errors/UserNotFound.cshtml");
            else if (IsAdmin())
                return View("Views/Errors/FeedbackAdminError.cshtml");


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("FeedbackId", "Email", "Title", "Body")] Feedback feedback)
        {
            if (ModelState.IsValid) {
                feedback.Email = User.FindFirstValue(ClaimTypes.Name);
                ctx.Feedback.Add(feedback);
                await ctx.SaveChangesAsync();
                return RedirectToAction("index", "flight");
            }

            return View(feedback);
        }

        private bool IsAdmin()
        {

            var currentUser = ctx.Users.Find(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (currentUser.IsAdmin)
                return true;

            return false;
        }
    }
}
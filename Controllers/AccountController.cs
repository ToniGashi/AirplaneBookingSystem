using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.ViewModel;
using AirplaneBookingSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace AirplaneBookingSystem.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {

            if (ModelState.IsValid) {
                var user = new User { FirstName = model.FirstName, LastName = model.LastName, Email =model.Email, UserName = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded) {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "flight");
                }

                AccountError(ref result);
            }

            return View();
        }

        public void AccountError(ref IdentityResult result) {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public void AccountError(ref Microsoft.AspNetCore.Identity.SignInResult result) {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model){

            if (ModelState.IsValid) {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded) {
                    return RedirectToAction("index", "flight");
                }

                AccountError(ref result);
    
            }

            return View();
        }
    }
}
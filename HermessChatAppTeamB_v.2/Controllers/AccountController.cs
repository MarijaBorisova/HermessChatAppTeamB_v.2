using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HermessChatAppTeamB_v._2.ViewModels;
using HermessChatAppTeamB_v._2.Models;
using Microsoft.AspNetCore.Identity;

namespace HermessChatAppTeamB_v._2.Controllers
{
    //to work with user records
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        //to get Identity services through constructor
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email };
                // adding to user
                var result = await _userManager.CreateAsync(user, model.Password);// UserManager- service for user control
                if (result.Succeeded)
                {
                    // set up cookies and add to db
                    await _signInManager.SignInAsync(user, false);// SignInManager- service for user authentication and to set up cookies
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);//if adding of user passed incorrectly, ModelState adds to Model the mistakes and that returns to view
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        // to get return for address in the parameter returnUrl and to give to LoginViewModel.
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);//get the login and password and the time cookies saving
                if (result.Succeeded)
                {
                    // check if to URL to be
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login and/or password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // to delete authentication cookies
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HermessChatAppTeamB_v._2.ViewModels;
using HermessChatAppTeamB_v._2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

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
        /* public async Task<IActionResult> Register(RegisterViewModel model)
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

                     // token generation for user
                     var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                     var callbackUrl = Url.Action(
                         "ConfirmEmail",
                         "Account",
                         new { userId = user.Id, code = code },
                         protocol: HttpContext.Request.Scheme);
                     EmailService emailService = new EmailService();
                     await emailService.SendEmailAsync(model.Email, "Confirm your account",
                         $"Please, confirm your registration, call back the link sent on your email: <a href='{callbackUrl}'>link</a>");

                     return Content("To continue the registration, please, check you email and go to the link in the email");
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

         }*/
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email };
                // adding the user
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // token generate for user
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(model.Email, "Confirm your account",
                        $"Please, confirm the email, go to the link: <a href='{callbackUrl}'>link</a>");

                    return Content("To finish the registration, please, check your email and follow the link in the email");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }

        [HttpGet]
        // to get return for address in the parameter returnUrl and to give to LoginViewModel.
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        /*public async Task<IActionResult> Login(LoginViewModel model)
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
        }*/
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    // to check if email is confirmed or not
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "You have not confirmed your email address");
                        return View(model);
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "incorrect login and/or password");
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // if the user with this email is not in DB
                    // the standard message to hide the user in bd
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Reset Password",
                $"In order to reset the password, go to link: <a href='{callbackUrl}'>link</a>");
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
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
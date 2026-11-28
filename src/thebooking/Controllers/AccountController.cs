using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using thebooking.ViewModels;

namespace thebooking.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // New users get the role "User" (students)
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("New user {UserId} registered.", user.Id);
                    return RedirectToAction("Index", "Home");
                }

                // Only the error codes are logged, never the email address or the password
                _logger.LogWarning("Registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Code)));
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred during registration.");
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again later.");
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        var model = new LoginViewModel { ReturnUrl = returnUrl };
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Only local URLs are allowed, to prevent open redirect attacks (as in Demo-ShopSecurity)
        string targetUrl = !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
            ? model.ReturnUrl
            : Url.Action("Index", "Home") ?? "~/";

        if (ModelState.IsValid)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("A user logged in.");
                    return Redirect(targetUrl);
                }

                // The email address is not logged, to protect the user's privacy
                _logger.LogWarning("Failed login attempt.");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred during login.");
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again later.");
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("A user logged out.");
        return RedirectToAction("Index", "Home");
    }
}
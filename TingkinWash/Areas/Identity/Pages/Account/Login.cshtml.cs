
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TingkinWash.Models;

namespace TingkinWash.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public class LoginInput
        {
            [Required(ErrorMessage = "Please enter your email address.")]
            [EmailAddress(ErrorMessage = "Enter a valid email address.")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Please enter your password.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                ModelState.AddModelError("",
                    "The email address or password is incorrect.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // ADMIN DASHBOARD
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction(
                        "Index",
                        "AdminDashboard"
                    );
                }

                // CUSTOMER DASHBOARD
                if (roles.Contains("Customer"))
                {
                    return RedirectToAction(
                        "Index",
                        "Dashboard"
                    );
                }

                await _signInManager.SignOutAsync();

                ModelState.AddModelError("",
                    "Your account has no assigned role.");

                return Page();
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("",
                    "Your account is temporarily locked. Please try again later.");
                return Page();
            }

            ModelState.AddModelError("",
                "The email address or password is incorrect.");

            return Page();
        }
    }
}

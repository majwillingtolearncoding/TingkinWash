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

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
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

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Dashboard");

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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TingkinWash.Models;

namespace TingkinWash.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public class RegisterInput
        {
            [Required(ErrorMessage = "First name is required.")]
            [StringLength(50)]
            public string FirstName { get; set; } = "";

            [Required(ErrorMessage = "Last name is required.")]
            [StringLength(50)]
            public string LastName { get; set; } = "";

            [Required(ErrorMessage = "Email address is required.")]
            [EmailAddress(ErrorMessage = "Enter a valid email address.")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Phone number is required.")]
            [Phone(ErrorMessage = "Enter a valid phone number.")]
            public string PhoneNumber { get; set; } = "";

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, MinimumLength = 8,
                ErrorMessage = "Password must contain at least 8 characters.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Required(ErrorMessage = "Please confirm your password.")]
            [DataType(DataType.Password)]
            [Compare("Password",
                ErrorMessage = "Your passwords do not match.")]
            public string ConfirmPassword { get; set; } = "";

            [Range(typeof(bool), "true", "true",
                ErrorMessage = "You must accept the Terms and Conditions.")]
            public bool AcceptTerms { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                FirstName = Input.FirstName.Trim(),
                LastName = Input.LastName.Trim(),
                UserName = Input.Email.Trim(),
                Email = Input.Email.Trim(),
                PhoneNumber = Input.PhoneNumber.Trim()
            };

            var result = await _userManager.CreateAsync(
                user,
                Input.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false);

                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return Page();
        }
    }
}
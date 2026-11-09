using Microsoft.AspNetCore.Identity;

namespace TingkinWash.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName
        {
            get => $"{FirstName} {LastName}".Trim();
            set
            {
                var parts = value.Split(
                    ' ',
                    2,
                    StringSplitOptions.RemoveEmptyEntries);

                FirstName = parts.Length > 0 ? parts[0] : "";
                LastName = parts.Length > 1 ? parts[1] : "";
            }
        }
    }
}
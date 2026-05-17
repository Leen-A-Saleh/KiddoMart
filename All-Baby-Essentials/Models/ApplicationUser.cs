using Microsoft.AspNetCore.Identity;

namespace All_Baby_Essentials.Models
{
    public enum ApplicationUserGender { F,M}
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth {  get; set; }
        public ApplicationUserGender Gender { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}

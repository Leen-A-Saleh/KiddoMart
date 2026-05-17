using All_Baby_Essentials.Models;

namespace All_Baby_Essentials.Areas.Admin.ViewModels
{
    public class ApplicationUserVM
    {
        public string Id { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public ApplicationUserGender Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}

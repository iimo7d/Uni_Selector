using Uni_Selector.Models;

namespace Uni_Selector.ViewModels.UserManagement
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; }
        public Models.UniversityRepresentative UniversityRepresentative { get; set; }
    }
}

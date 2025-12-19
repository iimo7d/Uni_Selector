using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.UserManagement
{
    public class CreateUserViewModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; }

        // For UniversityRep only
        public int? UniversityId { get; set; }
        public string Position { get; set; }
        public bool CanManagePrograms { get; set; } = true;
        public bool CanManageFees { get; set; } = true;
        public bool CanViewApplications { get; set; } = true;

        // These should NOT be validated - they're just for dropdown population
        [ValidateNever]
        public SelectList AvailableRoles { get; set; }

        [ValidateNever]
        public SelectList AvailableUniversities { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.UserManagement
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        // NewPassword is OPTIONAL - no [Required] attribute
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Account Status")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string Role { get; set; }

        // For UniversityRep only - NOT required at ViewModel level
        // Validation will be done in controller
        [Display(Name = "University")]
        public int? UniversityId { get; set; }

        // Position is NOT required - only needed for UniversityRep
        [StringLength(100)]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [Display(Name = "Can Manage Programs")]
        public bool CanManagePrograms { get; set; }

        [Display(Name = "Can Manage Fees")]
        public bool CanManageFees { get; set; }

        [Display(Name = "Can View Applications")]
        public bool CanViewApplications { get; set; }

        // Dropdown lists - should NOT be validated
        [ValidateNever]
        public SelectList AvailableRoles { get; set; }

        [ValidateNever]
        public SelectList AvailableUniversities { get; set; }
    }
}

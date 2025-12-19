using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.Student
{
    public class UploadBtecCertificateViewModel
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "الرجاء اختيار ملف الشهادة")]
        [Display(Name = "شهادة BTEC")]
        public IFormFile CertificateFile { get; set; } = null!;

        public string? CurrentCertificateUrl { get; set; }
    }
}

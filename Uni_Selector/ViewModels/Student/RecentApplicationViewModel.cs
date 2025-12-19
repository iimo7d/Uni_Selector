namespace Uni_Selector.ViewModels.Student
{
    public class RecentApplicationViewModel
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusBadgeClass { get; set; } = string.Empty;
        public DateTime ApplicationDate { get; set; }
    }
}

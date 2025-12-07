using Uni_Selector.Models;

namespace Uni_Selector.ViewModels.Student
{
    public class StudentDashboardViewModel
    {
        public Models.Student Student { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int TotalRecommendations { get; set; }
        public int ActiveDiscounts { get; set; }
        public List<StudentApplication> RecentApplications { get; set; } = new();

        // Location-based features
        public int NearbyUniversitiesCount { get; set; }
        public string? GoogleMapsApiKey { get; set; }
    }
}

namespace Uni_Selector.ViewModels.Student
{
    public class StudentDashboardViewModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public double GPA { get; set; }
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PathDisplay { get; set; } = string.Empty;
        public string? TrackDisplay { get; set; }
        public decimal RegistrationBudget { get; set; }
        public DateTime MemberSince { get; set; }
        public bool IsProfileComplete { get; set; }

        // Statistics
        public int TotalRecommendations { get; set; }
        public int ViewedRecommendations { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int ActiveDiscounts { get; set; }

        // Recent Activity
        public List<RecentRecommendationViewModel> RecentRecommendations { get; set; } = new();
        public List<RecentApplicationViewModel> RecentApplications { get; set; } = new();
        public List<NotificationViewModel> RecentNotifications { get; set; } = new();
    }
}

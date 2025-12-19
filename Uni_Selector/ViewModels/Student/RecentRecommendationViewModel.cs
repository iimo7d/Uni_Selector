namespace Uni_Selector.ViewModels.Student
{
    public class RecentRecommendationViewModel
    {
        public int Id { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;
        public double Score { get; set; }
        public decimal EstimatedCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsViewed { get; set; }
    }
}

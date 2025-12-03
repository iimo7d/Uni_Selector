namespace Uni_Selector.ViewModels.UniversityAdmin
{
    public class UniversityRepresentativesViewModel
    {
        public int UniversityId { get; set; }
        public string UniversityName { get; set; } = string.Empty;
        public List<RepresentativeItemViewModel> Representatives { get; set; } = new();
        public List<AvailableUserViewModel> AvailableUsers { get; set; } = new();
    }
}

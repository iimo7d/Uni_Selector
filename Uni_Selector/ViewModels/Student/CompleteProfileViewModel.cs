namespace Uni_Selector.ViewModels.Student
{
    public class CompleteProfileViewModel
    {
        public int CurrentStep { get; set; } = 1;
        public int TotalSteps { get; set; } = 4;
        public int StudentId { get; set; }

        // Step 1: Personal Information
        public PersonalInfoStepViewModel PersonalInfo { get; set; } = new();

        // Step 2: Academic Information
        public AcademicInfoStepViewModel AcademicInfo { get; set; } = new();

        // Step 3: Preferences
        public PreferencesStepViewModel Preferences { get; set; } = new();

        // Step 4: Guardian & Special Circumstances
        public GuardianStepViewModel Guardian { get; set; } = new();
    }
}

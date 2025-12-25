using Microsoft.EntityFrameworkCore;
using Uni_Selector.Data;
using Uni_Selector.Models;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.Service
{
    public class RecommendationService
    {
        private readonly AppDbContext _context;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recommendation>> GenerateRecommendationsAsync(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null || student.ProfileCompleted != true)
                return new List<Recommendation>();

            // Delete existing recommendations for this student
            var existingRecs = await _context.Recommendations
                .Where(r => r.StudentId == studentId)
                .ToListAsync();
            _context.Recommendations.RemoveRange(existingRecs);

            var recommendations = new List<Recommendation>();

            // Generate recommendations based on student path
            if (student.Path == PathType.BTEC)
            {
                recommendations.AddRange(await GenerateBtecRecommendationsAsync(student));
            }
            else
            {
                recommendations.AddRange(await GenerateRegularProgramRecommendationsAsync(student));
            }

            // Save recommendations to database
            _context.Recommendations.AddRange(recommendations);
            await _context.SaveChangesAsync();

            return recommendations;
        }

        private async Task<List<Recommendation>> GenerateRegularProgramRecommendationsAsync(Student student)
        {
            var recommendations = new List<Recommendation>();

            // Get all active university programs with their requirements
            var programs = await _context.UniversityPrograms
                .Include(up => up.University)
                .Include(up => up.Program)
                .Include(up => up.EntryRequirements)
                .Where(up => up.IsActive && up.University.IsActive)
                .ToListAsync();

            foreach (var program in programs)
            {
                // Check if student meets entry requirements
                var meetsRequirements = CheckEntryRequirements(student, program);
                if (!meetsRequirements)
                    continue;

                // Calculate recommendation score
                double score = CalculateProgramScore(student, program);

                // Calculate distance (simplified - using city match)
                double? distanceInKm = CalculateDistance(student, program.University);

                // Calculate estimated cost
                decimal estimatedCost = CalculateEstimatedCost(program, student);

                // Generate recommendation reasons
                var reasons = GenerateRecommendationReasons(student, program, score);

                recommendations.Add(new Recommendation
                {
                    StudentId = student.Id,
                    UniversityProgramId = program.Id,
                    Score = score,
                    ReasonsJson = System.Text.Json.JsonSerializer.Serialize(reasons),
                    DistanceInKm = distanceInKm,
                    EstimatedTotalCost = estimatedCost,
                    CreatedAt = DateTime.UtcNow,
                    IsViewed = false
                });
            }

            // Return top recommendations sorted by score
            return recommendations.OrderByDescending(r => r.Score).ToList();
        }

        private async Task<List<Recommendation>> GenerateBtecRecommendationsAsync(Student student)
        {
            var recommendations = new List<Recommendation>();

            // Get all active BTEC programs with their requirements
            var programs = await _context.BtecPrograms
                .Include(bp => bp.University)
                .Include(bp => bp.EntryRequirements)
                .Where(bp => bp.IsActive && bp.University.IsActive && bp.IsApprovedByBtecAuthority)
                .ToListAsync();

            foreach (var program in programs)
            {
                // Check if student meets BTEC entry requirements
                var meetsRequirements = CheckBtecEntryRequirements(student, program);
                if (!meetsRequirements)
                    continue;

                // Calculate recommendation score
                double score = CalculateBtecProgramScore(student, program);

                // Calculate distance
                double? distanceInKm = CalculateDistance(student, program.University);

                // Calculate estimated cost
                decimal estimatedCost = CalculateBtecEstimatedCost(program, student);

                // Generate recommendation reasons
                var reasons = GenerateBtecRecommendationReasons(student, program, score);

                recommendations.Add(new Recommendation
                {
                    StudentId = student.Id,
                    BtecProgramId = program.Id,
                    Score = score,
                    ReasonsJson = System.Text.Json.JsonSerializer.Serialize(reasons),
                    DistanceInKm = distanceInKm,
                    EstimatedTotalCost = estimatedCost,
                    CreatedAt = DateTime.UtcNow,
                    IsViewed = false
                });
            }

            return recommendations.OrderByDescending(r => r.Score).ToList();
        }

        #region Entry Requirements Checking

        private bool CheckEntryRequirements(Student student, UniversityProgram program)
        {
            var requirements = program.EntryRequirements;
            if (requirements == null || !requirements.Any())
                return true; // No specific requirements

            foreach (var req in requirements)
            {
                // Check GPA
                if (student.GPA < req.MinGPA)
                    continue;

                // Check Path
                if (req.Path != student.Path)
                    continue;

                // Check Academic Track (if Academic path)
                if (req.Path == PathType.Academic)
                {
                    if (req.AcademicTrack.HasValue && req.AcademicTrack != student.AcademicTrack)
                        continue;
                }

                // Check Vocational Branch (if Vocational path)
                if (req.Path == PathType.Vocational)
                {
                    if (req.VocationalBranch.HasValue && req.VocationalBranch != student.VocationalBranch)
                        continue;
                }

                // If we reach here, student meets at least one requirement
                return true;
            }

            return false;
        }

        private bool CheckBtecEntryRequirements(Student student, BtecProgram program)
        {
            var requirements = program.EntryRequirements;
            if (requirements == null || !requirements.Any())
                return true;

            foreach (var req in requirements)
            {
                // Check GPA
                if (student.GPA < req.MinGPA)
                    continue;

                // Check BTEC Level 2 requirement
                if (req.RequiresBtecL2 && !student.BtecLevel2Completed)
                    continue;

                // Check BTEC Level 3 requirement
                if (req.RequiresBtecL3 && !student.BtecLevel3Completed)
                    continue;

                // If we reach here, student meets at least one requirement
                return true;
            }

            return false;
        }

        #endregion

        #region Scoring Algorithms

        private double CalculateProgramScore(Student student, UniversityProgram program)
        {
            double totalScore = 0;

            // 1. Entry Requirements Match (40 points)
            var requirementScore = CalculateRequirementMatchScore(student, program);
            totalScore += requirementScore * 0.40;

            // 2. Budget Match (20 points)
            var budgetScore = CalculateBudgetScore(student, program);
            totalScore += budgetScore * 0.20;

            // 3. Location Match (15 points)
            var locationScore = CalculateLocationScore(student, program.University);
            totalScore += locationScore * 0.15;

            // 4. Major Match (15 points)
            var majorScore = CalculateMajorScore(student, program);
            totalScore += majorScore * 0.15;

            // 5. Language Preference (5 points)
            var languageScore = CalculateLanguageScore(student, program);
            totalScore += languageScore * 0.05;

            // 6. Special Circumstances Bonus (5 points)
            var bonusScore = CalculateSpecialCircumstancesBonus(student, program);
            totalScore += bonusScore * 0.05;

            return Math.Min(totalScore * 100, 100); // Normalize to 100
        }

        private double CalculateBtecProgramScore(Student student, BtecProgram program)
        {
            double totalScore = 0;

            // Similar scoring for BTEC programs
            totalScore += (student.BtecLevel2Completed ? 20 : 0);
            totalScore += (student.BtecLevel3Completed ? 20 : 0);

            // Budget match
            decimal firstSemesterCost = program.RegistrationFeeFirstSemester + (program.HourPriceBase * 15); // Assume 15 hours
            if (firstSemesterCost <= student.RegistrationBudget)
                totalScore += 20;
            else if (firstSemesterCost <= student.RegistrationBudget * 1.2m)
                totalScore += 10;

            // Location
            if (student.PreferredCity != null && program.University.City.Contains(student.PreferredCity, StringComparison.OrdinalIgnoreCase))
                totalScore += 15;

            // Language
            if (program.Language == student.PreferredLanguage || program.Language == LanguageCode.Both)
                totalScore += 10;

            // Special circumstances
            if (student.HasFamilyConnection && student.FamilyConnectionUniversityId == program.UniversityId)
                totalScore += 15;

            return totalScore;
        }

        private double CalculateRequirementMatchScore(Student student, UniversityProgram program)
        {
            // Perfect match if GPA significantly exceeds minimum
            var minGPA = program.EntryRequirements.Any() ? program.EntryRequirements.Min(r => r.MinGPA) : 0;

            if (student.GPA >= minGPA + 10)
                return 100;
            else if (student.GPA >= minGPA + 5)
                return 85;
            else if (student.GPA >= minGPA)
                return 70;

            return 50;
        }

        private double CalculateBudgetScore(Student student, UniversityProgram program)
        {
            // Calculate first semester cost
            decimal firstSemesterCost = program.RegistrationFeeFirstSemester + (program.HourPriceBase * 15); // Assume 15 hours

            if (firstSemesterCost <= student.RegistrationBudget)
                return 100;
            else if (firstSemesterCost <= student.RegistrationBudget * 1.1m)
                return 80;
            else if (firstSemesterCost <= student.RegistrationBudget * 1.2m)
                return 60;
            else if (firstSemesterCost <= student.RegistrationBudget * 1.3m)
                return 40;

            return 20;
        }

        private double CalculateLocationScore(Student student, University university)
        {
            // City match
            if (student.City.Equals(university.City, StringComparison.OrdinalIgnoreCase))
                return 100;

            // Preferred city match
            if (!string.IsNullOrEmpty(student.PreferredCity) &&
                university.City.Contains(student.PreferredCity, StringComparison.OrdinalIgnoreCase))
                return 90;

            // Same province
            if (student.Province.Equals(university.Province, StringComparison.OrdinalIgnoreCase))
                return 70;

            return 50;
        }

        private double CalculateMajorScore(Student student, UniversityProgram program)
        {
            if (string.IsNullOrEmpty(student.DesiredMajors))
                return 50;

            var desiredMajors = student.DesiredMajors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                      .Select(m => m.Trim().ToLower()).ToList();

            var programName = program.Program.NameEnglish.ToLower();

            foreach (var major in desiredMajors)
            {
                if (programName.Contains(major))
                    return 100;
            }

            return 40;
        }

        private double CalculateLanguageScore(Student student, UniversityProgram program)
        {
            if (program.Program.Language == student.PreferredLanguage)
                return 100;

            if (program.Program.Language == LanguageCode.Both)
                return 90;

            return 50;
        }

        private double CalculateSpecialCircumstancesBonus(Student student, UniversityProgram program)
        {
            double bonus = 0;

            // Family connection bonus
            if (student.HasFamilyConnection && student.FamilyConnectionUniversityId == program.UniversityId)
                bonus += 40;

            // Employee child bonus
            if (student.IsEmployeeChild)
                bonus += 20;

            // Orphan bonus
            if (student.IsOrphan)
                bonus += 20;

            // Disability bonus (universities with accessibility)
            if (student.HasDisability)
                bonus += 20;

            return Math.Min(bonus, 100);
        }

        #endregion

        #region Cost Calculation

        private decimal CalculateEstimatedCost(UniversityProgram program, Student student)
        {
            // Estimate first semester cost with 5% discount
            decimal registrationFee = program.RegistrationFeeFirstSemester;
            decimal creditHoursCost = program.HourPriceBase * 15; // Assume 15 hours first semester
            decimal totalBeforeDiscount = registrationFee + creditHoursCost;

            // Apply 5% platform discount
            decimal discount = totalBeforeDiscount * 0.05m;

            return totalBeforeDiscount - discount;
        }

        private decimal CalculateBtecEstimatedCost(BtecProgram program, Student student)
        {
            decimal registrationFee = program.RegistrationFeeFirstSemester;
            decimal creditHoursCost = program.HourPriceBase * 15; // Assume 15 hours
            decimal totalBeforeDiscount = registrationFee + creditHoursCost;

            decimal discount = totalBeforeDiscount * 0.05m;

            return totalBeforeDiscount - discount;
        }

        #endregion

        #region Distance Calculation

        private double? CalculateDistance(Student student, University university)
        {
            // Simplified distance calculation based on city match
            // In production, use actual GPS coordinates and Haversine formula

            if (student.City.Equals(university.City, StringComparison.OrdinalIgnoreCase))
                return 5.0; // Same city

            if (student.Province.Equals(university.Province, StringComparison.OrdinalIgnoreCase))
                return 25.0; // Same province

            return 75.0; // Different province
        }

        #endregion

        #region Reason Generation

        private List<string> GenerateRecommendationReasons(Student student, UniversityProgram program, double score)
        {
            var reasons = new List<string>();

            // GPA Match
            var minGPA = program.EntryRequirements.Any() ? program.EntryRequirements.Min(r => r.MinGPA) : 0;
            if (student.GPA >= minGPA + 10)
                reasons.Add("Your GPA significantly exceeds the minimum requirement");
            else if (student.GPA >= minGPA)
                reasons.Add("You meet the GPA requirement");

            // Budget Match
            decimal cost = CalculateEstimatedCost(program, student);
            if (cost <= student.RegistrationBudget)
                reasons.Add("Fits within your budget");

            // Location
            if (student.City.Equals(program.University.City, StringComparison.OrdinalIgnoreCase))
                reasons.Add("Located in your city");

            // Language
            if (program.Program.Language == student.PreferredLanguage)
                reasons.Add("Taught in your preferred language");

            // Special circumstances
            if (student.HasFamilyConnection && student.FamilyConnectionUniversityId == program.UniversityId)
                reasons.Add("You have family connection at this university");

            // Major match
            if (!string.IsNullOrEmpty(student.DesiredMajors))
            {
                var majors = student.DesiredMajors.Split(',').Select(m => m.Trim());
                foreach (var major in majors)
                {
                    if (program.Program.NameEnglish.Contains(major, StringComparison.OrdinalIgnoreCase))
                        reasons.Add($"Matches your desired major: {major}");
                }
            }

            if (score >= 80)
                reasons.Add("Highly recommended based on your profile");

            return reasons;
        }

        private List<string> GenerateBtecRecommendationReasons(Student student, BtecProgram program, double score)
        {
            var reasons = new List<string>();

            if (student.BtecLevel2Completed)
                reasons.Add("You have completed BTEC Level 2");

            if (student.BtecLevel3Completed)
                reasons.Add("You have completed BTEC Level 3");

            if (program.Language == student.PreferredLanguage)
                reasons.Add("Taught in your preferred language");

            decimal cost = CalculateBtecEstimatedCost(program, student);
            if (cost <= student.RegistrationBudget)
                reasons.Add("Fits within your budget");

            if (student.HasFamilyConnection && student.FamilyConnectionUniversityId == program.UniversityId)
                reasons.Add("You have family connection at this university");

            return reasons;
        }

        #endregion
    }
}


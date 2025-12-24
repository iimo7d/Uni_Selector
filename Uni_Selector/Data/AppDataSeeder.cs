using Microsoft.EntityFrameworkCore;
using Uni_Selector.Models;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.Data
{
    public class AppDataSeeder
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AppDataSeeder> _logger;

        public AppDataSeeder(AppDbContext db, ILogger<AppDataSeeder> logger)
        {
            _db = db;
            _logger = logger;
        }

        private static readonly DateTime SeedNow =
            new DateTime(2025, 12, 01, 0, 0, 0, DateTimeKind.Utc);

        private static readonly DateTime SemesterStart =
            new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc);

        public async Task SeedAsync(CancellationToken ct = default)
        {
            await SeedUniversitiesAsync(ct);
            await SeedProgramsAsync(ct);
            await SeedUniversityProgramsAndEntryReqAsync(ct);
            await SeedBtecProgramsAndReqAsync(ct);
            await SeedUniversityRepresentativesAsync(ct);
            await SeedStudentsAsync(ct);
            await SeedApplicationsCommissionDiscountAsync(ct);
        }

        private async Task SeedUniversitiesAsync(CancellationToken ct)
        {
            if (await _db.Universities.AnyAsync(ct)) return;

            _db.Universities.AddRange(
                new University
                {
                    Id = 1,
                    NameArabic = "الجامعة الألمانية الأردنية",
                    NameEnglish = "German Jordanian University",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Madaba Street, Amman 11180, Jordan",
                    Latitude = 31.9539,
                    Longitude = 35.8666,
                    PhoneNumber = "+962-6-4294444",
                    Email = "info@gju.edu.jo",
                    OfficialWebsite = "https://www.gju.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 2,
                    NameArabic = "جامعة الشرق الأوسط",
                    NameEnglish = "Middle East University",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Airport Road, Amman 11831, Jordan",
                    Latitude = 31.9215,
                    Longitude = 35.9411,
                    PhoneNumber = "+962-6-4790222",
                    Email = "info@meu.edu.jo",
                    OfficialWebsite = "https://www.meu.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 3,
                    NameArabic = "جامعة الإسراء",
                    NameEnglish = "Isra University",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "King Abdullah II Street, Amman 11622, Jordan",
                    Latitude = 31.9719,
                    Longitude = 35.8728,
                    PhoneNumber = "+962-6-4711710",
                    Email = "info@iu.edu.jo",
                    OfficialWebsite = "https://www.iu.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 4,
                    NameArabic = "جامعة الزرقاء",
                    NameEnglish = "Zarqa University",
                    Type = UniversityType.Private,
                    Province = "Zarqa",
                    City = "Zarqa",
                    FullAddress = "Zarqa 13110, Jordan",
                    Latitude = 32.0833,
                    Longitude = 36.0833,
                    PhoneNumber = "+962-5-3821100",
                    Email = "info@zu.edu.jo",
                    OfficialWebsite = "https://www.zu.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 5,
                    NameArabic = "جامعة البترا",
                    NameEnglish = "Petra University",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Airport Road, Amman 11196, Jordan",
                    Latitude = 31.9453,
                    Longitude = 35.9284,
                    PhoneNumber = "+962-6-5715546",
                    Email = "info@uop.edu.jo",
                    OfficialWebsite = "https://www.uop.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 6,
                    NameArabic = "جامعة عمان الأهلية",
                    NameEnglish = "Al-Ahliyya Amman University",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Al-Salt Highway, Amman 19328, Jordan",
                    Latitude = 32.0126,
                    Longitude = 35.8433,
                    PhoneNumber = "+962-6-5008000",
                    Email = "info@ammanu.edu.jo",
                    OfficialWebsite = "https://www.ammanu.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 7,
                    NameArabic = "كلية الجامعة للتكنولوجيا الحديثة",
                    NameEnglish = "New University College of Technology",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Amman, Jordan",
                    Latitude = 31.9454,
                    Longitude = 35.9284,
                    PhoneNumber = "+962-6-5000000",
                    Email = "info@nuct.edu.jo",
                    OfficialWebsite = "https://www.nuct.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 8,
                    NameArabic = "كلية الخوارزمي الجامعية",
                    NameEnglish = "Al-Khawarizmi College",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Airport Road, Amman, Jordan",
                    Latitude = 31.9454,
                    Longitude = 35.9284,
                    PhoneNumber = "+962-6-4899450",
                    Email = "info@khawarizmi.edu.jo",
                    OfficialWebsite = "https://www.khawarizmi.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 9,
                    NameArabic = "كلية المجتمع العربي",
                    NameEnglish = "Arab Community College",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Amman, Jordan",
                    Latitude = 31.9454,
                    Longitude = 35.9284,
                    PhoneNumber = "+962-6-5000000",
                    Email = "info@arabcollege.edu.jo",
                    OfficialWebsite = "https://www.arabcollege.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 10,
                    NameArabic = "جامعة العلوم التطبيقية الخاصة",
                    NameEnglish = "Applied Science Private University",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Al-Arab Street, Amman 11931, Jordan",
                    Latitude = 31.9909,
                    Longitude = 35.8709,
                    PhoneNumber = "+962-6-5609999",
                    Email = "info@asu.edu.jo",
                    OfficialWebsite = "https://www.asu.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                },
                new University
                {
                    Id = 11,
                    NameArabic = "جامعة الحسين التقنية",
                    NameEnglish = "Hussein Technical University",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Al-Hussein Bin Talal Street, Amman, Jordan",
                    Latitude = 31.9754,
                    Longitude = 35.8676,
                    PhoneNumber = "+962-6-5331000",
                    Email = "info@htu.edu.jo",
                    OfficialWebsite = "https://www.htu.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = SeedNow
                }
            );

            await _db.SaveChangesAsync(ct);
        }

        private async Task SeedProgramsAsync(CancellationToken ct)
        {
            if (await _db.Programs.AnyAsync(ct)) return;

            _db.Programs.AddRange(
                new ProgramEntity
                {
                    Id = 1,
                    NameArabic = "الذكاء الاصطناعي",
                    NameEnglish = "Artificial Intelligence",
                    Description = "Bachelor's degree in Artificial Intelligence and Machine Learning",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Computer Science",
                    TotalCreditHours = 132,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 2,
                    NameArabic = "التقنيات اللوجستية",
                    NameEnglish = "Logistics Technologies",
                    Description = "Bachelor's degree in Logistics and Supply Chain Management",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Business Administration",
                    TotalCreditHours = 132,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 3,
                    NameArabic = "التصميم الداخلي",
                    NameEnglish = "Interior Design",
                    Description = "Bachelor's degree in Interior Design and Architecture",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Arts and Design",
                    TotalCreditHours = 132,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 4,
                    NameArabic = "إدارة الأعمال الإلكترونية",
                    NameEnglish = "E-Business Management",
                    Description = "Diploma in Electronic Business Administration",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Business Administration",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 5,
                    NameArabic = "المحاسبة التقنية",
                    NameEnglish = "Technical Accounting",
                    Description = "Diploma in Technical Accounting",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Accounting",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 6,
                    NameArabic = "إدارة الفنادق",
                    NameEnglish = "Hotel Management",
                    Description = "Diploma in Hotel and Hospitality Management",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Tourism and Hospitality",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 7,
                    NameArabic = "إدارة السياحة والفنادق",
                    NameEnglish = "Tourism and Hotel Management",
                    Description = "Diploma in Tourism and Hotel Management",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Tourism and Hospitality",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 8,
                    NameArabic = "فنون الطهي",
                    NameEnglish = "Culinary Arts",
                    Description = "Bachelor in Culinary Arts and Food Management",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Hospitality",
                    TotalCreditHours = 132,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 9,
                    NameArabic = "تكنولوجيا إنشاء وصيانة المباني",
                    NameEnglish = "Building Construction and Maintenance Technology",
                    Description = "Diploma in Civil Engineering Technology",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Engineering",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 10,
                    NameArabic = "هندسة العمارة والتصميم الداخلي",
                    NameEnglish = "Architecture and Interior Design",
                    Description = "Diploma in Architecture and Interior Design",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Architecture",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 11,
                    NameArabic = "مساحة الطرق وحساب الكميات",
                    NameEnglish = "Road Surveying and Quantity Calculation",
                    Description = "Diploma in Road Surveying",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Engineering",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 12,
                    NameArabic = "تكنولوجيا الطاقة",
                    NameEnglish = "Energy Technology",
                    Description = "Diploma in Energy Technology",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Engineering",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 13,
                    NameArabic = "الاتصالات وشبكات الحاسوب",
                    NameEnglish = "Communications and Computer Networks",
                    Description = "Diploma in Computer Networks",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Information Technology",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 14,
                    NameArabic = "أمن المعلومات والشبكات",
                    NameEnglish = "Information Security and Networks",
                    Description = "Diploma in Cybersecurity",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Information Technology",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 15,
                    NameArabic = "الصيدلة",
                    NameEnglish = "Pharmacy",
                    Description = "Diploma in Pharmacy",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Medical",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 16,
                    NameArabic = "التمريض المشارك",
                    NameEnglish = "Associate Nursing",
                    Description = "Diploma in Nursing",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Medical",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 17,
                    NameArabic = "طب الأسنان",
                    NameEnglish = "Dentistry",
                    Description = "Bachelor in Dentistry",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Medical",
                    TotalCreditHours = 180,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 18,
                    NameArabic = "التصميم الجرافيكي",
                    NameEnglish = "Graphic Design",
                    Description = "Diploma in Graphic Design",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Arts",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 19,
                    NameArabic = "فنون التصميم الداخلي والديكور",
                    NameEnglish = "Interior Design and Decoration Arts",
                    Description = "Diploma in Interior Decoration",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Arts",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 20,
                    NameArabic = "فنون السينما والتلفزيون",
                    NameEnglish = "Cinema and Television Arts",
                    Description = "Diploma in Film and TV Production",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Arts",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 21,
                    NameArabic = "صناعة الأفلام",
                    NameEnglish = "Film Production",
                    Description = "Bachelor in Film Production",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Arts",
                    TotalCreditHours = 132,
                    CreatedAt = SeedNow
                },
                new ProgramEntity
                {
                    Id = 22,
                    NameArabic = "اللغة الإنجليزية التطبيقية",
                    NameEnglish = "Applied English Language",
                    Description = "Diploma in Applied English",
                    Degree = Degree.Diploma,
                    Language = LanguageCode.English,
                    AcademicClassification = "Languages",
                    TotalCreditHours = 72,
                    CreatedAt = SeedNow
                }
            );

            await _db.SaveChangesAsync(ct);
        }

        private async Task SeedUniversityProgramsAndEntryReqAsync(CancellationToken ct)
        {
            if (!await _db.UniversityPrograms.AnyAsync(ct))
            {
                var programs = new List<UniversityProgram>
                {
                    new UniversityProgram { Id = 1, UniversityId = 1, ProgramId = 2, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 110m, RegistrationFeeFirstSemester = 557m, RegistrationFeeRegularSemester = 230m, Capacity = 50, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 2, UniversityId = 1, ProgramId = 1, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 110m, RegistrationFeeFirstSemester = 557m, RegistrationFeeRegularSemester = 230m, Capacity = 50, IsActive = true, CreatedAt = SeedNow },

                    new UniversityProgram { Id = 3, UniversityId = 2, ProgramId = 1, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 65m, RegistrationFeeFirstSemester = 450m, RegistrationFeeRegularSemester = 450m, Capacity = 80, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 4, UniversityId = 2, ProgramId = 2, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 58.5m, RegistrationFeeFirstSemester = 450m, RegistrationFeeRegularSemester = 450m, Capacity = 60, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 5, UniversityId = 2, ProgramId = 3, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 65m, RegistrationFeeFirstSemester = 450m, RegistrationFeeRegularSemester = 450m, Capacity = 40, IsActive = true, CreatedAt = SeedNow },

                    new UniversityProgram { Id = 6, UniversityId = 3, ProgramId = 1, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 45m, RegistrationFeeFirstSemester = 350m, RegistrationFeeRegularSemester = 350m, Capacity = 70, IsActive = true, CreatedAt = SeedNow },

                    new UniversityProgram { Id = 7, UniversityId = 4, ProgramId = 1, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 50m, RegistrationFeeFirstSemester = 350m, RegistrationFeeRegularSemester = 350m, Capacity = 60, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 8, UniversityId = 4, ProgramId = 2, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 44m, RegistrationFeeFirstSemester = 350m, RegistrationFeeRegularSemester = 350m, Capacity = 50, IsActive = true, CreatedAt = SeedNow },

                    new UniversityProgram { Id = 9, UniversityId = 5, ProgramId = 2, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 45m, RegistrationFeeFirstSemester = 370m, RegistrationFeeRegularSemester = 370m, Capacity = 50, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 10, UniversityId = 5, ProgramId = 1, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 52.5m, RegistrationFeeFirstSemester = 370m, RegistrationFeeRegularSemester = 370m, Capacity = 60, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 11, UniversityId = 5, ProgramId = 3, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 75m, RegistrationFeeFirstSemester = 370m, RegistrationFeeRegularSemester = 370m, Capacity = 30, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 12, UniversityId = 5, ProgramId = 21, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 48.75m, RegistrationFeeFirstSemester = 370m, RegistrationFeeRegularSemester = 370m, Capacity = 30, IsActive = true, CreatedAt = SeedNow },

                    new UniversityProgram { Id = 13, UniversityId = 6, ProgramId = 17, StudySystem = StudySystem.Morning, DurationInYears = 5, SemesterStartDate = SemesterStart, HourPriceBase = 91m, RegistrationFeeFirstSemester = 500m, RegistrationFeeRegularSemester = 500m, Capacity = 25, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 14, UniversityId = 6, ProgramId = 1, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 80m, RegistrationFeeFirstSemester = 350m, RegistrationFeeRegularSemester = 350m, Capacity = 50, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 15, UniversityId = 6, ProgramId = 8, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 70m, RegistrationFeeFirstSemester = 350m, RegistrationFeeRegularSemester = 350m, Capacity = 30, IsActive = true, CreatedAt = SeedNow },

                    new UniversityProgram { Id = 16, UniversityId = 10, ProgramId = 1, StudySystem = StudySystem.Morning, DurationInYears = 4, SemesterStartDate = SemesterStart, HourPriceBase = 70m, RegistrationFeeFirstSemester = 450m, RegistrationFeeRegularSemester = 450m, Capacity = 60, IsActive = true, CreatedAt = SeedNow },

                    new UniversityProgram { Id = 17, UniversityId = 8, ProgramId = 4, StudySystem = StudySystem.Morning, DurationInYears = 2, SemesterStartDate = SemesterStart, HourPriceBase = 35m, RegistrationFeeFirstSemester = 200m, RegistrationFeeRegularSemester = 150m, Capacity = 40, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 18, UniversityId = 8, ProgramId = 5, StudySystem = StudySystem.Morning, DurationInYears = 2, SemesterStartDate = SemesterStart, HourPriceBase = 35m, RegistrationFeeFirstSemester = 200m, RegistrationFeeRegularSemester = 150m, Capacity = 40, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 19, UniversityId = 8, ProgramId = 6, StudySystem = StudySystem.Morning, DurationInYears = 2, SemesterStartDate = SemesterStart, HourPriceBase = 35m, RegistrationFeeFirstSemester = 200m, RegistrationFeeRegularSemester = 150m, Capacity = 30, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 20, UniversityId = 8, ProgramId = 14, StudySystem = StudySystem.Morning, DurationInYears = 2, SemesterStartDate = SemesterStart, HourPriceBase = 35m, RegistrationFeeFirstSemester = 200m, RegistrationFeeRegularSemester = 150m, Capacity = 35, IsActive = true, CreatedAt = SeedNow },
                    new UniversityProgram { Id = 21, UniversityId = 8, ProgramId = 18, StudySystem = StudySystem.Morning, DurationInYears = 2, SemesterStartDate = SemesterStart, HourPriceBase = 35m, RegistrationFeeFirstSemester = 200m, RegistrationFeeRegularSemester = 150m, Capacity = 30, IsActive = true, CreatedAt = SeedNow },
                };

                _db.UniversityPrograms.AddRange(programs);
                await _db.SaveChangesAsync(ct);
            }

            if (!await _db.EntryRequirements.AnyAsync(ct))
            {
                var uniPrograms = await _db.UniversityPrograms.AsNoTracking().ToListAsync(ct);

                var entryRequirements = new List<EntryRequirement>();
                int reqId = 1;

                foreach (var program in uniPrograms.OrderBy(x => x.Id))
                {
                    entryRequirements.Add(new EntryRequirement
                    {
                        Id = reqId++,
                        UniversityProgramId = program.Id,
                        MinGPA = program.HourPriceBase > 80 ? 75 : program.HourPriceBase > 60 ? 70 : 65,
                        Path = PathType.Academic,
                        AcademicTrack = AcademicTrack.Scientific,
                        AllowNoTawjihi = false,
                        EffectiveFrom = SeedNow
                    });

                    entryRequirements.Add(new EntryRequirement
                    {
                        Id = reqId++,
                        UniversityProgramId = program.Id,
                        MinGPA = program.HourPriceBase > 80 ? 70 : program.HourPriceBase > 60 ? 65 : 60,
                        Path = PathType.Academic,
                        AcademicTrack = AcademicTrack.Literary,
                        AllowNoTawjihi = false,
                        EffectiveFrom = SeedNow
                    });
                }

                _db.EntryRequirements.AddRange(entryRequirements);
                await _db.SaveChangesAsync(ct);
            }
        }

        private async Task SeedBtecProgramsAndReqAsync(CancellationToken ct)
        {
            if (!await _db.BtecPrograms.AnyAsync(ct))
            {
                _db.BtecPrograms.AddRange(
                    new BtecProgram
                    {
                        Id = 1,
                        UniversityId = 7,
                        NameArabic = "تقنيات الأعمال - BTEC",
                        NameEnglish = "Business Technologies - BTEC",
                        Description = "BTEC Level 5 in Business Technologies",
                        Level = BtecLevel.Level5,
                        TechnicalField = "Business Administration",
                        Language = LanguageCode.Both,
                        DurationInYears = 2,
                        SemesterStartDate = SemesterStart,
                        TotalCreditHours = 120,
                        HourPriceBase = 33.15m,
                        RegistrationFeeFirstSemester = 300m,
                        RegistrationFeeRegularSemester = 300m,
                        Capacity = 40,
                        IsActive = true,
                        IsApprovedByBtecAuthority = true,
                        ApprovalDate = new DateTime(2025, 06, 01, 0, 0, 0, DateTimeKind.Utc),
                        CreatedAt = SeedNow
                    },
                    new BtecProgram
                    {
                        Id = 2,
                        UniversityId = 11,
                        NameArabic = "الهندسة التقنية - BTEC",
                        NameEnglish = "Technical Engineering - BTEC",
                        Description = "BTEC Level 5 in Technical Engineering",
                        Level = BtecLevel.Level5,
                        TechnicalField = "Engineering",
                        Language = LanguageCode.Both,
                        DurationInYears = 2,
                        SemesterStartDate = SemesterStart,
                        TotalCreditHours = 120,
                        HourPriceBase = 100m,
                        RegistrationFeeFirstSemester = 350m,
                        RegistrationFeeRegularSemester = 350m,
                        Capacity = 30,
                        IsActive = true,
                        IsApprovedByBtecAuthority = true,
                        ApprovalDate = new DateTime(2025, 06, 01, 0, 0, 0, DateTimeKind.Utc),
                        CreatedAt = SeedNow
                    },
                    new BtecProgram
                    {
                        Id = 3,
                        UniversityId = 9,
                        NameArabic = "تكنولوجيا المعلومات - BTEC",
                        NameEnglish = "Information Technology - BTEC",
                        Description = "BTEC Level 5 in Information Technology",
                        Level = BtecLevel.Level5,
                        TechnicalField = "Information Technology",
                        Language = LanguageCode.Both,
                        DurationInYears = 2,
                        SemesterStartDate = SemesterStart,
                        TotalCreditHours = 120,
                        HourPriceBase = 28m,
                        RegistrationFeeFirstSemester = 250m,
                        RegistrationFeeRegularSemester = 250m,
                        Capacity = 35,
                        IsActive = true,
                        IsApprovedByBtecAuthority = true,
                        ApprovalDate = new DateTime(2025, 06, 01, 0, 0, 0, DateTimeKind.Utc),
                        CreatedAt = SeedNow
                    }
                );

                await _db.SaveChangesAsync(ct);
            }

            if (!await _db.BtecEntryRequirements.AnyAsync(ct))
            {
                _db.BtecEntryRequirements.AddRange(
                    new BtecEntryRequirement { Id = 1, BtecProgramId = 1, MinGPA = 60, RequiresBtecL2 = false, RequiresBtecL3 = false, Notes = "Accepts Tawjihi graduates with minimum 60%", EffectiveFrom = SeedNow },
                    new BtecEntryRequirement { Id = 2, BtecProgramId = 2, MinGPA = 65, RequiresBtecL2 = false, RequiresBtecL3 = false, Notes = "Accepts Tawjihi graduates with minimum 65%", EffectiveFrom = SeedNow },
                    new BtecEntryRequirement { Id = 3, BtecProgramId = 3, MinGPA = 60, RequiresBtecL2 = false, RequiresBtecL3 = false, Notes = "Accepts Tawjihi graduates with minimum 60%", EffectiveFrom = SeedNow }
                );

                await _db.SaveChangesAsync(ct);
            }
        }

        private async Task SeedUniversityRepresentativesAsync(CancellationToken ct)
        {
            if (await _db.UniversityRepresentatives.AnyAsync(ct)) return;

            _db.UniversityRepresentatives.AddRange(
                new UniversityRepresentative
                {
                    Id = 1,
                    UserId = "rep-001",
                    UniversityId = 1,
                    IsActive = true,
                    AssignedAt = SeedNow
                },
                new UniversityRepresentative
                {
                    Id = 2,
                    UserId = "rep-002",
                    UniversityId = 2,
                    IsActive = true,
                    AssignedAt = SeedNow
                }
            );

            await _db.SaveChangesAsync(ct);
        }

        private async Task SeedStudentsAsync(CancellationToken ct)
        {
            if (await _db.Students.AnyAsync(ct)) return;

            // Ensure users exist first (FK to AspNetUsers)
            var requiredUserIds = new[] { "student-001", "student-002", "student-003", "student-004" };
            var existingUserIds = await _db.Users
                .Where(u => requiredUserIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync(ct);

            var missing = requiredUserIds.Except(existingUserIds).ToList();
            if (missing.Any())
                throw new InvalidOperationException($"Missing AspNetUsers required for Students seed: {string.Join(", ", missing)}");

            var created1 = new DateTime(2025, 07, 01, 0, 0, 0, DateTimeKind.Utc);
            var created2 = new DateTime(2025, 08, 10, 0, 0, 0, DateTimeKind.Utc);
            var created3 = new DateTime(2025, 09, 05, 0, 0, 0, DateTimeKind.Utc);
            var created4 = new DateTime(2025, 10, 01, 0, 0, 0, DateTimeKind.Utc);

            _db.Students.AddRange(
                new Student
                {
                    Id = 1,
                    UserId = "student-001",
                    Province = "Amman",
                    City = "Amman",
                    Area = "Abdoun",
                    Latitude = 31.9539,
                    Longitude = 35.8666,
                    GPA = 85.5,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Scientific,
                    VocationalBranch = null,
                    BtecLevel2Completed = false,
                    BtecLevel3Completed = false,
                    BtecCertificateUrl = null,
                    RegistrationBudget = 10000m,
                    DesiredMajors = "الذكاء الاصطناعي, أمن المعلومات",
                    PreferredCity = "Amman",
                    MaxDistanceKm = 30,
                    PreferredLanguage = LanguageCode.Both,
                    HasFamilyConnection = false,
                    FamilyConnectionUniversityId = null,
                    CreatedAt = created1,
                    UpdatedAt = null,
                    NationalId = "1234567890",
                    SeatNumber = "A10001",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(2006, 03, 15),
                    Nationality = "Jordanian",
                    GuardianName = "Khaled Ahmad",
                    GuardianPhone = "0791111111",
                    GuardianRelation = "Father",
                    IsActive = true,
                    HasDisability = false,
                    DisabilityType = null,
                    IsOrphan = false,
                    IsEmployeeChild = false,
                    ProfileCompleted = true
                },
                new Student
                {
                    Id = 2,
                    UserId = "student-002",
                    Province = "Zarqa",
                    City = "Zarqa",
                    Area = "Al-Zarqa Al-Jadida",
                    Latitude = 32.0833,
                    Longitude = 36.0833,
                    GPA = 72.0,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Literary,
                    VocationalBranch = null,
                    BtecLevel2Completed = false,
                    BtecLevel3Completed = false,
                    BtecCertificateUrl = null,
                    RegistrationBudget = 6000m,
                    DesiredMajors = "التصميم الداخلي",
                    PreferredCity = "Amman",
                    MaxDistanceKm = 60,
                    PreferredLanguage = LanguageCode.Arabic,
                    HasFamilyConnection = true,
                    FamilyConnectionUniversityId = 2,
                    CreatedAt = created2,
                    UpdatedAt = null,
                    NationalId = "2234567890",
                    SeatNumber = "A20002",
                    Gender = Gender.Female,
                    DateOfBirth = new DateTime(2006, 07, 22),
                    Nationality = "Jordanian",
                    GuardianName = "Hassan Ali",
                    GuardianPhone = "0792222222",
                    GuardianRelation = "Father",
                    IsActive = true,
                    HasDisability = false,
                    DisabilityType = null,
                    IsOrphan = false,
                    IsEmployeeChild = false,
                    ProfileCompleted = true
                },
                new Student
                {
                    Id = 3,
                    UserId = "student-003",
                    Province = "Amman",
                    City = "Amman",
                    Area = "Marj Al-Hamam",
                    Latitude = 31.8794,
                    Longitude = 35.8414,
                    GPA = 68.0,
                    Path = PathType.BTEC,
                    AcademicTrack = null,
                    VocationalBranch = null,
                    BtecLevel2Completed = true,
                    BtecLevel3Completed = true,
                    BtecCertificateUrl = "/uploads/btec/omar_salem_btec_cert.pdf",
                    RegistrationBudget = 5000m,
                    DesiredMajors = "BTEC",
                    PreferredCity = "Amman",
                    MaxDistanceKm = 80,
                    PreferredLanguage = LanguageCode.English,
                    HasFamilyConnection = false,
                    FamilyConnectionUniversityId = null,
                    CreatedAt = created3,
                    UpdatedAt = null,
                    NationalId = "3234567890",
                    SeatNumber = "B30003",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(2006, 11, 08),
                    Nationality = "Jordanian",
                    GuardianName = "Salem Omar",
                    GuardianPhone = "0793333333",
                    GuardianRelation = "Father",
                    IsActive = true,
                    HasDisability = false,
                    DisabilityType = null,
                    IsOrphan = false,
                    IsEmployeeChild = false,
                    ProfileCompleted = true
                },
                new Student
                {
                    Id = 4,
                    UserId = "student-004",
                    Province = "Amman",
                    City = "Amman",
                    Area = "Shmeisani",
                    Latitude = 31.9636,
                    Longitude = 35.9094,
                    GPA = 92.5,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Scientific,
                    VocationalBranch = null,
                    BtecLevel2Completed = false,
                    BtecLevel3Completed = false,
                    BtecCertificateUrl = null,
                    RegistrationBudget = 15000m,
                    DesiredMajors = "طب الأسنان",
                    PreferredCity = "Amman",
                    MaxDistanceKm = 20,
                    PreferredLanguage = LanguageCode.Both,
                    HasFamilyConnection = false,
                    FamilyConnectionUniversityId = null,
                    CreatedAt = created4,
                    UpdatedAt = null,
                    NationalId = "4234567890",
                    SeatNumber = "A40004",
                    Gender = Gender.Female,
                    DateOfBirth = new DateTime(2006, 01, 30),
                    Nationality = "Jordanian",
                    GuardianName = "Ibrahim Mahmoud",
                    GuardianPhone = "0794444444",
                    GuardianRelation = "Father",
                    IsActive = true,
                    HasDisability = false,
                    DisabilityType = null,
                    IsOrphan = false,
                    IsEmployeeChild = false,
                    ProfileCompleted = true
                }
            );

            await _db.SaveChangesAsync(ct);
        }

        private async Task SeedApplicationsCommissionDiscountAsync(CancellationToken ct)
        {
            if (!await _db.StudentApplications.AnyAsync(ct))
            {
                var appBaseDate = new DateTime(2025, 11, 01, 0, 0, 0, DateTimeKind.Utc);

                _db.StudentApplications.AddRange(
                    new StudentApplication
                    {
                        Id = 1,
                        StudentId = 1,
                        UniversityProgramId = 3,
                        BtecProgramId = null,
                        Status = ApplicationStatus.Approved,
                        ApplicationDate = appBaseDate,
                        ApprovalDate = appBaseDate.AddDays(5),
                        UpdatedAt = appBaseDate.AddDays(5),
                        Notes = "Application approved. Waiting for student confirmation.",
                        PlannedFirstSemesterHours = 15,
                        HourDiscountPercent = 35m,
                        HourDiscountSetByUserId = "rep-002",
                        HourDiscountSetAt = appBaseDate.AddDays(5),
                        ApplicationNumber = "APP-2025-001",
                        AdmissionNumber = "ADM-MEU-2025-001",
                        RejectionReason = null
                    },
                    new StudentApplication
                    {
                        Id = 2,
                        StudentId = 1,
                        UniversityProgramId = 2,
                        BtecProgramId = null,
                        Status = ApplicationStatus.UnderReview,
                        ApplicationDate = appBaseDate.AddDays(2),
                        ApprovalDate = null,
                        UpdatedAt = appBaseDate.AddDays(2),
                        Notes = "Under review by university representative.",
                        PlannedFirstSemesterHours = 15,
                        HourDiscountPercent = null,
                        HourDiscountSetByUserId = null,
                        HourDiscountSetAt = null,
                        ApplicationNumber = "APP-2025-002",
                        AdmissionNumber = null,
                        RejectionReason = null
                    },
                    new StudentApplication
                    {
                        Id = 3,
                        StudentId = 2,
                        UniversityProgramId = 5,
                        BtecProgramId = null,
                        Status = ApplicationStatus.Pending,
                        ApplicationDate = appBaseDate.AddDays(7),
                        ApprovalDate = null,
                        UpdatedAt = appBaseDate.AddDays(7),
                        Notes = "New application submitted.",
                        PlannedFirstSemesterHours = 12,
                        HourDiscountPercent = null,
                        HourDiscountSetByUserId = null,
                        HourDiscountSetAt = null,
                        ApplicationNumber = "APP-2025-003",
                        AdmissionNumber = null,
                        RejectionReason = null
                    },
                    new StudentApplication
                    {
                        Id = 4,
                        StudentId = 3,
                        UniversityProgramId = null,
                        BtecProgramId = 1,
                        Status = ApplicationStatus.Approved,
                        ApplicationDate = appBaseDate.AddDays(10),
                        ApprovalDate = appBaseDate.AddDays(14),
                        UpdatedAt = appBaseDate.AddDays(14),
                        Notes = "BTEC application approved by NUCT.",
                        PlannedFirstSemesterHours = 18,
                        HourDiscountPercent = 30m,
                        HourDiscountSetByUserId = "rep-001",
                        HourDiscountSetAt = appBaseDate.AddDays(14),
                        ApplicationNumber = "APP-2025-004",
                        AdmissionNumber = "ADM-NUCT-2025-001",
                        RejectionReason = null
                    },
                    new StudentApplication
                    {
                        Id = 5,
                        StudentId = 4,
                        UniversityProgramId = 13,
                        BtecProgramId = null,
                        Status = ApplicationStatus.Enrolled,
                        ApplicationDate = appBaseDate.AddDays(3),
                        ApprovalDate = appBaseDate.AddDays(6),
                        UpdatedAt = appBaseDate.AddDays(20),
                        Notes = "Student enrolled and paid first semester registration.",
                        PlannedFirstSemesterHours = 18,
                        HourDiscountPercent = 40m,
                        HourDiscountSetByUserId = "admin-001",
                        HourDiscountSetAt = appBaseDate.AddDays(6),
                        ApplicationNumber = "APP-2025-005",
                        AdmissionNumber = "ADM-AAU-2025-001",
                        RejectionReason = null
                    },
                    new StudentApplication
                    {
                        Id = 6,
                        StudentId = 2,
                        UniversityProgramId = 21,
                        BtecProgramId = null,
                        Status = ApplicationStatus.UnderReview,
                        ApplicationDate = appBaseDate.AddDays(12),
                        ApprovalDate = null,
                        UpdatedAt = appBaseDate.AddDays(13),
                        Notes = "Under review. Need additional documents.",
                        PlannedFirstSemesterHours = 15,
                        HourDiscountPercent = null,
                        HourDiscountSetByUserId = null,
                        HourDiscountSetAt = null,
                        ApplicationNumber = "APP-2025-006",
                        AdmissionNumber = null,
                        RejectionReason = null
                    }
                );

                await _db.SaveChangesAsync(ct);
            }

            if (!await _db.Commissions.AnyAsync(ct))
            {
                var appBaseDate = new DateTime(2025, 11, 01, 0, 0, 0, DateTimeKind.Utc);

                _db.Commissions.AddRange(
                    new Commission
                    {
                        Id = 1,
                        ApplicationId = 1,
                        UniversityId = 2,
                        Mode = CommissionMode.FirstSemesterRegistration2Percent,
                        Percentage = 2m,
                        BaseAmount = 450m,
                        RegistrationFeeUsed = 450m,
                        AmountEstimated = 9m,
                        Settled = false,
                        CreatedAt = appBaseDate.AddDays(5),
                        CalculatedAt = appBaseDate.AddDays(5),
                        HoursCountUsed = null,
                        HourPriceUsed = null,
                        DiscountPercentApplied = null,
                        MonthlySettlementId = null
                    },
                    new Commission
                    {
                        Id = 2,
                        ApplicationId = 4,
                        UniversityId = 7,
                        Mode = CommissionMode.FirstSemesterRegistration2Percent,
                        Percentage = 2m,
                        BaseAmount = 300m,
                        RegistrationFeeUsed = 300m,
                        AmountEstimated = 6m,
                        Settled = false,
                        CreatedAt = appBaseDate.AddDays(14),
                        CalculatedAt = appBaseDate.AddDays(14),
                        HoursCountUsed = null,
                        HourPriceUsed = null,
                        DiscountPercentApplied = null,
                        MonthlySettlementId = null
                    },
                    new Commission
                    {
                        Id = 3,
                        ApplicationId = 5,
                        UniversityId = 6,
                        Mode = CommissionMode.FirstSemesterRegistration2Percent,
                        Percentage = 2m,
                        BaseAmount = 500m,
                        RegistrationFeeUsed = 500m,
                        AmountEstimated = 10m,
                        Settled = false,
                        CreatedAt = appBaseDate.AddDays(6),
                        CalculatedAt = appBaseDate.AddDays(6),
                        HoursCountUsed = null,
                        HourPriceUsed = null,
                        DiscountPercentApplied = null,
                        MonthlySettlementId = null
                    }
                );

                await _db.SaveChangesAsync(ct);
            }

            if (!await _db.DiscountGrants.AnyAsync(ct))
            {
                var appBaseDate = new DateTime(2025, 11, 01, 0, 0, 0, DateTimeKind.Utc);

                _db.DiscountGrants.AddRange(
                    new DiscountGrant
                    {
                        Id = 1,
                        ApplicationId = 1,
                        Code = "DISC-MEU-2025-001",
                        UniversityId = 2,
                        Percentage = 5m,
                        AmountEstimated = 22.5m,
                        Status = DiscountStatus.Issued,
                        GrantedAt = appBaseDate.AddDays(5),
                        RedeemedAt = null,
                        RedeemedByUserId = null
                    },
                    new DiscountGrant
                    {
                        Id = 2,
                        ApplicationId = 4,
                        Code = "DISC-NUCT-2025-001",
                        UniversityId = 7,
                        Percentage = 5m,
                        AmountEstimated = 15m,
                        Status = DiscountStatus.Redeemed,
                        GrantedAt = appBaseDate.AddDays(14),
                        RedeemedAt = appBaseDate.AddDays(20),
                        RedeemedByUserId = "rep-001"
                    },
                    new DiscountGrant
                    {
                        Id = 3,
                        ApplicationId = 5,
                        Code = "DISC-AAU-2025-001",
                        UniversityId = 6,
                        Percentage = 5m,
                        AmountEstimated = 25m,
                        Status = DiscountStatus.Redeemed,
                        GrantedAt = appBaseDate.AddDays(6),
                        RedeemedAt = appBaseDate.AddDays(15),
                        RedeemedByUserId = "rep-002"
                    }
                );

                await _db.SaveChangesAsync(ct);
            }
        }
    }
}



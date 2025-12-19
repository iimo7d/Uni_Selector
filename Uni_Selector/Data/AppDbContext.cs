using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uni_Selector.Models;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Student> Students { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<UniversityRepresentative> UniversityRepresentatives { get; set; }
        public DbSet<ProgramEntity> Programs { get; set; }
        public DbSet<UniversityProgram> UniversityPrograms { get; set; }
        public DbSet<EntryRequirement> EntryRequirements { get; set; }
        public DbSet<BtecProgram> BtecPrograms { get; set; }
        public DbSet<BtecEntryRequirement> BtecEntryRequirements { get; set; }
        public DbSet<StudentApplication> StudentApplications { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<DiscountGrant> DiscountGrants { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        public DbSet<MonthlySettlement> MonthlySettlements { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Configure Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => new { e.Province, e.City });
                entity.HasIndex(e => e.GPA);
                entity.HasIndex(e => e.Path);
                entity.HasIndex(e => e.IsActive);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure University
            modelBuilder.Entity<University>(entity =>
            {
                entity.HasIndex(e => e.NameArabic);
                entity.HasIndex(e => e.NameEnglish);
                entity.HasIndex(e => new { e.Province, e.City });
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Type);
            });

            // Configure UniversityRepresentative
            modelBuilder.Entity<UniversityRepresentative>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.UniversityId);
                entity.HasIndex(e => e.IsActive);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.University)
                    .WithMany(u => u.Representatives)
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ProgramEntity
            modelBuilder.Entity<ProgramEntity>(entity =>
            {
                entity.HasIndex(e => e.NameArabic);
                entity.HasIndex(e => e.NameEnglish);
                entity.HasIndex(e => e.Degree);
                entity.HasIndex(e => e.Language);
            });

            // Configure UniversityProgram
            modelBuilder.Entity<UniversityProgram>(entity =>
            {
                entity.HasIndex(e => e.UniversityId);
                entity.HasIndex(e => e.ProgramId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.UniversityId, e.ProgramId, e.StudySystem });

                entity.HasOne(e => e.University)
                    .WithMany(u => u.UniversityPrograms)
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Program)
                    .WithMany(p => p.UniversityPrograms)
                    .HasForeignKey(e => e.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure EntryRequirement
            modelBuilder.Entity<EntryRequirement>(entity =>
            {
                entity.HasIndex(e => e.UniversityProgramId);
                entity.HasIndex(e => e.Path);
                entity.HasIndex(e => e.MinGPA);

                entity.HasOne(e => e.UniversityProgram)
                    .WithMany(up => up.EntryRequirements)
                    .HasForeignKey(e => e.UniversityProgramId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BtecProgram
            modelBuilder.Entity<BtecProgram>(entity =>
            {
                entity.HasIndex(e => e.UniversityId);
                entity.HasIndex(e => e.Level);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.IsApprovedByBtecAuthority);
                entity.HasIndex(e => e.TechnicalField);

                entity.HasOne(e => e.University)
                    .WithMany(u => u.BtecPrograms)
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BtecEntryRequirement
            modelBuilder.Entity<BtecEntryRequirement>(entity =>
            {
                entity.HasIndex(e => e.BtecProgramId);
                entity.HasIndex(e => e.MinGPA);

                entity.HasOne(e => e.BtecProgram)
                    .WithMany(bp => bp.EntryRequirements)
                    .HasForeignKey(e => e.BtecProgramId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure StudentApplication
            modelBuilder.Entity<StudentApplication>(entity =>
            {
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.UniversityProgramId);
                entity.HasIndex(e => e.BtecProgramId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ApplicationDate);
                entity.HasIndex(e => e.ApplicationNumber).IsUnique();

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Applications)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UniversityProgram)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityProgramId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BtecProgram)
                    .WithMany()
                    .HasForeignKey(e => e.BtecProgramId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.HourDiscountSetByUser)
                    .WithMany()
                    .HasForeignKey(e => e.HourDiscountSetByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Recommendation
            modelBuilder.Entity<Recommendation>(entity =>
            {
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.UniversityProgramId);
                entity.HasIndex(e => e.BtecProgramId);
                entity.HasIndex(e => e.Score);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Recommendations)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.UniversityProgram)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityProgramId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BtecProgram)
                    .WithMany()
                    .HasForeignKey(e => e.BtecProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure DiscountGrant
            modelBuilder.Entity<DiscountGrant>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.ApplicationId).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.GrantedAt);

                entity.HasOne(e => e.Application)
                    .WithOne(a => a.DiscountGrant)
                    .HasForeignKey<DiscountGrant>(e => e.ApplicationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.University)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RedeemedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.RedeemedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Commission
            modelBuilder.Entity<Commission>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId).IsUnique();
                entity.HasIndex(e => e.UniversityId);
                entity.HasIndex(e => e.Settled);
                entity.HasIndex(e => e.MonthlySettlementId);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.Application)
                    .WithOne(a => a.Commission)
                    .HasForeignKey<Commission>(e => e.ApplicationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.University)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.MonthlySettlement)
                    .WithMany(ms => ms.Commissions)
                    .HasForeignKey(e => e.MonthlySettlementId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure MonthlySettlement
            modelBuilder.Entity<MonthlySettlement>(entity =>
            {
                entity.HasIndex(e => new { e.UniversityId, e.Year, e.Month }).IsUnique();
                entity.HasIndex(e => e.Closed);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.University)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ClosedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ClosedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Category);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025,11,1); 

            // Seed Roles
            var roles = new[]
            {
                new IdentityRole
                {
                    Id = "247845c3-87f8-46f1-a455-555c330c3c41",
                    Name = Uni_Selector.Models.UserRoles.Student,
                    NormalizedName = Uni_Selector.Models.UserRoles.Student.ToUpper(),
                    ConcurrencyStamp = "d72e20f4-8245-4a69-ad2b-358a14cd4ce7"
                },
                new IdentityRole
                {
                    Id = "80f3af8c-17c5-456e-a4cc-15bb694e863b",
                    Name = Uni_Selector.Models.UserRoles.UniversityRep,
                    NormalizedName = Uni_Selector.Models.UserRoles.UniversityRep.ToUpper(),
                    ConcurrencyStamp = "5b6cd610-3ead-4ec8-8d9d-2d4daf456312"
                },
                new IdentityRole
                {
                    Id = "dd348c1a-172c-41ce-81e7-d096a7596f93",
                    Name = Uni_Selector.Models.UserRoles.PlatformAdmin,
                    NormalizedName = Uni_Selector.Models.UserRoles.PlatformAdmin.ToUpper(),
                    ConcurrencyStamp = "afa21429-070e-41b0-ad52-91e817a3beb2"
                },
                new IdentityRole
                {
                    Id = "912202c3-2b0f-4a73-94e5-6d85d64fa4db",
                    Name = Uni_Selector.Models.UserRoles.BtecAuthority,
                    NormalizedName = Uni_Selector.Models.UserRoles.BtecAuthority.ToUpper(),
                    ConcurrencyStamp = "bf10b102-86d0-4caa-8e36-3434a5bb91c4"
                }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            // Seed Admin User
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = "bc7c5647-7196-414a-b94a-5c1a51ff492c",
                UserName = "admin@uniselector.com",
                NormalizedUserName = "ADMIN@UNISELECTOR.COM",
                Email = "admin@uniselector.com",
                NormalizedEmail = "ADMIN@UNISELECTOR.COM",
                EmailConfirmed = true,
                FullName = "Platform Administrator",
                IsActive = true,
                CreatedAt = now,
                SecurityStamp = "19cfcb40-6c5b-4e83-8796-ab1abf7741f9",
                ConcurrencyStamp = "4d33e595-96f6-48db-a638-33e59adba91b"
            };
            adminUser.PasswordHash = "AQAAAAIAAYagAAAAEPFUaq3Y9kfJmwtE4V8iv3DpjWSDEPZV/zgI8eRG5DA/AI4lc65U32nkR5GAysqXmA==";
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            // Assign Admin Role
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = adminUser.Id,
                RoleId = "dd348c1a-172c-41ce-81e7-d096a7596f93" // PlatformAdmin
            });

            // Seed Sample Universities
            var universities = new[]
            {
                new University
                {
                    Id = 1,
                    NameArabic = "الجامعة الأردنية",
                    NameEnglish = "University of Jordan",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Queen Rania Street, Amman 11942, Jordan",
                    Latitude = 31.9539,
                    Longitude = 35.9106,
                    PhoneNumber = "+96265355000",
                    Email = "info@ju.edu.jo",
                    OfficialWebsite = "https://www.ju.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegistration2Percent,
                    CreatedAt = now
                },
                new University
                {
                    Id = 2,
                    NameArabic = "الجامعة الهاشمية",
                    NameEnglish = "The Hashemite University",
                    Type = UniversityType.Private,
                    Province = "Zarqa",
                    City = "Zarqa",
                    FullAddress = "P.O. Box 330127, Zarqa 13133, Jordan",
                    Latitude = 32.1365,
                    Longitude = 36.0022,
                    PhoneNumber = "+96253903333",
                    Email = "info@hu.edu.jo",
                    OfficialWebsite = "https://www.hu.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.ProgramTotalHours2Percent,
                    CreatedAt = now
                },
                new University
                {
                    Id = 3,
                    NameArabic = "جامعة العلوم التطبيقية",
                    NameEnglish = "Applied Science Private University",
                    Type = UniversityType.Private,
                    Province = "Amman",
                    City = "Amman",
                    FullAddress = "Al-Arab Street, Shafa Badran, Amman 11931, Jordan",
                    Latitude = 32.0182,
                    Longitude = 35.8767,
                    PhoneNumber = "+96264609999",
                    Email = "info@asu.edu.jo",
                    OfficialWebsite = "https://www.asu.edu.jo",
                    IsActive = true,
                    CommissionMode = CommissionMode.FirstSemesterRegPlusHours2Percent,
                    CreatedAt = now
                }
            };
            modelBuilder.Entity<University>().HasData(universities);

            // Seed Sample Programs
            var programs = new[]
            {
                new ProgramEntity
                {
                    Id = 1,
                    NameArabic = "هندسة البرمجيات",
                    NameEnglish = "Software Engineering",
                    Description = "Bachelor's degree in Software Engineering",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.English,
                    AcademicClassification = "Engineering",
                    TotalCreditHours = 132,
                    CreatedAt = now
                },
                new ProgramEntity
                {
                    Id = 2,
                    NameArabic = "علوم الحاسوب",
                    NameEnglish = "Computer Science",
                    Description = "Bachelor's degree in Computer Science",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.English,
                    AcademicClassification = "Engineering",
                    TotalCreditHours = 132,
                    CreatedAt = now
                },
                new ProgramEntity
                {
                    Id = 3,
                    NameArabic = "إدارة الأعمال",
                    NameEnglish = "Business Administration",
                    Description = "Bachelor's degree in Business Administration",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.Both,
                    AcademicClassification = "Business",
                    TotalCreditHours = 126,
                    CreatedAt = now
                },
                new ProgramEntity
                {
                    Id = 4,
                    NameArabic = "الطب البشري",
                    NameEnglish = "Medicine",
                    Description = "Medical Doctor degree",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.English,
                    AcademicClassification = "Medical",
                    TotalCreditHours = 240,
                    CreatedAt = now
                },
                new ProgramEntity
                {
                    Id = 5,
                    NameArabic = "القانون",
                    NameEnglish = "Law",
                    Description = "Bachelor's degree in Law",
                    Degree = Degree.Bachelor,
                    Language = LanguageCode.Arabic,
                    AcademicClassification = "Law",
                    TotalCreditHours = 132,
                    CreatedAt = now
                }
            };
            modelBuilder.Entity<ProgramEntity>().HasData(programs);

            // Seed University Programs
            var universityPrograms = new[]
            {
                // University of Jordan Programs
                new UniversityProgram
                {
                    Id = 1,
                    UniversityId = 1,
                    ProgramId = 1,
                    StudySystem = StudySystem.Morning,
                    DurationInYears = 4,
                    SemesterStartDate = new DateTime(2024, 9, 1),
                    HourPriceBase = 85m,
                    RegistrationFeeFirstSemester = 500m,
                    RegistrationFeeRegularSemester = 300m,
                    Capacity = 100,
                    IsActive = true,
                    CreatedAt = now
                },
                new UniversityProgram
                {
                    Id = 2,
                    UniversityId = 1,
                    ProgramId = 2,
                    StudySystem = StudySystem.Morning,
                    DurationInYears = 4,
                    SemesterStartDate = new DateTime(2024, 9, 1),
                    HourPriceBase = 80m,
                    RegistrationFeeFirstSemester = 500m,
                    RegistrationFeeRegularSemester = 300m,
                    Capacity = 120,
                    IsActive = true,
                    CreatedAt = now
                },
                // Hashemite University Programs
                new UniversityProgram
                {
                    Id = 3,
                    UniversityId = 2,
                    ProgramId = 3,
                    StudySystem = StudySystem.Morning,
                    DurationInYears = 4,
                    SemesterStartDate = new DateTime(2024, 9, 1),
                    HourPriceBase = 70m,
                    RegistrationFeeFirstSemester = 400m,
                    RegistrationFeeRegularSemester = 250m,
                    Capacity = 150,
                    IsActive = true,
                    CreatedAt = now
                },
                // Applied Science University Programs
                new UniversityProgram
                {
                    Id = 4,
                    UniversityId = 3,
                    ProgramId = 1,
                    StudySystem = StudySystem.Evening,
                    DurationInYears = 4,
                    SemesterStartDate = new DateTime(2024, 9, 1),
                    HourPriceBase = 90m,
                    RegistrationFeeFirstSemester = 600m,
                    RegistrationFeeRegularSemester = 350m,
                    Capacity = 80,
                    IsActive = true,
                    CreatedAt = now
                },
                new UniversityProgram
                {
                    Id = 5,
                    UniversityId = 3,
                    ProgramId = 5,
                    StudySystem = StudySystem.Morning,
                    DurationInYears = 4,
                    SemesterStartDate = new DateTime(2024, 9, 1),
                    HourPriceBase = 75m,
                    RegistrationFeeFirstSemester = 450m,
                    RegistrationFeeRegularSemester = 275m,
                    Capacity = 100,
                    IsActive = true,
                    CreatedAt = now
                }
            };
            modelBuilder.Entity<UniversityProgram>().HasData(universityPrograms);

            // Seed Entry Requirements
            var entryRequirements = new[]
            {
                // Software Engineering - Scientific Track
                new EntryRequirement
                {
                    Id = 1,
                    UniversityProgramId = 1,
                    MinGPA = 80.0,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Scientific,
                    AllowNoTawjihi = false,
                    EffectiveFrom = new DateTime(2024, 1, 1)
                },
                // Computer Science - Scientific Track
                new EntryRequirement
                {
                    Id = 2,
                    UniversityProgramId = 2,
                    MinGPA = 75.0,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Scientific,
                    AllowNoTawjihi = false,
                    EffectiveFrom = new DateTime(2024, 1, 1)
                },
                // Business Administration - Literary Track
                new EntryRequirement
                {
                    Id = 3,
                    UniversityProgramId = 3,
                    MinGPA = 70.0,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Literary,
                    AllowNoTawjihi = false,
                    EffectiveFrom = new DateTime(2024, 1, 1)
                },
                // Business Administration - Scientific Track
                new EntryRequirement
                {
                    Id = 4,
                    UniversityProgramId = 3,
                    MinGPA = 68.0,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Scientific,
                    AllowNoTawjihi = false,
                    EffectiveFrom = new DateTime(2024, 1, 1)
                },
                // Software Engineering (Evening) - Scientific Track
                new EntryRequirement
                {
                    Id = 5,
                    UniversityProgramId = 4,
                    MinGPA = 75.0,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Scientific,
                    AllowNoTawjihi = false,
                    EffectiveFrom = new DateTime(2024, 1, 1)
                },
                // Law - Literary Track
                new EntryRequirement
                {
                    Id = 6,
                    UniversityProgramId = 5,
                    MinGPA = 72.0,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Literary,
                    AllowNoTawjihi = false,
                    EffectiveFrom = new DateTime(2024, 1, 1)
                },
                // Law - Sharia Track
                new EntryRequirement
                {
                    Id = 7,
                    UniversityProgramId = 5,
                    MinGPA = 70.0,
                    Path = PathType.Academic,
                    AcademicTrack = AcademicTrack.Sharia,
                    AllowNoTawjihi = false,
                    EffectiveFrom = new DateTime(2024, 1, 1)
                }
            };
            modelBuilder.Entity<EntryRequirement>().HasData(entryRequirements);
        }
    }
}
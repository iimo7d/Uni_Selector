using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uni_Selector.Models;

namespace Uni_Selector.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        #region DbSets
        public DbSet<Student> Students { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<UniversityRepresentative> UniversityRepresentatives { get; set; }
        public DbSet<ProgramEntity> Programs { get; set; }
        public DbSet<UniversityProgram> UniversityPrograms { get; set; }
        public DbSet<EntryRequirement> EntryRequirements { get; set; }
        public DbSet<BtecProgram> BtecPrograms { get; set; }
        public DbSet<BtecEntryRequirement> BtecEntryRequirements { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<StudentApplication> StudentApplications { get; set; }
        public DbSet<DiscountGrant> DiscountGrants { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        public DbSet<MonthlySettlement> MonthlySettlements { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Identity Tables Schema
            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "sec");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", "sec");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "sec");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "sec");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "sec");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "sec");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "sec");
            #endregion

            #region ApplicationUser Configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(256);
            });
            #endregion

            #region Student Configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.GPA);
                entity.HasIndex(e => e.Path);
                entity.HasIndex(e => new { e.Province, e.City });
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FamilyConnectionUniversity)
                    .WithMany()
                    .HasForeignKey(e => e.FamilyConnectionUniversityId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(e => e.RegistrationBudget)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.GPA)
                    .HasPrecision(5, 2);
            });
            #endregion

            #region University Configuration
            modelBuilder.Entity<University>(entity =>
            {
                entity.HasIndex(e => e.NameEnglish);
                entity.HasIndex(e => e.NameArabic);
                entity.HasIndex(e => new { e.Province, e.City });
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CommissionMode);

                // Defaults are set in the model class
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });
            #endregion

            #region UniversityRepresentative Configuration
            modelBuilder.Entity<UniversityRepresentative>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
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
            #endregion

            #region ProgramEntity Configuration
            modelBuilder.Entity<ProgramEntity>(entity =>
            {
                entity.HasIndex(e => e.NameEnglish);
                entity.HasIndex(e => e.NameArabic);
                entity.HasIndex(e => e.Degree);
                entity.HasIndex(e => e.Language);
                entity.HasIndex(e => new { e.Degree, e.Language });

                entity.Property(e => e.TotalCreditHours)
                    .HasDefaultValue(120);
            });
            #endregion

            #region UniversityProgram Configuration
            modelBuilder.Entity<UniversityProgram>(entity =>
            {
                entity.HasIndex(e => new { e.UniversityId, e.ProgramId });
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.StudySystem);
                entity.HasIndex(e => e.SemesterStartDate);

                entity.HasOne(e => e.University)
                    .WithMany(u => u.UniversityPrograms)
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Program)
                    .WithMany(p => p.UniversityPrograms)
                    .HasForeignKey(e => e.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.HourPriceBase)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RegistrationFeeFirstSemester)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RegistrationFeeRegularSemester)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.Capacity)
                    .HasDefaultValue(0);
            });
            #endregion

            #region EntryRequirement Configuration
            modelBuilder.Entity<EntryRequirement>(entity =>
            {
                entity.HasIndex(e => e.UniversityProgramId);
                entity.HasIndex(e => e.Path);
                entity.HasIndex(e => e.AcademicTrack);
                entity.HasIndex(e => e.VocationalBranch);
                entity.HasIndex(e => e.MinGPA);

                entity.HasOne(e => e.UniversityProgram)
                    .WithMany(up => up.EntryRequirements)
                    .HasForeignKey(e => e.UniversityProgramId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.MinGPA)
                    .HasPrecision(5, 2);

                entity.Property(e => e.AllowNoTawjihi)
                    .HasDefaultValue(false);
            });
            #endregion

            #region BtecProgram Configuration
            modelBuilder.Entity<BtecProgram>(entity =>
            {
                entity.HasIndex(e => e.UniversityId);
                entity.HasIndex(e => e.IsApprovedByBtecAuthority);
                entity.HasIndex(e => e.Level);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.TechnicalField);

                entity.HasOne(e => e.University)
                    .WithMany(u => u.BtecPrograms)
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.HourPriceBase)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RegistrationFeeFirstSemester)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RegistrationFeeRegularSemester)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.IsApprovedByBtecAuthority)
                    .HasDefaultValue(false);

                entity.Property(e => e.Capacity)
                    .HasDefaultValue(0);

                entity.Property(e => e.TotalCreditHours)
                    .HasDefaultValue(120);
            });
            #endregion

            #region BtecEntryRequirement Configuration
            modelBuilder.Entity<BtecEntryRequirement>(entity =>
            {
                entity.HasIndex(e => e.BtecProgramId);
                entity.HasIndex(e => e.MinGPA);

                entity.HasOne(e => e.BtecProgram)
                    .WithMany(bp => bp.EntryRequirements)
                    .HasForeignKey(e => e.BtecProgramId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.MinGPA)
                    .HasPrecision(5, 2);

                entity.Property(e => e.RequiresBtecL2)
                    .HasDefaultValue(true);

                entity.Property(e => e.RequiresBtecL3)
                    .HasDefaultValue(true);
            });
            #endregion

            #region Recommendation Configuration
            modelBuilder.Entity<Recommendation>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.Score });
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.IsViewed);
                entity.HasIndex(e => new { e.StudentId, e.CreatedAt });

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

                entity.Property(e => e.EstimatedTotalCost)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Score)
                    .HasPrecision(5, 2);

                entity.Property(e => e.IsViewed)
                    .HasDefaultValue(false);

                // Check constraint to ensure only one program type is set
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Recommendation_OneProgram",
                    "(UniversityProgramId IS NOT NULL AND BtecProgramId IS NULL) OR (UniversityProgramId IS NULL AND BtecProgramId IS NOT NULL)"
                ));
            });
            #endregion

            #region StudentApplication Configuration
            modelBuilder.Entity<StudentApplication>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.ApplicationDate });
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.Status, e.ApplicationDate });
                entity.HasIndex(e => e.UniversityProgramId);
                entity.HasIndex(e => e.BtecProgramId);

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Applications)
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

                entity.HasOne(e => e.HourDiscountSetByUser)
                    .WithMany()
                    .HasForeignKey(e => e.HourDiscountSetByUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Default is set in the model class
                entity.Property(e => e.HourDiscountPercent)
                    .HasColumnType("decimal(5,2)");

                // Check constraint to ensure only one program type is set
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Application_OneProgram",
                    "(UniversityProgramId IS NOT NULL AND BtecProgramId IS NULL) OR (UniversityProgramId IS NULL AND BtecProgramId IS NOT NULL)"
                ));
            });
            #endregion

            #region DiscountGrant Configuration
            modelBuilder.Entity<DiscountGrant>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => new { e.UniversityId, e.Status });
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.GrantedAt);

                entity.HasOne(e => e.Application)
                    .WithOne(a => a.DiscountGrant)
                    .HasForeignKey<DiscountGrant>(e => e.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.University)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RedeemedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.RedeemedByUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Defaults are set in the model class
                entity.Property(e => e.Percentage)
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.AmountEstimated)
                    .HasColumnType("decimal(18,2)");
            });
            #endregion

            #region Commission Configuration
            modelBuilder.Entity<Commission>(entity =>
            {
                entity.HasIndex(e => e.Settled);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.UniversityId, e.Settled });
                entity.HasIndex(e => e.Mode);
                entity.HasIndex(e => new { e.UniversityId, e.CreatedAt });

                entity.HasOne(e => e.Application)
                    .WithOne(a => a.Commission)
                    .HasForeignKey<Commission>(e => e.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.University)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.MonthlySettlement)
                    .WithMany(ms => ms.Commissions)
                    .HasForeignKey(e => e.MonthlySettlementId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Defaults are set in the model class
                entity.Property(e => e.Percentage)
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.BaseAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.AmountEstimated)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RegistrationFeeUsed)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.HourPriceUsed)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.DiscountPercentApplied)
                    .HasColumnType("decimal(5,2)");
            });
            #endregion

            #region MonthlySettlement Configuration
            modelBuilder.Entity<MonthlySettlement>(entity =>
            {
                entity.HasIndex(e => new { e.UniversityId, e.Year, e.Month }).IsUnique();
                entity.HasIndex(e => e.Closed);
                entity.HasIndex(e => new { e.UniversityId, e.Closed });
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.University)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ClosedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ClosedByUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(e => e.TotalCommission)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Closed)
                    .HasDefaultValue(false);
            });
            #endregion

            #region Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.IsRead });
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => new { e.UserId, e.Category, e.IsRead });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.IsRead)
                    .HasDefaultValue(false);

                entity.Property(e => e.EmailSent)
                    .HasDefaultValue(false);

                entity.Property(e => e.SmsSent)
                    .HasDefaultValue(false);
            });
            #endregion
        }
    }
}
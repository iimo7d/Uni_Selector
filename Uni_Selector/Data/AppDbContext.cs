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

        #region Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Change Identity table names
            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "sec");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", "sec");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "sec");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "sec");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "sec");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "sec");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "sec");
            #endregion

            #region Student Configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FamilyConnectionUniversity)
                    .WithMany()
                    .HasForeignKey(e => e.FamilyConnectionUniversityId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            #endregion

            #region University Representative Configuration
            modelBuilder.Entity<UniversityRepresentative>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();

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

            #region University Configuration
            modelBuilder.Entity<University>(entity =>
            {
                entity.HasIndex(e => e.NameEnglish);
                entity.HasIndex(e => e.NameArabic);
                entity.HasIndex(e => new { e.City, e.Province });
            });
            #endregion

            #region Program Entity Configuration
            modelBuilder.Entity<ProgramEntity>(entity =>
            {
                entity.HasIndex(e => e.NameEnglish);
                entity.HasIndex(e => e.NameArabic);
                entity.HasIndex(e => e.Degree);
            });
            #endregion

            #region University Program Configuration
            modelBuilder.Entity<UniversityProgram>(entity =>
            {
                entity.HasIndex(e => new { e.UniversityId, e.ProgramId });

                entity.HasOne(e => e.University)
                    .WithMany(u => u.UniversityPrograms)
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Program)
                    .WithMany(p => p.UniversityPrograms)
                    .HasForeignKey(e => e.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Entry Requirement Configuration
            modelBuilder.Entity<EntryRequirement>(entity =>
            {
                entity.HasOne(e => e.UniversityProgram)
                    .WithMany(up => up.EntryRequirements)
                    .HasForeignKey(e => e.UniversityProgramId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region BTEC Program Configuration
            modelBuilder.Entity<BtecProgram>(entity =>
            {
                entity.HasIndex(e => e.UniversityId);
                entity.HasIndex(e => e.IsApprovedByBtecAuthority);

                entity.HasOne(e => e.University)
                    .WithMany(u => u.BtecPrograms)
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region BTEC Entry Requirement Configuration
            modelBuilder.Entity<BtecEntryRequirement>(entity =>
            {
                entity.HasOne(e => e.BtecProgram)
                    .WithMany(bp => bp.EntryRequirements)
                    .HasForeignKey(e => e.BtecProgramId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Recommendation Configuration
            modelBuilder.Entity<Recommendation>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.Score });
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

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Recommendation_OneProgram",
                    "(UniversityProgramId IS NOT NULL AND BtecProgramId IS NULL) OR (UniversityProgramId IS NULL AND BtecProgramId IS NOT NULL)"
                ));
            });
            #endregion

            #region Student Application Configuration
            modelBuilder.Entity<StudentApplication>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.ApplicationDate });
                entity.HasIndex(e => e.Status);

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

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Application_OneProgram",
                    "(UniversityProgramId IS NOT NULL AND BtecProgramId IS NULL) OR (UniversityProgramId IS NULL AND BtecProgramId IS NOT NULL)"
                ));
            });
            #endregion

            #region Discount Grant Configuration
            modelBuilder.Entity<DiscountGrant>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => new { e.UniversityId, e.Status });

                entity.HasOne(e => e.Application)
                    .WithOne(a => a.DiscountGrant)
                    .HasForeignKey<DiscountGrant>(e => e.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.RedeemedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.RedeemedByUserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            #endregion

            #region Commission Configuration
            modelBuilder.Entity<Commission>(entity =>
            {
                entity.HasIndex(e => e.Settled);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.Application)
                    .WithOne(a => a.Commission)
                    .HasForeignKey<Commission>(e => e.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MonthlySettlement)
                    .WithMany(ms => ms.Commissions)
                    .HasForeignKey(e => e.MonthlySettlementId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            #endregion

            #region Monthly Settlement Configuration
            modelBuilder.Entity<MonthlySettlement>(entity =>
            {
                entity.HasIndex(e => new { e.UniversityId, e.Year, e.Month }).IsUnique();
                entity.HasIndex(e => e.Closed);

                entity.HasOne(e => e.University)
                    .WithMany()
                    .HasForeignKey(e => e.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ClosedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ClosedByUserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            #endregion

            #region Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.IsRead });
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region ApplicationUser Configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.IsActive);
            });
            #endregion
        }
        #endregion
    }
}
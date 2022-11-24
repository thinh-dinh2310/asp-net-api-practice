using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
#nullable disable

namespace BusinessObject
{
    public partial class eRecruitmentContext : DbContext
    {
        public eRecruitmentContext()
        {
        }

        public eRecruitmentContext(DbContextOptions<eRecruitmentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicantPost> ApplicantPosts { get; set; }
        public virtual DbSet<AuthToken> AuthTokens { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Interview> Interviews { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LocationPost> LocationPosts { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<PostSkill> PostSkills { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSkill> UserSkills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                               .SetBasePath(Directory.GetCurrentDirectory())
                                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ApplicantPost>(entity =>
            {
                entity.HasKey(e => new { e.ApplicantId, e.PostId, e.Count })
                    .HasName("PK_ApplicantPosts_1");

                entity.Property(e => e.Resume).IsRequired();

                entity.HasOne(d => d.Applicant)
                    .WithMany(p => p.ApplicantPosts)
                    .HasForeignKey(d => d.ApplicantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicantPosts_Users");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.ApplicantPosts)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicantPosts_Posts");
            });

            modelBuilder.Entity<AuthToken>(entity =>
            {
                entity.Property(e => e.AuthTokenId).ValueGeneratedNever();

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AuthTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AuthTokens_Users");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).ValueGeneratedNever();

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Interview>(entity =>
            {
                entity.HasKey(e => new { e.InterviewerId, e.Round, e.PostId, e.ApplicantId });

                entity.Property(e => e.EndDateTime).HasColumnType("datetime");

                entity.Property(e => e.Feedback).IsRequired();

                entity.Property(e => e.StartDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Applicant)
                    .WithMany(p => p.InterviewApplicants)
                    .HasForeignKey(d => d.ApplicantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Interviews_Users2");


                entity.HasOne(d => d.Interviewer)
                    .WithMany(p => p.InterviewInterviewers)
                    .HasForeignKey(d => d.InterviewerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Interviews_Users1");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Interviews)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Interviews_Posts");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.Property(e => e.LevelId).ValueGeneratedNever();

                entity.Property(e => e.LevelName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");

                entity.Property(e => e.LocationId).ValueGeneratedNever();

                entity.Property(e => e.LocationName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LocationPost>(entity =>
            {
                entity.HasKey(e => new { e.LocationId, e.PostId });

                entity.ToTable("LocationPost");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.LocationPosts)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LocationPost_Location");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.LocationPosts)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LocationPost_Posts");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.PostId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.LocationId).HasColumnName("locationId");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Posts_Categories");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Posts_Levels");
            });

            modelBuilder.Entity<PostSkill>(entity =>
            {
                entity.HasKey(e => new { e.PostId, e.SkillsId });

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostSkills)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostSkillsId_Posts");

                entity.HasOne(d => d.Skills)
                    .WithMany(p => p.PostSkills)
                    .HasForeignKey(d => d.SkillsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostSkillsId_Skills");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasKey(e => e.SkillsId);

                entity.Property(e => e.SkillsId).ValueGeneratedNever();

                entity.Property(e => e.SkillName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(90);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(90);

                entity.Property(e => e.DisplayName).HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(90)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Roles");
            });

            modelBuilder.Entity<UserSkill>(entity =>
            {
                entity.HasKey(e => new { e.SkillsId, e.UsersId });

                entity.ToTable("UserSkill");

                entity.HasOne(d => d.Skills)
                    .WithMany(p => p.UserSkills)
                    .HasForeignKey(d => d.SkillsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSkill_Skills");

                entity.HasOne(d => d.Users)
                    .WithMany(p => p.UserSkills)
                    .HasForeignKey(d => d.UsersId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSkill_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

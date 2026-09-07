using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Recommendations.Models;

public partial class RecommendationContext : DbContext
{
    public RecommendationContext()
    {
    }

    public RecommendationContext(DbContextOptions<RecommendationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Compilation> Compilations { get; set; }

    public virtual DbSet<Confirmation> Confirmations { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Experience> Experiences { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCompilation> UserCompilations { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSkill> UserSkills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseFirebird("User=sysdba;password=070507;database=localhost/3050:/databases/Recomendations.fdb;dialect=3;charset=UTF8;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Compilation>(entity =>
        {
            entity.HasKey(e => e.IdCompilation);

            entity.ToTable("COMPILATION");

            entity.HasIndex(e => e.IdCompilation, "PK_COMPILATION").IsUnique();

            entity.Property(e => e.IdCompilation).HasColumnName("ID_COMPILATION");
            entity.Property(e => e.Date)
                .HasColumnType("DATE")
                .HasColumnName("DATE");
            entity.Property(e => e.NameCompilation)
                .HasMaxLength(60)
                .HasColumnName("NAME_COMPILATION");
        });

        modelBuilder.Entity<Confirmation>(entity =>
        {
            entity.HasKey(e => e.IdConfirmation);

            entity.ToTable("CONFIRMATIONS");

            entity.HasIndex(e => e.UserFrom, "FK_CONFIRMATIONS_1");

            entity.HasIndex(e => e.UserTo, "FK_CONFIRMATIONS_2");

            entity.HasIndex(e => e.IdSkill, "FK_CONFIRMATIONS_3");

            entity.HasIndex(e => e.IdConfirmation, "PK_CONFIRMATIONS").IsUnique();

            entity.Property(e => e.IdConfirmation).HasColumnName("ID_CONFIRMATION");
            entity.Property(e => e.Date)
                .HasColumnType("DATE")
                .HasColumnName("DATE");
            entity.Property(e => e.IdSkill).HasColumnName("ID_SKILL");
            entity.Property(e => e.Status)
                .HasMaxLength(60)
                .HasColumnName("STATUS");
            entity.Property(e => e.UserFrom).HasColumnName("USER_FROM");
            entity.Property(e => e.UserTo).HasColumnName("USER_TO");

            entity.HasOne(d => d.IdSkillNavigation).WithMany(p => p.Confirmations)
                .HasForeignKey(d => d.IdSkill)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CONFIRMATIONS_3");

            entity.HasOne(d => d.UserFromNavigation).WithMany(p => p.ConfirmationUserFromNavigations)
                .HasForeignKey(d => d.UserFrom)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CONFIRMATIONS_1");

            entity.HasOne(d => d.UserToNavigation).WithMany(p => p.ConfirmationUserToNavigations)
                .HasForeignKey(d => d.UserTo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CONFIRMATIONS_2");
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.IdEducation);

            entity.ToTable("EDUCATIONS");

            entity.HasIndex(e => e.IdUser, "FK_EDUCATIONS_1");

            entity.HasIndex(e => e.IdEducation, "PK_EDUCATIONS").IsUnique();

            entity.Property(e => e.IdEducation).HasColumnName("ID_EDUCATION");
            entity.Property(e => e.DateEnd)
                .HasColumnType("DATE")
                .HasColumnName("DATE_END");
            entity.Property(e => e.DateStart)
                .HasColumnType("DATE")
                .HasColumnName("DATE_START");
            entity.Property(e => e.IdUser).HasColumnName("ID_USER");
            entity.Property(e => e.NameEducation)
                .HasMaxLength(60)
                .HasColumnName("NAME_EDUCATION");
            entity.Property(e => e.Result)
                .HasMaxLength(60)
                .HasColumnName("RESULT");
            entity.Property(e => e.TypeEductaion)
                .HasMaxLength(60)
                .HasColumnName("TYPE_EDUCTAION");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Educations)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_EDUCATIONS_1");
        });

        modelBuilder.Entity<Experience>(entity =>
        {
            entity.ToTable("EXPERIENCES");

            entity.HasIndex(e => e.IdUser, "FK_EXPERIENCES_1");

            entity.HasIndex(e => e.Id, "PK_EXPERIENCES").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateEnd)
                .HasColumnType("DATE")
                .HasColumnName("DATE_END");
            entity.Property(e => e.DateStart)
                .HasColumnType("DATE")
                .HasColumnName("DATE_START");
            entity.Property(e => e.IdUser).HasColumnName("ID_USER");
            entity.Property(e => e.NameCompany)
                .HasMaxLength(60)
                .HasColumnName("NAME_COMPANY");
            entity.Property(e => e.Post)
                .HasMaxLength(60)
                .HasColumnName("POST");
            entity.Property(e => e.Type)
                .HasMaxLength(60)
                .HasColumnName("TYPE");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Experiences)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_EXPERIENCES_1");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.IdLog);

            entity.ToTable("LOGS");

            entity.HasIndex(e => e.IdLog, "PK_LOGS").IsUnique();

            entity.Property(e => e.IdLog).HasColumnName("ID_LOG");
            entity.Property(e => e.DescriptionLog)
                .HasMaxLength(100)
                .HasColumnName("DESCRIPTION_LOG");
            entity.Property(e => e.LogId).HasColumnName("LOG_ID");
            entity.Property(e => e.TypeLog)
                .HasMaxLength(60)
                .HasColumnName("TYPE_LOG");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.IdNotification);

            entity.ToTable("NOTIFICATIONS");

            entity.HasIndex(e => e.IdUser, "FK_NOTIFICATIONS_1");

            entity.HasIndex(e => e.IdNotification, "PK_NOTIFICATIONS").IsUnique();

            entity.Property(e => e.IdNotification).HasColumnName("ID_NOTIFICATION");
            entity.Property(e => e.Date)
                .HasColumnType("DATE")
                .HasColumnName("DATE");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.IdUser).HasColumnName("ID_USER");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_NOTIFICATIONS_1");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.IdRating);

            entity.ToTable("RATING");

            entity.HasIndex(e => e.UserId, "FK_RATING_1");

            entity.HasIndex(e => e.IdSkill, "FK_RATING_2");

            entity.HasIndex(e => e.IdRating, "PK_RATING").IsUnique();

            entity.Property(e => e.IdRating).HasColumnName("ID_RATING");
            entity.Property(e => e.DateRating)
                .HasColumnType("DATE")
                .HasColumnName("DATE_rating");
            entity.Property(e => e.IdSkill).HasColumnName("ID_SKILL");
            entity.Property(e => e.LevelRating).HasColumnName("LEVEL_rating");
            entity.Property(e => e.TypeRating)
                .HasMaxLength(50)
                .HasColumnName("TYPE_RATING");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.IdSkillNavigation).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.IdSkill)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_RATING_2");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_RATING_1");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.IdSkill);

            entity.ToTable("SKILLS");

            entity.HasIndex(e => e.IdSkill, "PK_SKILLS").IsUnique();

            entity.Property(e => e.IdSkill).HasColumnName("ID_SKILL");
            entity.Property(e => e.NameSkill)
                .HasMaxLength(60)
                .HasColumnName("NAME_SKILL");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("USERS");

            entity.HasIndex(e => e.IdUser, "PK_USERS").IsUnique();

            entity.HasIndex(e => e.EmailUser, "UQ_USERS_1").IsUnique();

            entity.HasIndex(e => e.PhoneUser, "UQ_USERS_2").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("ID_USER");
            entity.Property(e => e.DateCreate)
                .HasColumnType("DATE")
                .HasColumnName("DATE_CREATE");
            entity.Property(e => e.EmailUser)
                .HasMaxLength(60)
                .HasColumnName("EMAIL_USER");
            entity.Property(e => e.NameUser)
                .HasMaxLength(60)
                .HasColumnName("NAME_USER");
            entity.Property(e => e.PasswordUser)
                .HasMaxLength(40)
                .HasColumnName("PASSWORD_USER");
            entity.Property(e => e.PatronymicUser)
                .HasMaxLength(60)
                .HasColumnName("PATRONYMIC_USER");
            entity.Property(e => e.PhoneUser)
                .HasMaxLength(20)
                .HasColumnName("PHONE_USER");
            entity.Property(e => e.Pin)
                .HasMaxLength(1)
                .HasColumnName("PIN");
            entity.Property(e => e.RoleUser)
                .HasMaxLength(20)
                .HasColumnName("ROLE_USER");
            entity.Property(e => e.SurnameUser)
                .HasMaxLength(60)
                .HasColumnName("SURNAME_USER");
        });

        modelBuilder.Entity<UserCompilation>(entity =>
        {
            entity.HasKey(e => e.IdUserCompilation);

            entity.ToTable("USER_COMPILATION");

            entity.HasIndex(e => e.IdUser, "FK_USER_COMPILATION_1");

            entity.HasIndex(e => e.IdCompilation, "FK_USER_COMPILATION_2");

            entity.HasIndex(e => e.IdUserCompilation, "PK_USER_COMPILATION").IsUnique();

            entity.Property(e => e.IdUserCompilation).HasColumnName("ID_USER_COMPILATION");
            entity.Property(e => e.IdCompilation).HasColumnName("ID_COMPILATION");
            entity.Property(e => e.IdUser).HasColumnName("ID_USER");

            entity.HasOne(d => d.IdCompilationNavigation).WithMany(p => p.UserCompilations)
                .HasForeignKey(d => d.IdCompilation)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_USER_COMPILATION_2");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserCompilations)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_USER_COMPILATION_1");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.IdUserRole);

            entity.ToTable("USER_ROLES");

            entity.HasIndex(e => e.IdUser, "FK_USER_ROLES_1");

            entity.HasIndex(e => e.IdUserRole, "PK_USER_ROLES").IsUnique();

            entity.Property(e => e.IdUserRole).HasColumnName("ID_USER_ROLE");
            entity.Property(e => e.IdUser).HasColumnName("ID_USER");
            entity.Property(e => e.NameRoleAfter)
                .HasMaxLength(60)
                .HasColumnName("NAME_ROLE_AFTER");
            entity.Property(e => e.NameRoleBefore)
                .HasMaxLength(60)
                .HasColumnName("NAME_ROLE_BEFORE");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_USER_ROLES_1");
        });

        modelBuilder.Entity<UserSkill>(entity =>
        {
            entity.ToTable("USER_SKILLS");

            entity.HasIndex(e => e.IdSkill, "FK_USER_SKILLS_1");

            entity.HasIndex(e => e.UserId, "FK_USER_SKILLS_2");

            entity.HasIndex(e => e.Id, "PK_USER_SKILLS").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdSkill).HasColumnName("ID_SKILL");
            entity.Property(e => e.MarkSkill).HasColumnName("MARK_SKILL");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("STATUS");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.IdSkillNavigation).WithMany(p => p.UserSkills)
                .HasForeignKey(d => d.IdSkill)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_USER_SKILLS_1");

            entity.HasOne(d => d.User).WithMany(p => p.UserSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_USER_SKILLS_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

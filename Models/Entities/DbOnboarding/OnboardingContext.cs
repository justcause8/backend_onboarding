using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Models.Entities.DbOnboarding;

public partial class OnboardingContext : DbContext
{
    public OnboardingContext()
    {
    }

    public OnboardingContext(DbContextOptions<OnboardingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<AnswerOption> AnswerOptions { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<OnboardingRoute> OnboardingRoutes { get; set; }

    public virtual DbSet<OnboardingStage> OnboardingStages { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<QuestionType> QuestionTypes { get; set; }

    public virtual DbSet<SystemAudit> SystemAudits { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserOnboardingRouteStatus> UserOnboardingRouteStatuses { get; set; }

    public virtual DbSet<UserOnboardingStageStatus> UserOnboardingStageStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Onboarding;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answer__3213E83FDBD61B3E");

            entity.ToTable("Answer");

            entity.HasIndex(e => e.Id, "UQ__Answer__3213E83EB62F1E2C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerText)
                .HasMaxLength(255)
                .HasColumnName("answer_text");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.FkQuestionId).HasColumnName("FK_question_id");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");

            entity.HasOne(d => d.FkQuestion).WithMany(p => p.Answers)
                .HasForeignKey(d => d.FkQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answer__FK_quest__571DF1D5");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Answers)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answer__FK_user___4F7CD00D");
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AnswerOp__3213E83F51C7C4B6");

            entity.ToTable("AnswerOption");

            entity.HasIndex(e => e.Id, "UQ__AnswerOp__3213E83EABC40A56").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkAnswerId).HasColumnName("FK_answer_id");
            entity.Property(e => e.SelectedAnswerOption).HasColumnName("selected_answer_option");

            entity.HasOne(d => d.FkAnswer).WithMany(p => p.AnswerOptions)
                .HasForeignKey(d => d.FkAnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AnswerOpt__FK_an__5070F446");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3213E83FECD01A09");

            entity.ToTable("Course");

            entity.HasIndex(e => e.Id, "UQ__Course__3213E83EE7A87961").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(4000)
                .HasColumnName("description");
            entity.Property(e => e.FkOnboardingStage).HasColumnName("FK_onboarding_stage");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.FkOnboardingStageNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.FkOnboardingStage)
                .HasConstraintName("FK__Course__FK_onboa__52593CB8");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Courses)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK__Course__FK_user___5165187F");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3213E83FCE4B5AFE");

            entity.HasIndex(e => e.Id, "UQ__Material__3213E83E0E206286").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkCourseId).HasColumnName("FK_course_id");
            entity.Property(e => e.UrlDocument)
                .HasMaxLength(255)
                .HasColumnName("url_document");

            entity.HasOne(d => d.FkCourse).WithMany(p => p.Materials)
                .HasForeignKey(d => d.FkCourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Materials__FK_co__534D60F1");
        });

        modelBuilder.Entity<OnboardingRoute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Onboardi__3213E83F3ED25E87");

            entity.ToTable("OnboardingRoute");

            entity.HasIndex(e => e.Id, "UQ__Onboardi__3213E83E83321C51").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<OnboardingStage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Onboardi__3213E83F82B6D63E");

            entity.ToTable("OnboardingStage");

            entity.HasIndex(e => e.Id, "UQ__Onboardi__3213E83E8DD7B92B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FkOnboardingRouteId).HasColumnName("FK_onboarding_route_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.FkOnboardingRoute).WithMany(p => p.OnboardingStages)
                .HasForeignKey(d => d.FkOnboardingRouteId)
                .HasConstraintName("FK__Onboardin__FK_on__4E88ABD4");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83FB60F4979");

            entity.ToTable("Question");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83E610333A0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkQuestionTypeId).HasColumnName("FK_question_type_id");
            entity.Property(e => e.FkTestId).HasColumnName("FK_test_id");
            entity.Property(e => e.TextQuestion)
                .HasMaxLength(255)
                .HasColumnName("text_question");

            entity.HasOne(d => d.FkQuestionType).WithMany(p => p.Questions)
                .HasForeignKey(d => d.FkQuestionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__FK_que__5535A963");

            entity.HasOne(d => d.FkTest).WithMany(p => p.Questions)
                .HasForeignKey(d => d.FkTestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__FK_tes__5441852A");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83F2F602663");

            entity.ToTable("QuestionOption");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83E59CD35AB").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer");
            entity.Property(e => e.FkQuestionId).HasColumnName("FK_question_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Text)
                .HasMaxLength(255)
                .HasColumnName("text");

            entity.HasOne(d => d.FkQuestion).WithMany(p => p.QuestionOptions)
                .HasForeignKey(d => d.FkQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuestionO__FK_qu__5629CD9C");
        });

        modelBuilder.Entity<QuestionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83FB357F3E3");

            entity.ToTable("QuestionType");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83E0AFC07A5").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameQuestionType)
                .HasMaxLength(255)
                .HasColumnName("name_question_type");
        });

        modelBuilder.Entity<SystemAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemAu__3213E83F5DC5473E");

            entity.ToTable("SystemAudit");

            entity.HasIndex(e => e.Id, "UQ__SystemAu__3213E83EE8D8A36F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .HasColumnName("action");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.TimeInterval).HasColumnName("time_interval");

            entity.HasOne(d => d.FkUser).WithMany(p => p.SystemAudits)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK__SystemAud__FK_us__4D94879B");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Test__3213E83F6E1A2906");

            entity.ToTable("Test");

            entity.HasIndex(e => e.Id, "UQ__Test__3213E83E2FE0A866").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FkCourseId).HasColumnName("FK_course_id");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.PassingScore)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("passing_score");
            entity.Property(e => e.ResultsScore)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("results_score");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.FkCourse).WithMany(p => p.Tests)
                .HasForeignKey(d => d.FkCourseId)
                .HasConstraintName("FK__Test__FK_course___4CA06362");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Tests)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Test__FK_user_id__4BAC3F29");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83FA2001890");

            entity.ToTable("User");

            entity.HasIndex(e => e.Uid, "UQ__User__DD7012656F00FBA9").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(255)
                .HasColumnName("job_title");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.Uid).HasColumnName("uid");
        });

        modelBuilder.Entity<UserOnboardingRouteStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserOnbo__3213E83FDDA0AA45");

            entity.ToTable("UserOnboardingRouteStatus");

            entity.HasIndex(e => e.Id, "UQ__UserOnbo__3213E83E50E779ED").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FactEndDate).HasColumnName("fact_end_date");
            entity.Property(e => e.FactStartDate).HasColumnName("fact_start_date");
            entity.Property(e => e.FkOnboardingRouteId).HasColumnName("FK_onboarding_route_id");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.FkOnboardingRoute).WithMany(p => p.UserOnboardingRouteStatuses)
                .HasForeignKey(d => d.FkOnboardingRouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOnboa__FK_on__59063A47");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserOnboardingRouteStatuses)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOnboa__FK_us__5812160E");
        });

        modelBuilder.Entity<UserOnboardingStageStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserOnbo__3213E83F6281821B");

            entity.ToTable("UserOnboardingStageStatus");

            entity.HasIndex(e => e.Id, "UQ__UserOnbo__3213E83E55D639AE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FactEndDate).HasColumnName("fact_end_date");
            entity.Property(e => e.FactStartDate).HasColumnName("fact_start_date");
            entity.Property(e => e.FkOnboardingStageId).HasColumnName("FK_onboarding_stage_id");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.FkOnboardingStage).WithMany(p => p.UserOnboardingStageStatuses)
                .HasForeignKey(d => d.FkOnboardingStageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOnboa__FK_on__5AEE82B9");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserOnboardingStageStatuses)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOnboa__FK_us__59FA5E80");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

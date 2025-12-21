using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Models.Entitie.DbOnboarding;

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

    public virtual DbSet<UserProgress> UserProgresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=OnboardingConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answer__3213E83F53FC2B61");

            entity.ToTable("Answer");

            entity.HasIndex(e => e.Id, "UQ__Answer__3213E83EEF94F9B7").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerText)
                .HasMaxLength(255)
                .HasColumnName("answer_text");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Fk1UserId).HasColumnName("FK1_user_id");
            entity.Property(e => e.Fk2QuestionId).HasColumnName("FK2_question_id");

            entity.HasOne(d => d.Fk1User).WithMany(p => p.Answers)
                .HasForeignKey(d => d.Fk1UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answer__FK1_user__5441852A");

            entity.HasOne(d => d.Fk2Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.Fk2QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answer__FK2_ques__59063A47");
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AnswerOp__3213E83F179B8832");

            entity.ToTable("AnswerOption");

            entity.HasIndex(e => e.Id, "UQ__AnswerOp__3213E83E55CD946B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkAnswerId).HasColumnName("FK_answer_id");
            entity.Property(e => e.SelectedAnswerOption).HasColumnName("selected_answer_option");

            entity.HasOne(d => d.FkAnswer).WithMany(p => p.AnswerOptions)
                .HasForeignKey(d => d.FkAnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AnswerOpt__FK_an__5535A963");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3213E83F9D3C4BEA");

            entity.ToTable("Course");

            entity.HasIndex(e => e.Id, "UQ__Course__3213E83E538825E6").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(4000)
                .HasColumnName("description");
            entity.Property(e => e.FkOnbordingStage).HasColumnName("FK_onbording_stage");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.FkOnbordingStageNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.FkOnbordingStage)
                .HasConstraintName("FK__Course__FK_onbor__4F7CD00D");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3213E83F38951C91");

            entity.HasIndex(e => e.Id, "UQ__Material__3213E83EF6F831B5").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkCourseId).HasColumnName("FK_course_id");
            entity.Property(e => e.UrlDocument)
                .HasMaxLength(255)
                .HasColumnName("url_document");

            entity.HasOne(d => d.FkCourse).WithMany(p => p.Materials)
                .HasForeignKey(d => d.FkCourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Materials__FK_co__4E88ABD4");
        });

        modelBuilder.Entity<OnboardingRoute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Onboardi__3213E83FCFA73BD6");

            entity.ToTable("OnboardingRoute");

            entity.HasIndex(e => e.Id, "UQ__Onboardi__3213E83EAB2520CA").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.FkUser).WithMany(p => p.OnboardingRoutes)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK__Onboardin__FK_us__5DCAEF64");
        });

        modelBuilder.Entity<OnboardingStage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Onboardi__3213E83FC25D62D4");

            entity.ToTable("OnboardingStage");

            entity.HasIndex(e => e.Id, "UQ__Onboardi__3213E83EE4366BBB").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FkOnbordingRouteId).HasColumnName("FK_onbording_route_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.FkOnbordingRoute).WithMany(p => p.OnboardingStages)
                .HasForeignKey(d => d.FkOnbordingRouteId)
                .HasConstraintName("FK__Onboardin__FK_on__534D60F1");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83FF83BCA78");

            entity.ToTable("Question");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83EBE161D98").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkQuestionTypeId).HasColumnName("FK_question_type_id");
            entity.Property(e => e.FkTestId).HasColumnName("FK_test_id");
            entity.Property(e => e.TextQuestion)
                .HasMaxLength(255)
                .HasColumnName("text_question");

            entity.HasOne(d => d.FkQuestionType).WithMany(p => p.Questions)
                .HasForeignKey(d => d.FkQuestionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__FK_que__571DF1D5");

            entity.HasOne(d => d.FkTest).WithMany(p => p.Questions)
                .HasForeignKey(d => d.FkTestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__FK_tes__5629CD9C");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83FE8657653");

            entity.ToTable("QuestionOption");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83E10B61D7A").IsUnique();

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
                .HasConstraintName("FK__QuestionO__FK_qu__5812160E");
        });

        modelBuilder.Entity<QuestionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83FB21100C6");

            entity.ToTable("QuestionType");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83E6AEB396F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameQuestionType)
                .HasMaxLength(255)
                .HasColumnName("name_question_type");
        });

        modelBuilder.Entity<SystemAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemAu__3213E83F2021685D");

            entity.ToTable("SystemAudit");

            entity.HasIndex(e => e.Id, "UQ__SystemAu__3213E83E6AFB03E3").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .HasColumnName("action");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.TimeInterval).HasColumnName("time_interval");

            entity.HasOne(d => d.FkUser).WithMany(p => p.SystemAudits)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SystemAud__FK_us__52593CB8");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Test__3213E83F876C692E");

            entity.ToTable("Test");

            entity.HasIndex(e => e.Id, "UQ__Test__3213E83E9DCF0291").IsUnique();

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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Test__FK_course___5070F446");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Tests)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Test__FK_user_id__5165187F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83F205F7DF4");

            entity.ToTable("User");

            entity.HasIndex(e => e.Uid, "UQ__User__DD701265CF7949A3").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__UserOnbo__3213E83F18CE3741");

            entity.ToTable("UserOnboardingRouteStatus");

            entity.HasIndex(e => e.Id, "UQ__UserOnbo__3213E83E60D4B7BA").IsUnique();

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
                .HasConstraintName("FK__UserOnboa__FK_on__5BE2A6F2");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserOnboardingRouteStatuses)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOnboa__FK_us__5AEE82B9");
        });

        modelBuilder.Entity<UserOnboardingStageStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserOnbo__3213E83F6A9C7602");

            entity.ToTable("UserOnboardingStageStatus");

            entity.HasIndex(e => e.Id, "UQ__UserOnbo__3213E83E49844A6F").IsUnique();

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
                .HasConstraintName("FK__UserOnboa__FK_on__5CD6CB2B");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserOnboardingStageStatuses)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOnboa__FK_us__59FA5E80");
        });

        modelBuilder.Entity<UserProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserProg__3213E83FECA43AE4");

            entity.ToTable("UserProgress");

            entity.HasIndex(e => e.Id, "UQ__UserProg__3213E83E4C2F2729").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkCourseId).HasColumnName("FK_course_id");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.FkCourse).WithMany(p => p.UserProgresses)
                .HasForeignKey(d => d.FkCourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserProgr__FK_co__5FB337D6");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserProgresses)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserProgr__FK_us__5EBF139D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

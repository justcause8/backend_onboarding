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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Onboarding;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answer__3213E83F4C9CF4BA");

            entity.ToTable("Answer");

            entity.HasIndex(e => e.Id, "UQ__Answer__3213E83E8709298A").IsUnique();

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
                .HasConstraintName("FK__Answer__FK1_user__4F7CD00D");

            entity.HasOne(d => d.Fk2Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.Fk2QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answer__FK2_ques__571DF1D5");
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AnswerOp__3213E83F3C851CCD");

            entity.ToTable("AnswerOption");

            entity.HasIndex(e => e.Id, "UQ__AnswerOp__3213E83EEA3855D3").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Course__3213E83FD93EE78F");

            entity.ToTable("Course");

            entity.HasIndex(e => e.Id, "UQ__Course__3213E83EF878957E").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(4000)
                .HasColumnName("description");
            entity.Property(e => e.Fk1UserId).HasColumnName("FK1_user_id");
            entity.Property(e => e.Fk2OnbordingStage).HasColumnName("FK2_onbording_stage");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Fk1User).WithMany(p => p.Courses)
                .HasForeignKey(d => d.Fk1UserId)
                .HasConstraintName("FK__Course__FK1_user__5165187F");

            entity.HasOne(d => d.Fk2OnbordingStageNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.Fk2OnbordingStage)
                .HasConstraintName("FK__Course__FK2_onbo__52593CB8");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3213E83F856B31D6");

            entity.HasIndex(e => e.Id, "UQ__Material__3213E83E0DFCF45C").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Onboardi__3213E83F5650B787");

            entity.ToTable("OnboardingRoute");

            entity.HasIndex(e => e.Id, "UQ__Onboardi__3213E83E269133A9").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FkmentorId).HasColumnName("FKMentorId");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Fkmentor).WithMany(p => p.OnboardingRoutes)
                .HasForeignKey(d => d.FkmentorId)
                .HasConstraintName("FK__Onboardin__FKMen__5BE2A6F2");
        });

        modelBuilder.Entity<OnboardingStage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Onboardi__3213E83F7E42CF58");

            entity.ToTable("OnboardingStage");

            entity.HasIndex(e => e.Id, "UQ__Onboardi__3213E83EAD5F774E").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Fk1OnbordingRouteId).HasColumnName("FK1_onbording_route_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Fk1OnbordingRoute).WithMany(p => p.OnboardingStages)
                .HasForeignKey(d => d.Fk1OnbordingRouteId)
                .HasConstraintName("FK__Onboardin__FK1_o__4E88ABD4");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83F38EC08BD");

            entity.ToTable("Question");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83E85627E1D").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fk1TestId).HasColumnName("FK1_test_id");
            entity.Property(e => e.Fk2QuestionTypeId).HasColumnName("FK2_question_type_id");
            entity.Property(e => e.TextQuestion)
                .HasMaxLength(255)
                .HasColumnName("text_question");

            entity.HasOne(d => d.Fk1Test).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Fk1TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__FK1_te__5441852A");

            entity.HasOne(d => d.Fk2QuestionType).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Fk2QuestionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__FK2_qu__5535A963");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83FFD19C1A3");

            entity.ToTable("QuestionOption");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83E39DC8B90").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83F91D90170");

            entity.ToTable("QuestionType");

            entity.HasIndex(e => e.Id, "UQ__Question__3213E83ECBF75F71").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameQuestionType)
                .HasMaxLength(255)
                .HasColumnName("name_question_type");
        });

        modelBuilder.Entity<SystemAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemAu__3213E83FED5351D5");

            entity.ToTable("SystemAudit");

            entity.HasIndex(e => e.Id, "UQ__SystemAu__3213E83E1840AE4A").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Test__3213E83FB11E317E");

            entity.ToTable("Test");

            entity.HasIndex(e => e.Id, "UQ__Test__3213E83E56E5FEC6").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Fk1CourseId).HasColumnName("FK1_course_id");
            entity.Property(e => e.Fk2UserId).HasColumnName("FK2_user_id");
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

            entity.HasOne(d => d.Fk1Course).WithMany(p => p.Tests)
                .HasForeignKey(d => d.Fk1CourseId)
                .HasConstraintName("FK__Test__FK1_course__4CA06362");

            entity.HasOne(d => d.Fk2User).WithMany(p => p.Tests)
                .HasForeignKey(d => d.Fk2UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Test__FK2_user_i__4BAC3F29");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83F958B2878");

            entity.ToTable("User");

            entity.HasIndex(e => e.Uid, "UQ__User__DD701265FCE90D60").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__UserOnbo__3213E83F4B8EA645");

            entity.ToTable("UserOnboardingRouteStatus");

            entity.HasIndex(e => e.Id, "UQ__UserOnbo__3213E83EC99EF78C").IsUnique();

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
                .HasConstraintName("FK__UserOnboa__FK_on__59FA5E80");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserOnboardingRouteStatuses)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOnboa__FK_us__59063A47");
        });

        modelBuilder.Entity<UserOnboardingStageStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserOnbo__3213E83F3F974232");

            entity.ToTable("UserOnboardingStageStatus");

            entity.HasIndex(e => e.Id, "UQ__UserOnbo__3213E83EFF6EF427").IsUnique();

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
                .HasConstraintName("FK__UserOnboa__FK_us__5812160E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

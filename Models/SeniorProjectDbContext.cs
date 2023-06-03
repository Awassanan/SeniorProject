using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SeniorProject.Models
{
    public partial class SeniorProjectDbContext : DbContext
    {
        public SeniorProjectDbContext()
        {
        }

        public SeniorProjectDbContext(DbContextOptions<SeniorProjectDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Faq> Faq { get; set; } = null!;
        public virtual DbSet<Lecturer> Lecturer { get; set; } = null!;
        public virtual DbSet<Project> Project { get; set; } = null!;
        public virtual DbSet<ProjectAssignment> ProjectAssignment { get; set; } = null!;
        public virtual DbSet<ProjectDownload> ProjectDownload { get; set; } = null!;
        public virtual DbSet<ProjectTimeline> ProjectTimeline { get; set; } = null!;
        public virtual DbSet<ProjectUpload> ProjectUpload { get; set; } = null!;
        public virtual DbSet<Proposal> Proposal { get; set; } = null!;
        public virtual DbSet<ProposalAssignment> ProposalAssignment { get; set; } = null!;
        public virtual DbSet<ProposalDownload> ProposalDownload { get; set; } = null!;
        public virtual DbSet<ProposalTimeline> ProposalTimeline { get; set; } = null!;
        public virtual DbSet<ProposalUpload> ProposalUpload { get; set; } = null!;
        public virtual DbSet<Semester> Semester { get; set; } = null!;
        public virtual DbSet<Student> Student { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=53276464;database=SeniorProject", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.8-mariadb")); // test cloud math comp
                // optionsBuilder.UseMySql("server=localhost;port=3306;user=dev;password=12345678;database=SeniorProject", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.8-mariadb")); // test localhost or cache111
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Faq>(entity =>
            {
                entity.ToTable("FAQ");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Answer)
                    .HasMaxLength(2000)
                    .HasDefaultValueSql("''")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Question)
                    .HasMaxLength(2000)
                    .HasDefaultValueSql("''")
                    .UseCollation("utf8mb4_thai_520_w2");
            });

            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.HasIndex(e => e.Email, "Email")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Hint)
                    .HasMaxLength(200)
                    .HasComment("คำใบ้สำหรับ keyword กรณีลืมรหัสและพิมพ์ keyword ผิดเกิน 3 ครั้ง")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Keyword)
                    .HasMaxLength(200)
                    .HasComment("keyword ยืนยันตัวตนตอนเปลี่ยน/ลืมรหัสผ่าน")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Password)
                    .HasMaxLength(44)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .HasComment("xxx-xxx-xxxx")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.ProfilePicture)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Salt)
                    .HasMaxLength(24)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Title)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(e => e.CommitteeId1, "CommitteeId1");

                entity.HasIndex(e => e.AdvisorId1, "FK1_Project_Teacher");

                entity.HasIndex(e => e.AdvisorId2, "FK2_Project_Teacher");

                entity.HasIndex(e => e.CommitteeId2, "FK4_Project_Teacher");

                entity.HasIndex(e => e.StudentId1, "FK_Project_Student");

                entity.HasIndex(e => e.StudentId2, "FK_Project_Student_2");

                entity.HasIndex(e => e.StudentId3, "FK_Project_Student_3");

                entity.HasIndex(e => e.SemesterId, "FK_Proposal_Semester");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Advisor1UploadFile)
                    .HasMaxLength(50)
                    .HasComment("เก็บชื่อไฟล์ ให้อาจารย์ส่งใบรายงานผลสอบหลังกรอกคะแนนแล้ว (All)")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Advisor2UploadFile)
                    .HasMaxLength(50)
                    .HasComment("COMP")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.AdvisorId1).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AdvisorId2).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Committee1UploadFile)
                    .HasMaxLength(50)
                    .HasComment("COMP")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Committee2UploadFile)
                    .HasMaxLength(50)
                    .HasComment("COMP")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.CommitteeId1).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CommitteeId2).HasColumnType("int(10) unsigned");

                entity.Property(e => e.GradeStudent1).HasColumnType("enum('W','I','F','D','D+','C','C+','B','B+','A')");

                entity.Property(e => e.GradeStudent2).HasColumnType("enum('W','I','F','D','D+','C','C+','B','B+','A')");

                entity.Property(e => e.GradeStudent3).HasColumnType("enum('W','I','F','D','D+','C','C+','B','B+','A')");

                entity.Property(e => e.Major)
                    .HasColumnType("enum('MATH','COMP')")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.No).HasColumnType("int(10) unsigned");

                entity.Property(e => e.ProjectNameEn)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.ProjectNameTh)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.SemesterId)
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.StudentId1)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.StudentId2)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.StudentId3)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.HasOne(d => d.AdvisorId1Navigation)
                    .WithMany(p => p.ProjectAdvisorId1Navigation)
                    .HasForeignKey(d => d.AdvisorId1)
                    .HasConstraintName("FK_Project_Lecturer");

                entity.HasOne(d => d.AdvisorId2Navigation)
                    .WithMany(p => p.ProjectAdvisorId2Navigation)
                    .HasForeignKey(d => d.AdvisorId2)
                    .HasConstraintName("FK_Project_Lecturer_2");

                entity.HasOne(d => d.CommitteeId1Navigation)
                    .WithMany(p => p.ProjectCommitteeId1Navigation)
                    .HasForeignKey(d => d.CommitteeId1)
                    .HasConstraintName("FK_Project_Lecturer_3");

                entity.HasOne(d => d.CommitteeId2Navigation)
                    .WithMany(p => p.ProjectCommitteeId2Navigation)
                    .HasForeignKey(d => d.CommitteeId2)
                    .HasConstraintName("FK_Project_Lecturer_4");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.SemesterId)
                    .HasConstraintName("project_ibfk_8");

                entity.HasOne(d => d.StudentId1Navigation)
                    .WithMany(p => p.ProjectStudentId1Navigation)
                    .HasForeignKey(d => d.StudentId1)
                    .HasConstraintName("project_ibfk_5");

                entity.HasOne(d => d.StudentId2Navigation)
                    .WithMany(p => p.ProjectStudentId2Navigation)
                    .HasForeignKey(d => d.StudentId2)
                    .HasConstraintName("project_ibfk_6");

                entity.HasOne(d => d.StudentId3Navigation)
                    .WithMany(p => p.ProjectStudentId3Navigation)
                    .HasForeignKey(d => d.StudentId3)
                    .HasConstraintName("project_ibfk_7");
            });

            modelBuilder.Entity<ProjectAssignment>(entity =>
            {
                entity.HasIndex(e => e.SemesterId, "FK_ProjectAssignment_Semester");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssignmentName)
                    .HasMaxLength(200)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Deadline).HasColumnType("datetime");

                entity.Property(e => e.FileType).HasMaxLength(100);

                entity.Property(e => e.MaxSize)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValueSql("'200'")
                    .HasComment("หน่วยเป็น byte (ส่วนมาก MB)");

                entity.Property(e => e.SaveName)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.SemesterId).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.ProjectAssignment)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectAssignment_Semester");
            });

            modelBuilder.Entity<ProjectDownload>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.DownloadLink)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.FileName)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.FileSize)
                    .HasColumnType("smallint(6)")
                    .HasComment("หน่วยเป็น byte (ส่วนมาก kB)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectTimeline>(entity =>
            {
                entity.HasIndex(e => e.SemesterId, "FK_ProjectTimeline_Semester");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.SemesterId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.ToDo)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.ProjectTimeline)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectTimeline_Semester");
            });

            modelBuilder.Entity<ProjectUpload>(entity =>
            {
                entity.HasIndex(e => e.AssignmentId, "FK1_AssignmentStatus_Project");

                entity.HasIndex(e => e.ProjectId, "FK2_AssignmentStatus_Project");

                entity.HasIndex(e => e.StudentId, "FK3_AssignmentStatus_Project");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssignmentId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.FileName)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.FileType)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.ProjectId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.StudentId)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.SubmitDate).HasColumnType("datetime");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.ProjectUpload)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectUpload_ProjectAssignment");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectUpload)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectUpload_Project");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.ProjectUpload)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("projectupload_ibfk_3");
            });

            modelBuilder.Entity<Proposal>(entity =>
            {
                entity.HasIndex(e => e.CommitteeId1, "CommitteeId1");

                entity.HasIndex(e => e.AdvisorId1, "FK1_Project_Teacher");

                entity.HasIndex(e => e.AdvisorId2, "FK2_Project_Teacher");

                entity.HasIndex(e => e.CommitteeId2, "FK4_Project_Teacher");

                entity.HasIndex(e => e.StudentId1, "FK_Project_Student");

                entity.HasIndex(e => e.StudentId2, "FK_Project_Student_2");

                entity.HasIndex(e => e.StudentId3, "FK_Project_Student_3");

                entity.HasIndex(e => e.SemesterId, "FK_Proposal_Semester");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Advisor1UploadFile)
                    .HasMaxLength(50)
                    .HasComment("เก็บชื่อไฟล์ ให้อาจารย์ส่งใบรายงานผลสอบหลังกรอกคะแนนแล้ว (All)")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Advisor2UploadFile)
                    .HasMaxLength(50)
                    .HasComment("COMP")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.AdvisorId1).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AdvisorId2).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Committee1UploadFile)
                    .HasMaxLength(50)
                    .HasComment("COMP")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Committee2UploadFile)
                    .HasMaxLength(50)
                    .HasComment("COMP")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.CommitteeId1).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CommitteeId2).HasColumnType("int(10) unsigned");

                entity.Property(e => e.GradeStudent1).HasColumnType("enum('W','I','F','D','D+','C','C+','B','B+','A')");

                entity.Property(e => e.GradeStudent2).HasColumnType("enum('W','I','F','D','D+','C','C+','B','B+','A')");

                entity.Property(e => e.GradeStudent3).HasColumnType("enum('W','I','F','D','D+','C','C+','B','B+','A')");

                entity.Property(e => e.Major)
                    .HasColumnType("enum('MATH','COMP')")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.No).HasColumnType("int(10) unsigned");

                entity.Property(e => e.ProjectNameEn)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.ProjectNameTh)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.SemesterId)
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.StudentId1)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.StudentId2)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.StudentId3)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.HasOne(d => d.AdvisorId1Navigation)
                    .WithMany(p => p.ProposalAdvisorId1Navigation)
                    .HasForeignKey(d => d.AdvisorId1)
                    .HasConstraintName("FK_Proposal_Lecturer");

                entity.HasOne(d => d.AdvisorId2Navigation)
                    .WithMany(p => p.ProposalAdvisorId2Navigation)
                    .HasForeignKey(d => d.AdvisorId2)
                    .HasConstraintName("FK_Proposal_Lecturer_2");

                entity.HasOne(d => d.CommitteeId1Navigation)
                    .WithMany(p => p.ProposalCommitteeId1Navigation)
                    .HasForeignKey(d => d.CommitteeId1)
                    .HasConstraintName("FK_Proposal_Lecturer_3");

                entity.HasOne(d => d.CommitteeId2Navigation)
                    .WithMany(p => p.ProposalCommitteeId2Navigation)
                    .HasForeignKey(d => d.CommitteeId2)
                    .HasConstraintName("FK_Proposal_Lecturer_4");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Proposal)
                    .HasForeignKey(d => d.SemesterId)
                    .HasConstraintName("FK_Proposal_Semester");

                entity.HasOne(d => d.StudentId1Navigation)
                    .WithMany(p => p.ProposalStudentId1Navigation)
                    .HasForeignKey(d => d.StudentId1)
                    .HasConstraintName("FK_Project_Student");

                entity.HasOne(d => d.StudentId2Navigation)
                    .WithMany(p => p.ProposalStudentId2Navigation)
                    .HasForeignKey(d => d.StudentId2)
                    .HasConstraintName("FK_Project_Student_2");

                entity.HasOne(d => d.StudentId3Navigation)
                    .WithMany(p => p.ProposalStudentId3Navigation)
                    .HasForeignKey(d => d.StudentId3)
                    .HasConstraintName("FK_Project_Student_3");
            });

            modelBuilder.Entity<ProposalAssignment>(entity =>
            {
                entity.HasIndex(e => e.SemesterId, "FK_ProposalAssignment_Semester");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssignmentName)
                    .HasMaxLength(200)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Deadline).HasColumnType("datetime");

                entity.Property(e => e.FileType).HasMaxLength(100);

                entity.Property(e => e.MaxSize)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValueSql("'200'")
                    .HasComment("หน่วยเป็น byte (ส่วนมาก MB)");

                entity.Property(e => e.SaveName)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.SemesterId).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.ProposalAssignment)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProposalAssignment_Semester");
            });

            modelBuilder.Entity<ProposalDownload>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.DownloadLink)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.FileName)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.FileSize)
                    .HasColumnType("smallint(6)")
                    .HasComment("หน่วยเป็น byte (ส่วนมาก kB)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProposalTimeline>(entity =>
            {
                entity.HasIndex(e => e.SemesterId, "FK_ProposalTimeline_Semester");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.SemesterId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.ToDo)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.ProposalTimeline)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProposalTimeline_Semester");
            });

            modelBuilder.Entity<ProposalUpload>(entity =>
            {
                entity.HasIndex(e => e.AssignmentId, "FK1_AssignmentStatus_Project");

                entity.HasIndex(e => e.ProposalId, "FK2_AssignmentStatus_Project");

                entity.HasIndex(e => e.StudentId, "FK3_AssignmentStatus_Project");

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssignmentId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.FileName)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.FileType)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.ProposalId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.StudentId)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.SubmitDate).HasColumnType("datetime");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.ProposalUpload)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProposalAssignmentStatus_ProposalAssignment");

                entity.HasOne(d => d.Proposal)
                    .WithMany(p => p.ProposalUpload)
                    .HasForeignKey(d => d.ProposalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProposalUpload_Proposal");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.ProposalUpload)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK3_AssignmentStatus_Project");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11) unsigned");

                entity.Property(e => e.AcademicYear).HasColumnType("smallint(6)");

                entity.Property(e => e.Term).HasColumnType("tinyint(4)");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(e => e.Email, "Email")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("ตามบัตรปชช.")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Hint)
                    .HasMaxLength(200)
                    .HasComment("คำใบ้สำหรับ keyword กรณีลืมรหัสและพิมพ์ keyword ผิดเกิน 3 ครั้ง")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Keyword)
                    .HasMaxLength(200)
                    .HasComment("keyword ยืนยันตัวตนตอนเปลี่ยน/ลืมรหัสผ่าน")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Major)
                    .HasColumnType("enum('MATH','COMP')")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Password)
                    .HasMaxLength(44)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .HasComment("xxx-xxx-xxxx")
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.ProfilePicture).HasMaxLength(50);

                entity.Property(e => e.Salt)
                    .HasMaxLength(24)
                    .UseCollation("utf8mb4_thai_520_w2");

                entity.Property(e => e.Title)
                    .HasMaxLength(10)
                    .UseCollation("utf8mb4_thai_520_w2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

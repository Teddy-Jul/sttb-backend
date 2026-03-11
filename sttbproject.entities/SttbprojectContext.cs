using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace sttbproject.entities;

public partial class SttbprojectContext : DbContext
{
    public SttbprojectContext()
    {
    }

    public SttbprojectContext(DbContextOptions<SttbprojectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryCourse> CategoryCourses { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<PageView> PageViews { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<ProgramCourseCategory> ProgramCourseCategories { get; set; }

    public virtual DbSet<ProgramFee> ProgramFees { get; set; }

    public virtual DbSet<ProgramFeeCategory> ProgramFeeCategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SearchIndex> SearchIndices { get; set; }

    public virtual DbSet<SiteSetting> SiteSettings { get; set; }

    public virtual DbSet<StudyProgram> StudyPrograms { get; set; }

    public virtual DbSet<SystemLog> SystemLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=sttbproject;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__audit_lo__6031F9F87D8F3580");

            entity.ToTable("audit_logs");

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "idx_audit_logs_entity");

            entity.HasIndex(e => e.UserId, "idx_audit_logs_user_id");

            entity.Property(e => e.AuditLogId).HasColumnName("audit_log_id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__audit_log__user___1C5231C2");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__categori__D54EE9B4E91E2FB5");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Slug, "UQ__categori__32DD1E4C01BFA60C").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<CategoryCourse>(entity =>
        {
            entity.HasKey(e => e.CategoryCourseId).HasName("PK__category__2E903533A668EAD0");

            entity.ToTable("category_courses");

            entity.HasIndex(e => e.CourseId, "idx_category_courses_course");

            entity.HasIndex(e => e.ProgramCategoryId, "idx_category_courses_program_category");

            entity.Property(e => e.CategoryCourseId).HasColumnName("category_course_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Credits).HasColumnName("credits");
            entity.Property(e => e.ProgramCategoryId).HasColumnName("program_category_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Course).WithMany(p => p.CategoryCourses)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__category___cours__3EA749C6");

            entity.HasOne(d => d.ProgramCategory).WithMany(p => p.CategoryCourses)
                .HasForeignKey(d => d.ProgramCategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__category___progr__3DB3258D");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.ContactMessageId).HasName("PK__contact___8463BC75F3E4A71A");

            entity.ToTable("contact_messages");

            entity.HasIndex(e => e.Status, "idx_contact_messages_status");

            entity.Property(e => e.ContactMessageId).HasColumnName("contact_message_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.MessageText).HasColumnName("message_text");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("new")
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(200)
                .HasColumnName("subject");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__courses__8F1EF7AEAC9D9C45");

            entity.ToTable("courses");

            entity.HasIndex(e => e.CourseName, "idx_courses_name");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .HasColumnName("course_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__course_c__D54EE9B460086E07");

            entity.ToTable("course_categories");

            entity.HasIndex(e => e.CategoryName, "idx_course_categories_name");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(200)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Medium>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("PK__media__D0A840F470E08056");

            entity.ToTable("media");

            entity.Property(e => e.MediaId).HasColumnName("media_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath).HasColumnName("file_path");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .HasColumnName("file_type");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Media)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK__media__uploaded___019E3B86");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__menus__4CA0FADCD9E4F0F8");

            entity.ToTable("menus");

            entity.Property(e => e.MenuId).HasColumnName("menu_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuItemId).HasName("PK__menu_ite__973431D5D0A0D1DE");

            entity.ToTable("menu_items");

            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.MenuId).HasColumnName("menu_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.Menu).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK__menu_item__menu___14B10FFA");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__menu_item__paren__15A53433");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.HasKey(e => e.PageId).HasName("PK__pages__637F305ADCDD1DA9");

            entity.ToTable("pages");

            entity.HasIndex(e => e.Slug, "UQ__pages__32DD1E4CB4FA0291").IsUnique();

            entity.HasIndex(e => e.Slug, "idx_pages_slug");

            entity.HasIndex(e => e.Status, "idx_pages_status");

            entity.Property(e => e.PageId).HasColumnName("page_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("draft")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PageCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__pages__created_b__7CD98669");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PageUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__pages__updated_b__7DCDAAA2");
        });

        modelBuilder.Entity<PageView>(entity =>
        {
            entity.HasKey(e => e.PageViewId).HasName("PK__page_vie__49E7912C7D8642CE");

            entity.ToTable("page_views");

            entity.HasIndex(e => e.PageId, "idx_page_views_page_id");

            entity.Property(e => e.PageViewId).HasColumnName("page_view_id");
            entity.Property(e => e.PageId).HasColumnName("page_id");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.ViewedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("viewed_at");
            entity.Property(e => e.VisitorIp)
                .HasMaxLength(50)
                .HasColumnName("visitor_ip");

            entity.HasOne(d => d.Page).WithMany(p => p.PageViews)
                .HasForeignKey(d => d.PageId)
                .HasConstraintName("FK__page_view__page___22FF2F51");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__permissi__E5331AFA68092CAA");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.Name, "UQ__permissi__72E12F1B40274391").IsUnique();

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__posts__3ED787667916AD55");

            entity.ToTable("posts");

            entity.HasIndex(e => e.Slug, "UQ__posts__32DD1E4C1A1DE274").IsUnique();

            entity.HasIndex(e => e.AuthorId, "idx_posts_author_id");

            entity.HasIndex(e => e.PublishedAt, "idx_posts_published_at");

            entity.HasIndex(e => e.Slug, "idx_posts_slug");

            entity.HasIndex(e => e.Status, "idx_posts_status");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Excerpt).HasColumnName("excerpt");
            entity.Property(e => e.FeaturedImageId).HasColumnName("featured_image_id");
            entity.Property(e => e.PublishedAt).HasColumnName("published_at");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("draft")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Author).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__posts__author_id__084B3915");

            entity.HasOne(d => d.FeaturedImage).WithMany(p => p.Posts)
                .HasForeignKey(d => d.FeaturedImageId)
                .HasConstraintName("FK__posts__featured___0662F0A3");

            entity.HasMany(d => d.Categories).WithMany(p => p.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "PostCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK__post_cate__categ__0FEC5ADD"),
                    l => l.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .HasConstraintName("FK__post_cate__post___0EF836A4"),
                    j =>
                    {
                        j.HasKey("PostId", "CategoryId").HasName("PK__post_cat__638369FDC4824D2A");
                        j.ToTable("post_categories");
                        j.IndexerProperty<int>("PostId").HasColumnName("post_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });
        });

        modelBuilder.Entity<ProgramCourseCategory>(entity =>
        {
            entity.HasKey(e => e.ProgramCategoryId).HasName("PK__program___06381A5BAE03EA2C");

            entity.ToTable("program_course_categories");

            entity.HasIndex(e => e.CategoryId, "idx_program_course_categories_category");

            entity.HasIndex(e => e.ProgramId, "idx_program_course_categories_program");

            entity.Property(e => e.ProgramCategoryId).HasColumnName("program_category_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.ProgramId).HasColumnName("program_id");
            entity.Property(e => e.TotalCredits).HasColumnName("total_credits");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.ProgramCourseCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__program_c__categ__351DDF8C");

            entity.HasOne(d => d.Program).WithMany(p => p.ProgramCourseCategories)
                .HasForeignKey(d => d.ProgramId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__program_c__progr__3429BB53");
        });

        modelBuilder.Entity<ProgramFee>(entity =>
        {
            entity.HasKey(e => e.FeeId).HasName("PK__program___A19C8AFBBA2D25B8");

            entity.ToTable("program_fees");

            entity.HasIndex(e => e.FeeCategoryId, "idx_program_fees_category");

            entity.HasIndex(e => e.ProgramId, "idx_program_fees_program");

            entity.Property(e => e.FeeId).HasColumnName("fee_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.FeeCategoryId).HasColumnName("fee_category_id");
            entity.Property(e => e.FeeName)
                .HasMaxLength(200)
                .HasColumnName("fee_name");
            entity.Property(e => e.ProgramId).HasColumnName("program_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.FeeCategory).WithMany(p => p.ProgramFees)
                .HasForeignKey(d => d.FeeCategoryId)
                .HasConstraintName("FK__program_f__fee_c__4830B400");

            entity.HasOne(d => d.Program).WithMany(p => p.ProgramFees)
                .HasForeignKey(d => d.ProgramId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__program_f__progr__473C8FC7");
        });

        modelBuilder.Entity<ProgramFeeCategory>(entity =>
        {
            entity.HasKey(e => e.FeeCategoryId).HasName("PK__program___FB1019012C589CCB");

            entity.ToTable("program_fee_categories");

            entity.Property(e => e.FeeCategoryId).HasColumnName("fee_category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__760965CC1C26CC8D");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "UQ__roles__72E12F1BEA66D70B").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("FK__role_perm__permi__7814D14C"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__role_perm__role___7720AD13"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__role_per__C85A5463E01CE137");
                        j.ToTable("role_permissions");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<SearchIndex>(entity =>
        {
            entity.HasKey(e => e.SearchIndexId).HasName("PK__search_i__995B47BF80098EBA");

            entity.ToTable("search_index");

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "idx_search_index_entity");

            entity.Property(e => e.SearchIndexId).HasColumnName("search_index_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(e => e.SiteSettingId).HasName("PK__site_set__2C48802318BB31A6");

            entity.ToTable("site_settings");

            entity.HasIndex(e => e.SettingKey, "UQ__site_set__0DFAC427B9967BD9").IsUnique();

            entity.Property(e => e.SiteSettingId).HasColumnName("site_setting_id");
            entity.Property(e => e.SettingKey)
                .HasMaxLength(100)
                .HasColumnName("setting_key");
            entity.Property(e => e.SettingValue).HasColumnName("setting_value");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<StudyProgram>(entity =>
        {
            entity.HasKey(e => e.ProgramId).HasName("PK__study_pr__3A7890ACC56BFED1");

            entity.ToTable("study_programs");

            entity.HasIndex(e => e.DegreeLevel, "idx_study_programs_degree_level");

            entity.Property(e => e.ProgramId).HasColumnName("program_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DegreeLevel)
                .HasMaxLength(20)
                .HasColumnName("degree_level");
            entity.Property(e => e.DegreeTitle)
                .HasMaxLength(50)
                .HasColumnName("degree_title");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ProgramName)
                .HasMaxLength(200)
                .HasColumnName("program_name");
            entity.Property(e => e.StudyDuration)
                .HasMaxLength(100)
                .HasColumnName("study_duration");
            entity.Property(e => e.TotalCredits).HasColumnName("total_credits");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.SystemLogId).HasName("PK__system_l__2D4D082A69A58B57");

            entity.ToTable("system_logs");

            entity.Property(e => e.SystemLogId).HasColumnName("system_log_id");
            entity.Property(e => e.Context).HasColumnName("context");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Level)
                .HasMaxLength(20)
                .HasColumnName("level");
            entity.Property(e => e.LogMessage).HasColumnName("log_message");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F6329708E");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E61642CD7A63E").IsUnique();

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.RoleId, "idx_users_role_id");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__users__role_id__6F7F8B4B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

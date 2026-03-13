-- ============================================
-- STTB Database Initialization Script
-- SQL Server (SSMS) Version
-- ============================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'sttbproject')
BEGIN
    CREATE DATABASE sttbproject;
END
GO

USE sttbproject;
GO

-- ============================================
-- DROP TABLES (for clean re-initialization)
-- Order: children before parents (FK dependency order)
-- ============================================

-- Leaf / junction tables first (no other tables depend on them)
IF OBJECT_ID('search_index', 'U') IS NOT NULL DROP TABLE search_index;
IF OBJECT_ID('page_views', 'U') IS NOT NULL DROP TABLE page_views;
IF OBJECT_ID('system_logs', 'U') IS NOT NULL DROP TABLE system_logs;
IF OBJECT_ID('audit_logs', 'U') IS NOT NULL DROP TABLE audit_logs;
IF OBJECT_ID('contact_messages', 'U') IS NOT NULL DROP TABLE contact_messages;
IF OBJECT_ID('menu_items', 'U') IS NOT NULL DROP TABLE menu_items;
IF OBJECT_ID('menus', 'U') IS NOT NULL DROP TABLE menus;
IF OBJECT_ID('post_categories', 'U') IS NOT NULL DROP TABLE post_categories;
IF OBJECT_ID('role_permissions', 'U') IS NOT NULL DROP TABLE role_permissions;
IF OBJECT_ID('category_courses', 'U') IS NOT NULL DROP TABLE category_courses;
IF OBJECT_ID('program_course_categories', 'U') IS NOT NULL DROP TABLE program_course_categories;
IF OBJECT_ID('program_fees', 'U') IS NOT NULL DROP TABLE program_fees;

-- Tables that reference media / users — must be dropped before media and users
IF OBJECT_ID('academic_calendars', 'U') IS NOT NULL DROP TABLE academic_calendars;
IF OBJECT_ID('events', 'U') IS NOT NULL DROP TABLE events;
IF OBJECT_ID('posts', 'U') IS NOT NULL DROP TABLE posts;
IF OBJECT_ID('pages', 'U') IS NOT NULL DROP TABLE pages;

-- Tables that reference courses / study_programs / categories
IF OBJECT_ID('courses', 'U') IS NOT NULL DROP TABLE courses;
IF OBJECT_ID('course_categories', 'U') IS NOT NULL DROP TABLE course_categories;
IF OBJECT_ID('program_fee_categories', 'U') IS NOT NULL DROP TABLE program_fee_categories;
IF OBJECT_ID('study_programs', 'U') IS NOT NULL DROP TABLE study_programs;
IF OBJECT_ID('categories', 'U') IS NOT NULL DROP TABLE categories;

-- Parent tables (referenced by the ones above)
IF OBJECT_ID('permissions', 'U') IS NOT NULL DROP TABLE permissions;
IF OBJECT_ID('media', 'U') IS NOT NULL DROP TABLE media;
IF OBJECT_ID('users', 'U') IS NOT NULL DROP TABLE users;
IF OBJECT_ID('roles', 'U') IS NOT NULL DROP TABLE roles;
IF OBJECT_ID('site_settings', 'U') IS NOT NULL DROP TABLE site_settings;
GO

-- ============================================
-- CREATE TABLES
-- ============================================

-- 1. Roles
IF OBJECT_ID('roles', 'U') IS NULL
BEGIN
    CREATE TABLE roles (
        role_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(50) UNIQUE NOT NULL,
        description NVARCHAR(MAX) NULL
    );
END
GO

-- 2. Users
IF OBJECT_ID('users', 'U') IS NULL
BEGIN
    CREATE TABLE users (
        user_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(100) NULL,
        email NVARCHAR(150) UNIQUE NOT NULL,
        password_hash NVARCHAR(MAX) NOT NULL,
        role_id INT NULL FOREIGN KEY REFERENCES roles(role_id),
        status NVARCHAR(20) DEFAULT 'active',
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 NULL
    );
END
GO

-- 3. Permissions
IF OBJECT_ID('permissions', 'U') IS NULL
BEGIN
    CREATE TABLE permissions (
        permission_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(100) UNIQUE NOT NULL,
        description NVARCHAR(MAX) NULL
    );
END
GO

-- 4. Role Permissions
IF OBJECT_ID('role_permissions', 'U') IS NULL
BEGIN
    CREATE TABLE role_permissions (
        role_id INT NOT NULL FOREIGN KEY REFERENCES roles(role_id) ON DELETE CASCADE,
        permission_id INT NOT NULL FOREIGN KEY REFERENCES permissions(permission_id) ON DELETE CASCADE,
        PRIMARY KEY (role_id, permission_id)
    );
END
GO

-- 5. Pages
IF OBJECT_ID('pages', 'U') IS NULL
BEGIN
    CREATE TABLE pages (
        page_id INT IDENTITY(1,1) PRIMARY KEY,
        title NVARCHAR(200) NULL,
        slug NVARCHAR(200) UNIQUE NULL,
        content NVARCHAR(MAX) NULL,
        status NVARCHAR(20) DEFAULT 'draft',
        created_by INT NULL FOREIGN KEY REFERENCES users(user_id),
        updated_by INT NULL FOREIGN KEY REFERENCES users(user_id),
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 NULL
    );
END
GO

-- 6. Media
IF OBJECT_ID('media', 'U') IS NULL
BEGIN
    CREATE TABLE media (
        media_id INT IDENTITY(1,1) PRIMARY KEY,
        file_name NVARCHAR(255) NULL,
        file_path NVARCHAR(MAX) NULL,
        file_type NVARCHAR(50) NULL,
        file_size BIGINT NULL,
        uploaded_by INT NULL FOREIGN KEY REFERENCES users(user_id),
        created_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- 7. Posts (Blog/News)
IF OBJECT_ID('posts', 'U') IS NULL
BEGIN
    CREATE TABLE posts (
        post_id INT IDENTITY(1,1) PRIMARY KEY,
        title NVARCHAR(255) NULL,
        slug NVARCHAR(255) UNIQUE NULL,
        content NVARCHAR(MAX) NULL,
        excerpt NVARCHAR(MAX) NULL,
        featured_image_id INT NULL FOREIGN KEY REFERENCES media(media_id),
        status NVARCHAR(20) DEFAULT 'draft',
        author_id INT NULL FOREIGN KEY REFERENCES users(user_id),
        published_at DATETIME2 NULL,
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 NULL
    );
END
GO

-- 8. Categories
IF OBJECT_ID('categories', 'U') IS NULL
BEGIN
    CREATE TABLE categories (
        category_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(100) NULL,
        slug NVARCHAR(100) UNIQUE NULL
    );
END
GO

-- 9. Post Categories
IF OBJECT_ID('post_categories', 'U') IS NULL
BEGIN
    CREATE TABLE post_categories (
        post_id INT NOT NULL FOREIGN KEY REFERENCES posts(post_id) ON DELETE CASCADE,
        category_id INT NOT NULL FOREIGN KEY REFERENCES categories(category_id) ON DELETE CASCADE,
        PRIMARY KEY (post_id, category_id)
    );
END
GO

-- 10. Menus
IF OBJECT_ID('menus', 'U') IS NULL
BEGIN
    CREATE TABLE menus (
        menu_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(100) NULL
    );
END
GO

-- 11. Menu Items
IF OBJECT_ID('menu_items', 'U') IS NULL
BEGIN
    CREATE TABLE menu_items (
        menu_item_id INT IDENTITY(1,1) PRIMARY KEY,
        menu_id INT NOT NULL FOREIGN KEY REFERENCES menus(menu_id) ON DELETE CASCADE,
        title NVARCHAR(100) NULL,
        url NVARCHAR(255) NULL,
        parent_id INT NULL FOREIGN KEY REFERENCES menu_items(menu_item_id),
        position INT NULL
    );
END
GO

-- 12. Contact Messages
IF OBJECT_ID('contact_messages', 'U') IS NULL
BEGIN
    CREATE TABLE contact_messages (
        contact_message_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(100) NULL,
        email NVARCHAR(150) NULL,
        subject NVARCHAR(200) NULL,
        message_text NVARCHAR(MAX) NULL,
        status NVARCHAR(20) DEFAULT 'new',
        created_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- 13. Audit Logs
IF OBJECT_ID('audit_logs', 'U') IS NULL
BEGIN
    CREATE TABLE audit_logs (
        audit_log_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NULL FOREIGN KEY REFERENCES users(user_id),
        action NVARCHAR(100) NULL,
        entity_type NVARCHAR(50) NULL,
        entity_id INT NULL,
        details NVARCHAR(MAX) NULL,
        created_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- 14. System Logs
IF OBJECT_ID('system_logs', 'U') IS NULL
BEGIN
    CREATE TABLE system_logs (
        system_log_id INT IDENTITY(1,1) PRIMARY KEY,
        level NVARCHAR(20) NULL,
        log_message NVARCHAR(MAX) NULL,
        context NVARCHAR(MAX) NULL, -- JSON stored as NVARCHAR(MAX)
        created_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- 15. Page Views (Analytics)
IF OBJECT_ID('page_views', 'U') IS NULL
BEGIN
    CREATE TABLE page_views (
        page_view_id INT IDENTITY(1,1) PRIMARY KEY,
        page_id INT NULL FOREIGN KEY REFERENCES pages(page_id),
        visitor_ip NVARCHAR(50) NULL,
        user_agent NVARCHAR(MAX) NULL,
        viewed_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- 16. Site Settings
IF OBJECT_ID('site_settings', 'U') IS NULL
BEGIN
    CREATE TABLE site_settings (
        site_setting_id INT IDENTITY(1,1) PRIMARY KEY,
        setting_key NVARCHAR(100) UNIQUE NOT NULL,
        setting_value NVARCHAR(MAX) NULL,
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- 17. Search Index
IF OBJECT_ID('search_index', 'U') IS NULL
BEGIN
    CREATE TABLE search_index (
        search_index_id INT IDENTITY(1,1) PRIMARY KEY,
        entity_type NVARCHAR(50) NULL,
        entity_id INT NULL,
        title NVARCHAR(MAX) NULL,
        content NVARCHAR(MAX) NULL,
        updated_at DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('study_programs', 'U') IS NULL
BEGIN
    CREATE TABLE study_programs (
        program_id INT IDENTITY(1,1) PRIMARY KEY,
        program_name NVARCHAR(200) NOT NULL,
        degree_level NVARCHAR(20),
        degree_title NVARCHAR(50),
        total_credits INT,
        study_duration NVARCHAR(100),
        description NVARCHAR(MAX),
        slug NVARCHAR(200) UNIQUE NULL,
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('course_categories', 'U') IS NULL
BEGIN
    CREATE TABLE course_categories (
        category_id INT IDENTITY(1,1) PRIMARY KEY,
        category_name NVARCHAR(200),
        description NVARCHAR(MAX),
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('program_course_categories', 'U') IS NULL
BEGIN
    CREATE TABLE program_course_categories (
        program_category_id INT IDENTITY(1,1) PRIMARY KEY,
        program_id INT
            FOREIGN KEY REFERENCES study_programs(program_id) ON DELETE CASCADE,
        category_id INT
            FOREIGN KEY REFERENCES course_categories(category_id),
        total_credits INT,
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('courses', 'U') IS NULL
BEGIN
    CREATE TABLE courses (
        course_id INT IDENTITY(1,1) PRIMARY KEY,
        course_name NVARCHAR(255),
        description NVARCHAR(MAX),
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('category_courses', 'U') IS NULL
BEGIN
    CREATE TABLE category_courses (
        category_course_id INT IDENTITY(1,1) PRIMARY KEY,
        program_category_id INT
            FOREIGN KEY REFERENCES program_course_categories(program_category_id)
            ON DELETE CASCADE,
        course_id INT
            FOREIGN KEY REFERENCES courses(course_id),
        credits INT,
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('program_fee_categories', 'U') IS NULL
BEGIN
    CREATE TABLE program_fee_categories (
        fee_category_id INT IDENTITY(1,1) PRIMARY KEY,
        category_name NVARCHAR(100),
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('program_fees', 'U') IS NULL
BEGIN
    CREATE TABLE program_fees (
        fee_id INT IDENTITY(1,1) PRIMARY KEY,
        program_id INT
            FOREIGN KEY REFERENCES study_programs(program_id) ON DELETE CASCADE,
        fee_category_id INT
            FOREIGN KEY REFERENCES program_fee_categories(fee_category_id),
        fee_name NVARCHAR(200),
        amount DECIMAL(12,2),
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('events', 'U') IS NULL
BEGIN
    CREATE TABLE events (
        event_id INT IDENTITY(1,1) PRIMARY KEY,
        title NVARCHAR(255) NOT NULL,
        slug NVARCHAR(255) UNIQUE NULL,
        description NVARCHAR(MAX) NULL,
        location NVARCHAR(255) NULL,
        start_date DATETIME2 NOT NULL,
        end_date DATETIME2 NULL,
        featured_image_id INT NULL FOREIGN KEY REFERENCES media(media_id),
        status NVARCHAR(20) DEFAULT 'draft',   -- draft | published | cancelled
        created_by INT NULL FOREIGN KEY REFERENCES users(user_id),
        updated_by INT NULL FOREIGN KEY REFERENCES users(user_id),
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('academic_calendars', 'U') IS NULL
BEGIN
    CREATE TABLE academic_calendars (
        academic_calendar_id INT IDENTITY(1,1) PRIMARY KEY,
        title NVARCHAR(255) NOT NULL,
        slug NVARCHAR(255) UNIQUE NULL,
        description NVARCHAR(MAX) NULL,
        start_date DATETIME2 NOT NULL,
        end_date DATETIME2 NULL,
        academic_year NVARCHAR(20) NULL,   -- e.g. "2025/2026"
        semester NVARCHAR(20) NULL,        -- "Ganjil" | "Genap"
        event_type NVARCHAR(50) NULL,      -- "UTS" | "UAS" | "Libur" | "Pendaftaran" | etc.
        status NVARCHAR(20) DEFAULT 'published',
        featured_image_id INT NULL FOREIGN KEY REFERENCES media(media_id),
        created_by INT NULL FOREIGN KEY REFERENCES users(user_id),
        updated_by INT NULL FOREIGN KEY REFERENCES users(user_id),
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 NULL
    );
END
GO

-- Add featured_image_id to existing academic_calendars tables (idempotent)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('academic_calendars') AND name = 'featured_image_id'
)
BEGIN
    ALTER TABLE academic_calendars
        ADD featured_image_id INT NULL
            CONSTRAINT FK__academic_calendars__featured_image_id
            FOREIGN KEY REFERENCES media(media_id);
END
GO

-- ============================================
-- INSERT DUMMY DATA
-- ============================================

-- Insert Roles
SET IDENTITY_INSERT roles ON;
IF NOT EXISTS (SELECT 1 FROM roles WHERE role_id = 1)
    INSERT INTO roles (role_id, name, description) VALUES (1, 'super_admin', 'Full system access with all privileges');
IF NOT EXISTS (SELECT 1 FROM roles WHERE role_id = 2)
    INSERT INTO roles (role_id, name, description) VALUES (2, 'admin', 'Administrative access to manage content');
IF NOT EXISTS (SELECT 1 FROM roles WHERE role_id = 3)
    INSERT INTO roles (role_id, name, description) VALUES (3, 'editor', 'Can create, edit, and publish content');
IF NOT EXISTS (SELECT 1 FROM roles WHERE role_id = 4)
    INSERT INTO roles (role_id, name, description) VALUES (4, 'content_creator', 'Can create and edit own content');
IF NOT EXISTS (SELECT 1 FROM roles WHERE role_id = 5)
    INSERT INTO roles (role_id, name, description) VALUES (5, 'marketing', 'Manage promotions and events');
IF NOT EXISTS (SELECT 1 FROM roles WHERE role_id = 6)
    INSERT INTO roles (role_id, name, description) VALUES (6, 'user', 'Regular user with read-only access');
SET IDENTITY_INSERT roles OFF;
GO

-- Insert Permissions
SET IDENTITY_INSERT permissions ON;
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 1)
    INSERT INTO permissions (permission_id, name, description) VALUES (1, 'manage_users', 'Create, update, and delete users');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 2)
    INSERT INTO permissions (permission_id, name, description) VALUES (2, 'manage_roles', 'Manage roles and permissions');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 3)
    INSERT INTO permissions (permission_id, name, description) VALUES (3, 'manage_settings', 'Update system settings');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 4)
    INSERT INTO permissions (permission_id, name, description) VALUES (4, 'view_logs', 'View audit and system logs');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 5)
    INSERT INTO permissions (permission_id, name, description) VALUES (5, 'edit_pages', 'Edit static website pages');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 6)
    INSERT INTO permissions (permission_id, name, description) VALUES (6, 'publish_content', 'Publish articles and posts');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 7)
    INSERT INTO permissions (permission_id, name, description) VALUES (7, 'manage_media', 'Upload and manage media files');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 8)
    INSERT INTO permissions (permission_id, name, description) VALUES (8, 'create_articles', 'Create blog articles');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 9)
    INSERT INTO permissions (permission_id, name, description) VALUES (9, 'edit_articles', 'Edit blog articles');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 10)
    INSERT INTO permissions (permission_id, name, description) VALUES (10, 'delete_articles', 'Delete blog articles');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 11)
    INSERT INTO permissions (permission_id, name, description) VALUES (11, 'manage_events', 'Create and manage events');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 12)
    INSERT INTO permissions (permission_id, name, description) VALUES (12, 'manage_menu', 'Manage navigation menus');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 13)
    INSERT INTO permissions (permission_id, name, description) VALUES (13, 'view_analytics', 'View website analytics');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 14)
    INSERT INTO permissions (permission_id, name, description) VALUES (14, 'programs.view', 'View study programs');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 15)
    INSERT INTO permissions (permission_id, name, description) VALUES (15, 'programs.create', 'Create new study programs');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 16)
    INSERT INTO permissions (permission_id, name, description) VALUES (16, 'programs.edit', 'Edit existing study programs');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 17)
    INSERT INTO permissions (permission_id, name, description) VALUES (17, 'programs.delete', 'Delete study programs');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 18)
    INSERT INTO permissions (permission_id, name, description) VALUES (18, 'programs.manage', 'Full management access to study programs');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 19)
    INSERT INTO permissions (permission_id, name, description) VALUES (19, 'courses.view', 'View courses');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 20)
    INSERT INTO permissions (permission_id, name, description) VALUES (20, 'courses.create', 'Create new courses');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 21)
    INSERT INTO permissions (permission_id, name, description) VALUES (21, 'courses.edit', 'Edit existing courses');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 22)
    INSERT INTO permissions (permission_id, name, description) VALUES (22, 'courses.delete', 'Delete courses');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 23)
    INSERT INTO permissions (permission_id, name, description) VALUES (23, 'courses.manage', 'Full management access to courses');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 24)
    INSERT INTO permissions (permission_id, name, description) VALUES (24, 'program_fees.view', 'View program fees');
IF NOT EXISTS (SELECT 1 FROM permissions WHERE permission_id = 25)
    INSERT INTO permissions (permission_id, name, description) VALUES (25, 'program_fees.manage', 'Manage program fees');
SET IDENTITY_INSERT permissions OFF;
GO

-- ============================================
-- ASSIGN PERMISSIONS TO ROLES
-- ============================================

-- ============================================
-- SUPER ADMIN (role_id = 1)
-- gets ALL permissions
-- ============================================
INSERT INTO role_permissions (role_id, permission_id)
SELECT 1, permission_id
FROM permissions
WHERE NOT EXISTS (
    SELECT 1 
    FROM role_permissions rp
    WHERE rp.role_id = 1 
    AND rp.permission_id = permissions.permission_id
);



-- ============================================
-- ADMIN (role_id = 2)
-- full management access
-- ============================================
INSERT INTO role_permissions (role_id, permission_id)
SELECT 2, permission_id
FROM permissions
WHERE name IN (
    'programs.view','programs.create','programs.edit','programs.delete','programs.manage',
    'courses.view','courses.create','courses.edit','courses.delete','courses.manage',
    'program_fees.view','program_fees.manage'
)
AND NOT EXISTS (
    SELECT 1 
    FROM role_permissions rp
    WHERE rp.role_id = 2 
    AND rp.permission_id = permissions.permission_id
);



-- ============================================
-- EDITOR (role_id = 3)
-- view + edit only
-- ============================================
INSERT INTO role_permissions (role_id, permission_id)
SELECT 3, permission_id
FROM permissions
WHERE name IN (
    'programs.view','programs.edit',
    'courses.view','courses.edit',
    'program_fees.view'
)
AND NOT EXISTS (
    SELECT 1 
    FROM role_permissions rp
    WHERE rp.role_id = 3 
    AND rp.permission_id = permissions.permission_id
);



-- ============================================
-- CONTENT CREATOR (role_id = 4)
-- view content only
-- ============================================
INSERT INTO role_permissions (role_id, permission_id)
SELECT 4, permission_id
FROM permissions
WHERE name IN (
    'programs.view',
    'courses.view',
    'program_fees.view'
)
AND NOT EXISTS (
    SELECT 1 
    FROM role_permissions rp
    WHERE rp.role_id = 4 
    AND rp.permission_id = permissions.permission_id
);



-- ============================================
-- MARKETING (role_id = 5)
-- marketing + program fees visibility
-- ============================================
INSERT INTO role_permissions (role_id, permission_id)
SELECT 5, permission_id
FROM permissions
WHERE name IN (
    'programs.view',
    'courses.view',
    'program_fees.view',
    'program_fees.manage'
)
AND NOT EXISTS (
    SELECT 1 
    FROM role_permissions rp
    WHERE rp.role_id = 5 
    AND rp.permission_id = permissions.permission_id
);



-- ============================================
-- REGULAR USER (role_id = 6)
-- view only
-- ============================================
INSERT INTO role_permissions (role_id, permission_id)
SELECT 6, permission_id
FROM permissions
WHERE name IN (
    'programs.view',
    'courses.view',
    'program_fees.view'
)
AND NOT EXISTS (
    SELECT 1 
    FROM role_permissions rp
    WHERE rp.role_id = 6 
    AND rp.permission_id = permissions.permission_id
);

GO

-- Insert Users (Password: 'Password123!' - should be hashed in production)
SET IDENTITY_INSERT users ON;
IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = 1)
    INSERT INTO users (user_id, name, email, password_hash, role_id, status) VALUES (1, 'Super Admin', 'superadmin@sttb.ac.id', '$2a$12$chFt9a6xmn3Tht8OdXeAo.fO79NmzJSrJyIaBl4WDbgEyZ2K997EC', 1, 'active');
IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = 2)
    INSERT INTO users (user_id, name, email, password_hash, role_id, status) VALUES (2, 'Admin User', 'admin@sttb.ac.id', '$2a$12$HykkVxf2IqHIL9/waeG0UO7gpFG4n6xLavOClhuZjchLzd9ClNmca', 2, 'active');
IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = 3)
    INSERT INTO users (user_id, name, email, password_hash, role_id, status) VALUES (3, 'Editor John', 'editor@sttb.ac.id', '$2a$12$WJYrN9n3RwAD14Mu/ytaM..bn7rkdNlnJwuQ1S9yntyL55Q6km/8i', 3, 'active');
IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = 4)
    INSERT INTO users (user_id, name, email, password_hash, role_id, status) VALUES (4, 'Content Creator Jane', 'creator@sttb.ac.id', '$2a$12$.pzXkVWuHiQMPIRaRnKtAeETA2EJiEgKPtpx9gD1L6A/udyNJ42ua', 4, 'active');
IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = 5)
    INSERT INTO users (user_id, name, email, password_hash, role_id, status) VALUES (5, 'Marketing Mike', 'marketing@sttb.ac.id', '$2a$12$GEucw.VbybLm9591CP7DneY7mHpbTIL284ZOs.R7h8EO9ewjlVhRq', 5, 'active');
SET IDENTITY_INSERT users OFF;
GO

-- Insert Site Settings
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'site_name')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('site_name', 'Sekolah Tinggi Teologi Bethel');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'site_abbreviation')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('site_abbreviation', 'STTB');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'site_email')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('site_email', 'info@sttb.ac.id');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'contact_phone')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('contact_phone', '021-8765432');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'contact_address')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('contact_address', 'Jl. Teologia No. 123, Jakarta');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'facebook_url')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('facebook_url', 'https://facebook.com/sttb');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'instagram_url')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('instagram_url', 'https://instagram.com/sttb');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'twitter_url')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('twitter_url', 'https://twitter.com/sttb');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'maintenance_mode')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('maintenance_mode', 'false');
IF NOT EXISTS (SELECT 1 FROM site_settings WHERE setting_key = 'registration_open')
    INSERT INTO site_settings (setting_key, setting_value) VALUES ('registration_open', 'true');
GO

-- Insert Pages
SET IDENTITY_INSERT pages ON;
IF NOT EXISTS (SELECT 1 FROM pages WHERE page_id = 1)
    INSERT INTO pages (page_id, title, slug, content, status, created_by, created_at) VALUES (1, 'Home', 'home', '<h1>Welcome to STTB</h1><p>Sekolah Tinggi Teologi Bethel is a premier theological institution.</p>', 'published', 1, GETDATE());
IF NOT EXISTS (SELECT 1 FROM pages WHERE page_id = 2)
    INSERT INTO pages (page_id, title, slug, content, status, created_by, created_at) VALUES (2, 'About Us', 'about', '<h1>About STTB</h1><p>Founded in 1950, STTB has been providing quality theological education.</p>', 'published', 1, GETDATE());
IF NOT EXISTS (SELECT 1 FROM pages WHERE page_id = 3)
    INSERT INTO pages (page_id, title, slug, content, status, created_by, created_at) VALUES (3, 'Academic Programs', 'programs', '<h1>Our Programs</h1><p>We offer Strata 1 and Strata 2 programs in various theological studies.</p>', 'published', 1, GETDATE());
IF NOT EXISTS (SELECT 1 FROM pages WHERE page_id = 4)
    INSERT INTO pages (page_id, title, slug, content, status, created_by, created_at) VALUES (4, 'Admission', 'admission', '<h1>Admission Information</h1><p>Learn how to apply to STTB.</p>', 'published', 1, GETDATE());
IF NOT EXISTS (SELECT 1 FROM pages WHERE page_id = 5)
    INSERT INTO pages (page_id, title, slug, content, status, created_by, created_at) VALUES (5, 'Contact Us', 'contact', '<h1>Contact Information</h1><p>Get in touch with us.</p>', 'published', 1, GETDATE());
SET IDENTITY_INSERT pages OFF;
GO

-- Insert Media [have to input manual via API]
SET IDENTITY_INSERT media ON;
IF NOT EXISTS (SELECT 1 FROM media WHERE media_id = 6)
    INSERT INTO media (media_id, file_name, file_path, file_type, file_size, uploaded_by) VALUES (6,'download1.jpg','media/2026/03/e2dd4b5c-3db8-43d5-802e-6728e28e7592.jpeg','image/jpeg',6290,1);
IF NOT EXISTS (SELECT 1 FROM media WHERE media_id = 7)
    INSERT INTO media (media_id, file_name, file_path, file_type, file_size, uploaded_by) VALUES (7,'download2.jpeg','media/2026/03/c464d3e9-a167-4339-a58f-ab6cb9ea1669.jpeg','image/jpeg',5183,1);
-- Dummy Data VV
IF NOT EXISTS (SELECT 1 FROM media WHERE media_id = 1)
    INSERT INTO media (media_id, file_name, file_path, file_type, file_size, uploaded_by) VALUES (1, 'logo.png', '/uploads/images/logo.png', 'image/png', 45678, 1);
IF NOT EXISTS (SELECT 1 FROM media WHERE media_id = 2)
    INSERT INTO media (media_id, file_name, file_path, file_type, file_size, uploaded_by) VALUES (2, 'hero-banner.jpg', '/uploads/images/hero-banner.jpg', 'image/jpeg', 234567, 1);
IF NOT EXISTS (SELECT 1 FROM media WHERE media_id = 3)
    INSERT INTO media (media_id, file_name, file_path, file_type, file_size, uploaded_by) VALUES (3, 'campus-building.jpg', '/uploads/images/campus-building.jpg', 'image/jpeg', 345678, 2);
IF NOT EXISTS (SELECT 1 FROM media WHERE media_id = 4)
    INSERT INTO media (media_id, file_name, file_path, file_type, file_size, uploaded_by) VALUES (4, 'students-library.jpg', '/uploads/images/students-library.jpg', 'image/jpeg', 298765, 2);
IF NOT EXISTS (SELECT 1 FROM media WHERE media_id = 5)
    INSERT INTO media (media_id, file_name, file_path, file_type, file_size, uploaded_by) VALUES (5, 'graduation-2025.jpg', '/uploads/images/graduation-2025.jpg', 'image/jpeg', 456789, 3);
SET IDENTITY_INSERT media OFF;
GO

-- Insert Categories
SET IDENTITY_INSERT categories ON;
IF NOT EXISTS (SELECT 1 FROM categories WHERE category_id = 1)
    INSERT INTO categories (category_id, name, slug) VALUES (1, 'Campus News', 'campus-news');
IF NOT EXISTS (SELECT 1 FROM categories WHERE category_id = 2)
    INSERT INTO categories (category_id, name, slug) VALUES (2, 'Academic', 'academic');
IF NOT EXISTS (SELECT 1 FROM categories WHERE category_id = 3)
    INSERT INTO categories (category_id, name, slug) VALUES (3, 'Events', 'events');
IF NOT EXISTS (SELECT 1 FROM categories WHERE category_id = 4)
    INSERT INTO categories (category_id, name, slug) VALUES (4, 'Announcements', 'announcements');
IF NOT EXISTS (SELECT 1 FROM categories WHERE category_id = 5)
    INSERT INTO categories (category_id, name, slug) VALUES (5, 'Student Life', 'student-life');
IF NOT EXISTS (SELECT 1 FROM categories WHERE category_id = 6)
    INSERT INTO categories (category_id, name, slug) VALUES (6, 'Research', 'research');
SET IDENTITY_INSERT categories OFF;
GO

-- Insert Posts
SET IDENTITY_INSERT posts ON;
IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = 1)
    INSERT INTO posts (post_id, title, slug, content, excerpt, featured_image_id, status, author_id, published_at, created_at) VALUES
    (1, 'Welcome to New Academic Year 2025/2026', 'welcome-academic-year-2025-2026',
     '<p>We are excited to welcome all students to the new academic year. This year brings new opportunities and challenges.</p><p>Registration begins March 15, 2026.</p>',
     'Welcome message for the new academic year starting March 2026',
     1, 'published', 3, '2026-03-01 09:00:00', '2026-02-28 10:00:00');
IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = 2)
    INSERT INTO posts (post_id, title, slug, content, excerpt, featured_image_id, status, author_id, published_at, created_at) VALUES
    (2, 'Campus Library Renovation Completed', 'library-renovation-completed',
     '<p>Our state-of-the-art library renovation has been completed, featuring modern study spaces and digital resources.</p>',
     'The campus library has undergone a major renovation with new facilities',
     4, 'published', 3, '2026-02-15 14:00:00', '2026-02-14 11:00:00');
IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = 3)
    INSERT INTO posts (post_id, title, slug, content, excerpt, featured_image_id, status, author_id, published_at, created_at) VALUES
    (3, 'Upcoming Theological Symposium 2026', 'theological-symposium-2026',
     '<p>Join us for our annual Theological Symposium on March 25-27, 2026. Distinguished speakers from around the world will participate.</p>',
     'Annual theological symposium announcement with international speakers',
     2, 'published', 5, '2026-02-20 10:00:00', '2026-02-19 09:00:00');
IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = 4)
    INSERT INTO posts (post_id, title, slug, content, excerpt, featured_image_id, status, author_id, published_at, created_at) VALUES
    (4, 'Student Research Excellence Awards', 'student-research-awards',
     '<p>Congratulations to our students who received excellence awards for their outstanding research contributions.</p>',
     'Students honored for their exceptional research work',
     3, 'published', 3, '2026-03-05 15:00:00', '2026-03-04 12:00:00');
IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = 5)
    INSERT INTO posts (post_id, title, slug, content, excerpt, featured_image_id, status, author_id, published_at, created_at) VALUES
    (5, 'Registration for Strata 2 Program Now Open', 'strata-2-registration-open',
     '<p>Applications for our Strata 2 (Master''s) program are now being accepted. Deadline is April 30, 2026.</p>',
     'Master''s program registration announcement for 2026 intake',
     5, 'published', 5, '2026-03-08 08:00:00', '2026-03-07 14:00:00');
SET IDENTITY_INSERT posts OFF;
GO

-- Insert Post Categories
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 1 AND category_id = 1) INSERT INTO post_categories VALUES (1, 1);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 1 AND category_id = 4) INSERT INTO post_categories VALUES (1, 4);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 2 AND category_id = 1) INSERT INTO post_categories VALUES (2, 1);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 2 AND category_id = 5) INSERT INTO post_categories VALUES (2, 5);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 3 AND category_id = 3) INSERT INTO post_categories VALUES (3, 3);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 3 AND category_id = 2) INSERT INTO post_categories VALUES (3, 2);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 4 AND category_id = 5) INSERT INTO post_categories VALUES (4, 5);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 4 AND category_id = 6) INSERT INTO post_categories VALUES (4, 6);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 5 AND category_id = 4) INSERT INTO post_categories VALUES (5, 4);
IF NOT EXISTS (SELECT 1 FROM post_categories WHERE post_id = 5 AND category_id = 2) INSERT INTO post_categories VALUES (5, 2);
GO

-- Insert Menus
SET IDENTITY_INSERT menus ON;
IF NOT EXISTS (SELECT 1 FROM menus WHERE menu_id = 1)
    INSERT INTO menus (menu_id, name) VALUES (1, 'Main Navigation');
IF NOT EXISTS (SELECT 1 FROM menus WHERE menu_id = 2)
    INSERT INTO menus (menu_id, name) VALUES (2, 'Footer Menu');
IF NOT EXISTS (SELECT 1 FROM menus WHERE menu_id = 3)
    INSERT INTO menus (menu_id, name) VALUES (3, 'Quick Links');
SET IDENTITY_INSERT menus OFF;
GO

-- Insert Menu Items (Main Navigation)
SET IDENTITY_INSERT menu_items ON;
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 1)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (1, 1, 'Home', '/', NULL, 1);
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 2)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (2, 1, 'About', '/about', NULL, 2);
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 3)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (3, 1, 'Programs', '/programs', NULL, 3);
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 4)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (4, 1, 'Admission', '/admission', NULL, 4);
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 5)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (5, 1, 'News', '/news', NULL, 5);
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 6)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (6, 1, 'Contact', '/contact', NULL, 6);
-- Insert submenu items
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 7)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (7, 1, 'Strata 1', '/programs/strata-1', 3, 1);
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 8)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (8, 1, 'Strata 2', '/programs/strata-2', 3, 2);
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 9)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (9, 1, 'Online Registration', '/admission/online', 4, 1);
IF NOT EXISTS (SELECT 1 FROM menu_items WHERE menu_item_id = 10)
    INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES (10, 1, 'Manual Registration', '/admission/manual', 4, 2);
SET IDENTITY_INSERT menu_items OFF;
GO

-- Insert Contact Messages
INSERT INTO contact_messages (name, email, subject, message_text, status) VALUES
('John Doe', 'john.doe@email.com', 'Inquiry about Admission', 'I would like to know more about the admission process for Strata 1 program.', 'new'),
('Jane Smith', 'jane.smith@email.com', 'Program Information', 'Can you provide more details about the Theology program?', 'new'),
('Michael Brown', 'michael.brown@email.com', 'Campus Tour Request', 'I would like to schedule a campus tour. When are you available?', 'read');
GO

-- Insert Audit Logs
INSERT INTO audit_logs (user_id, action, entity_type, entity_id, details) VALUES
(1, 'created', 'page', 1, 'Created page: Home'),
(3, 'published', 'post', 1, 'Published article: Welcome to New Academic Year 2025/2026'),
(2, 'uploaded', 'media', 3, 'Uploaded file: campus-building.jpg'),
(5, 'created', 'post', 3, 'Created article: Upcoming Theological Symposium 2026'),
(3, 'updated', 'page', 2, 'Updated page: About Us');
GO

-- Insert System Logs
INSERT INTO system_logs (level, log_message, context) VALUES
('info', 'Application started successfully', '{"version": "1.0.0", "environment": "production"}'),
('warning', 'High memory usage detected', '{"memory_usage": "85%", "threshold": "80%"}'),
('info', 'Database backup completed', '{"backup_size": "245MB", "duration": "12s"}'),
('error', 'Failed to send email notification', '{"recipient": "test@example.com", "error": "SMTP connection timeout"}');
GO

-- Insert Page Views
INSERT INTO page_views (page_id, visitor_ip, user_agent, viewed_at) VALUES
(1, '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)', '2026-03-10 08:15:00'),
(1, '192.168.1.101', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)', '2026-03-10 08:30:00'),
(2, '192.168.1.102', 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X)', '2026-03-10 09:00:00'),
(3, '192.168.1.103', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)', '2026-03-10 09:45:00'),
(1, '192.168.1.104', 'Mozilla/5.0 (X11; Linux x86_64)', '2026-03-10 10:00:00');
GO

-- Insert Search Index
INSERT INTO search_index (entity_type, entity_id, title, content, updated_at) VALUES
('page', 1, 'Home', 'Welcome to STTB Sekolah Tinggi Teologi Bethel premier theological institution', GETDATE()),
('page', 2, 'About Us', 'About STTB Founded 1950 quality theological education', GETDATE()),
('post', 1, 'Welcome to New Academic Year 2025/2026', 'excited welcome students academic year opportunities registration', GETDATE()),
('post', 2, 'Campus Library Renovation Completed', 'library renovation completed modern study spaces digital resources', GETDATE()),
('post', 3, 'Upcoming Theological Symposium 2026', 'Theological Symposium speakers participate', GETDATE());
GO

-- Insert Study Programs / Akademik
IF NOT EXISTS (SELECT 1 FROM study_programs WHERE slug = 'sarjana-teologi')
BEGIN
    INSERT INTO study_programs
    (program_name, degree_level, degree_title, total_credits, study_duration, description, slug)
    VALUES
    ('Sarjana Teologi', 'S1', 'S.Th', 148, '4 Tahun', 'Program Sarjana Teologi fokus pada pelayanan gereja dan studi teologi', 'sarjana-teologi'),
    ('Sarjana Pendidikan Kristen', 'S1', 'S.Pd', 145, '4 Tahun', 'Program pendidikan Kristen untuk menghasilkan pendidik Kristen', 'sarjana-pendidikan-kristen'),
    ('Magister Teologi Pelayanan Pastoral Gereja Urban', 'S2', 'M.Th', 56, '2 Tahun', 'Program Magister Teologi dengan fokus pelayanan pastoral gereja urban', 'magister-teologi-pastoral-urban'),
    ('Magister Teologi Transformasi Budaya Masyarakat','S2','M.Th',56,'2 Tahun','Program magister teologi yang berfokus pada transformasi budaya dan masyarakat', 'magister-teologi-transformasi-budaya'),
    ('Magister Pendidikan Agama Kristen','S2','M.Pd',60,'2 Tahun','Program magister untuk pengembangan kepemimpinan dan pendidikan Kristen', 'magister-pendidikan-agama-kristen'),
    ('Magister Ministri Marketplace','S2','M.Min',54,'2 Tahun','Program magister ministri dengan fokus pelayanan di dunia kerja dan marketplace', 'magister-ministri-marketplace'),
    ('Magister Ministri Kepemimpinan Pastoral','S2','M.Min',45,'2 Tahun','Program kepemimpinan pastoral bagi pemimpin gereja', 'magister-ministri-kepemimpinan-pastoral'),
    ('Magister Ministri Teologi Pelayanan Gerejawi','S2','M.Min',65,'2.5 Tahun','Program ministri untuk pengembangan pelayanan gereja secara komprehensif', 'magister-ministri-teologi-pelayanan-gerejawi');
END
GO

-- Insert Category Course
IF NOT EXISTS (SELECT 1 FROM course_categories WHERE category_name = 'Dasar Umum')
BEGIN
    INSERT INTO course_categories (category_name, description) VALUES
    -- S1 Categories
    ('Dasar Umum','Mata kuliah dasar umum'),
    ('Studi Biblika','Studi Alkitab dan bahasa Alkitab'),
    ('Studi Teologi','Doktrin dan teologi sistematika'),
    ('Sejarah & Budaya','Sejarah gereja dan fenomenologi agama'),
    ('Praktika','Pelayanan dan praktik gereja'),
    ('Konsentrasi','Mata kuliah konsentrasi program'),
    ('Praktik Lapangan','Praktik pelayanan lapangan'),

    -- S2 Categories
    ('Mata Kuliah Fondasi','Mata kuliah dasar yang memberikan landasan teologis, metodologis, dan akademik bagi mahasiswa program S2 sebelum memasuki studi lanjutan.'),
    ('Fondasi Biblika','Mata kuliah yang membangun pemahaman mendalam tentang Alkitab, termasuk hermeneutika, latar belakang historis, dan metode penafsiran biblika.'),
    ('Fondasi Sistematika - Historika','Mata kuliah yang membahas perkembangan doktrin Kristen secara sistematis dan historis, termasuk pemikiran teologi sepanjang sejarah gereja.'),
    ('Mata Kuliah Inti','Mata kuliah inti program S2'),
    ('Mata Kuliah Konsentrasi','Konsentrasi S2'),
    ('Mata Kuliah Elektif','Mata kuliah pilihan'),
    ('Mata Kuliah Penelitian','Mata kuliah yang mempersiapkan mahasiswa dalam metodologi penelitian teologi, penulisan akademik, serta pengembangan proposal penelitian.'),
    ('Penelitian & Tugas Akhir','Penelitian program S2'),
    ('Tugas Akhir','Karya ilmiah akhir berupa tesis atau penelitian akademik yang menunjukkan kemampuan analisis teologis dan metodologi penelitian mahasiswa.'),
    ('Mentoring','Mentoring akademik dan spiritual');
END
GO

-- Insert Program Course Categories
IF NOT EXISTS (SELECT 1 FROM program_course_categories WHERE program_id = 1 AND category_id = 1)
BEGIN
    INSERT INTO program_course_categories
    (program_id, category_id, total_credits)
    VALUES
    -- Program_id 1 = Sarjana Teologi
    -- Category_id 1 = Dasar Umum
    (1,1,14),
    (1,2,34),
    (1,3,23),
    (1,4,11),
    (1,5,42),
    (1,6,9),
    (1,7,9),
    (1,8,6),

    -- Program_id 2 = Sarjana Pendidikan Kristen
    (2,1,12),
    (2,2,29),
    (2,3,20),
    (2,9,65),
    (2,7,9),
    (2,8,7),

    -- Program_id 3 = Magister Teologi Pelayanan Pastoral Gereja Urban
    (3,10,15),
    (3,11,18),
    (3,12,6),
    (3,13,15),
    (3,14,2),


    -- ============================================
    -- Magister Teologi Transformasi Budaya Masyarakat
    -- ============================================

    (4,11,15), -- Mata Kuliah Inti
    (4,12,18), -- Mata Kuliah Konsentrasi
    (4,13,6),  -- Mata Kuliah Elektif
    (4,15,15), -- Penelitian & Tugas Akhir
    (4,17,2),  -- Mentoring


    -- ============================================
    -- Magister Pendidikan Kristen
    -- ============================================

    (5,8,12),  -- Mata Kuliah Fondasi
    (5,11,16), -- Mata Kuliah Inti
    (5,12,9),  -- Konsentrasi Desain Kurikulum
    (5,12,9),  -- Konsentrasi Kepemimpinan Pendidikan
    (5,13,6),  -- Mata Kuliah Elektif
    (5,14,17), -- Mata Kuliah Penelitian


    -- ============================================
    -- Magister Ministri Marketplace
    -- ============================================

    (6,9,9),   -- Fondasi Biblika
    (6,10,12), -- Fondasi Sistematika - Historika
    (6,11,12), -- Mata Kuliah Inti
    (6,12,6),  -- Mata Kuliah Konsentrasi
    (6,13,6),  -- Mata Kuliah Elektif
    (6,14,9),  -- Mata Kuliah Penelitian


    -- ============================================
    -- Magister Ministri Kepemimpinan Pastoral
    -- ============================================

    (7,9,9),   -- Fondasi Biblika
    (7,11,15), -- Mata Kuliah Inti
    (7,12,9),  -- Mata Kuliah Konsentrasi
    (7,13,3),  -- Mata Kuliah Elektif
    (7,16,9),  -- Tugas Akhir


    -- ============================================
    -- Magister Ministri Teologi Pelayanan Gerejawi
    -- ============================================
    (8,9,9),   -- Fondasi Biblika
    (8,10,12), -- Fondasi Sistematika - Historika
    (8,11,12), -- Mata Kuliah Inti
    (8,12,22), -- Mata Kuliah Konsentrasi
    (8,16,10); -- Tugas Akhir
END
GO

-- Insert Course / Mata Kuliah
IF NOT EXISTS (SELECT 1 FROM courses WHERE course_name = 'Pancasila dan Kewarganegaraan')
BEGIN
    INSERT INTO courses (course_name, description) VALUES
('Pancasila dan Kewarganegaraan','Mata kuliah ini membentuk pemikiran kebangsaan yang didorong iman Kristen dalam bidang kewiraan dan membahas Pancasila sebagai falsafah hidup, asas kehidupan bermasyarakat, berbangsa dan bernegara serta penghayatannya dalam iman Kristen.'),
('Bahasa Indonesia','Mata kuliah ini memperlengkapi mahasiswa dengan keterampilan membaca seperti menemukan ide pokok, skimming, scanning, summarizing dan reading comprehension.'),
('Bahasa Inggris Teologi','Mata kuliah ini mempelajari tata bahasa Inggris dasar serta penerapannya pada bacaan dan kosa kata teologis melalui empat keterampilan bahasa: mendengar, berbicara, membaca dan menulis.'),
('Metode Berpikir','Mata kuliah ini mempelajari konsep dasar logika dan berpikir kritis sehingga mahasiswa mampu menghasilkan pemikiran kritis berdasarkan data yang akurat.'),
('Psikologi Perkembangan Masa Hidup','Mata kuliah ini mempelajari konsep dasar psikologi perkembangan termasuk teori, rentang masa hidup dan kontribusinya bagi pelayanan.'),
('Metode Penulisan & Penelitian','Mata kuliah ini memberikan dasar pemahaman dan keterampilan praktis untuk melakukan penelitian literatur termasuk merumuskan masalah dan menyusun kajian pustaka.'),


('Pengantar Alkitab dan Teologi Biblika','Studi komprehensif tentang asal-usul sejarah dan perkembangan pemikiran religius Perjanjian Lama dan Perjanjian Baru serta hubungan keduanya.'),
('Studi PL 1: Kitab Taurat','Studi komprehensif kitab Kejadian sampai Ulangan dengan penekanan aspek historis, teologis, hermeneutis dan aplikasi praktis.'),
('Studi PL 2: Kitab Sejarah','Studi komprehensif kitab Hakim-Hakim sampai Ester dengan penekanan aspek historis, teologis dan aplikasi pelayanan.'),
('Studi PL 3: Kitab Sastra','Studi kitab Ayub, Mazmur, Amsal, Pengkhotbah dan Kidung Agung dengan fokus historis, teologis dan aplikasi rohani.'),
('Studi PL 4: Kitab Nabi-nabi','Studi komprehensif kitab nabi besar dan nabi kecil dengan pendekatan historis, teologis dan eksegesis.'),
('Studi PB 1: Kitab Injil','Studi komprehensif kitab-kitab Injil dengan fokus historis, teologis dan aplikasi bagi pelayanan.'),
('Studi PB 2: Kis Para Rasul & Paulus','Studi kitab Kisah Para Rasul dan surat-surat Paulus dengan penekanan historis, teologis dan eksegesis.'),
('Studi PB 3: Surat Umum & Wahyu','Studi surat-surat umum dan kitab Wahyu dengan penekanan historis, teologis dan aplikasi pelayanan.'),
('Hermeneutika Biblika','Mata kuliah ini memperlengkapi mahasiswa dengan prinsip dan metode penafsiran Alkitab.'),
('Bahasa Ibrani','Mata kuliah yang mempelajari elemen bahasa Ibrani Alkitab dari pengenalan huruf, tata bahasa hingga latihan penerjemahan.'),
('Bahasa Yunani','Mata kuliah yang mempelajari elemen dasar bahasa Yunani Perjanjian Baru.'),
('Bahasa Yunani Lanjutan','Mata kuliah lanjutan yang menekankan kemampuan menerjemahkan teks Perjanjian Baru dalam bahasa Yunani.'),


('Prolegomena & Doktrin Alkitab','Studi sistematis mengenai pengantar teologi dan doktrin Alkitab sebagai Firman Allah.'),
('Doktrin Allah Penciptaan & Manusia','Studi tentang pribadi dan karya Allah, doktrin penciptaan serta natur manusia sebagai gambar Allah.'),
('Doktrin Kristus, Dosa & Keselamatan','Studi tentang pribadi Kristus, doktrin dosa dan keselamatan dalam perspektif Reformed.'),
('Doktrin Roh Kudus & Akhir Zaman','Studi tentang pribadi dan karya Roh Kudus serta doktrin eskatologi.'),
('Doktrin Gereja','Studi tentang esensi, natur, misi dan organisasi gereja menurut Alkitab.'),
('Apologetika','Mata kuliah yang memperlengkapi mahasiswa menjawab pertanyaan kritis tentang iman Kristen.'),
('Etika Kristen','Mata kuliah yang mempelajari prinsip-prinsip moral Alkitab dalam kehidupan manusia modern.'),


('Sejarah Gereja Dunia','Mata kuliah yang mempelajari perkembangan gereja dari masa pra-Kristus hingga gereja modern.'),
('Sejarah Gereja Indonesia','Mata kuliah yang mempelajari perkembangan gereja di Indonesia dan kontribusi tokoh-tokoh Kristen.'),
('Sejarah Teologi','Mata kuliah yang mempelajari perkembangan pemikiran teologi dari gereja mula-mula hingga masa kini.'),
('Fenomenologi Agama','Mata kuliah yang mempelajari agama sebagai fenomena sosial dan kultural.'),
('Iman & Kebudayaan','Mata kuliah yang membahas hubungan iman Kristen dengan kebudayaan.'),


('Homiletika 1','Mata kuliah dasar penyusunan khotbah ekspositori dan praktik berkhotbah.'),
('Homiletika 2','Mata kuliah lanjutan yang membahas teknik penyampaian khotbah dalam konteks urban.'),
('Konseling Pastoral - Dasar','Mata kuliah yang mempelajari prinsip dasar konseling pastoral dan keterampilan awal konseling.'),
('Konseling Pastoral - Pastoral Issues','Mata kuliah yang membahas strategi penanganan kasus konseling pastoral masa kini.'),
('Misiologi','Mata kuliah yang mempelajari konsep misi secara teologis dan biblika.'),
('Pelayanan Penggembalaan','Mata kuliah yang membahas prinsip pastoral dalam pelayanan gereja.'),
('Kepemimpinan Kristen & Manajemen Gereja','Mata kuliah tentang kepemimpinan Alkitabiah dan manajemen gereja.'),


('Artikel Jurnal','Mata kuliah yang membimbing mahasiswa menghasilkan penelitian ilmiah dalam konteks pelayanan gerejawi.'),
('Proyek: Merancang Program Pembinaan','Mata kuliah yang membimbing mahasiswa merancang program pembinaan iman gerejawi.'),


('Teologi Reformed & Injili','Mata kuliah yang mempelajari sejarah dan inti teologi reformasi.'),
('Asuhan Kristen','Mata kuliah tentang pola asuhan Kristiani dalam keluarga.'),
('Formasi Spiritualitas','Mata kuliah tentang pembentukan spiritualitas Kristen.'),


('Pelayanan Ibadah & Musik','Mata kuliah tentang sejarah dan teologi pujian jemaat.'),
('Perintisan & Pengembangan Gereja','Mata kuliah yang membahas strategi perintisan dan pertumbuhan gereja.'),
('Pelayanan Anak Transformatif','Mata kuliah tentang metode pelayanan anak yang holistik.'),
('Pelayanan Kaum Muda Transformatif','Mata kuliah tentang strategi pelayanan bagi kaum muda.'),
('Pelayanan Orang Dewasa','Mata kuliah ini membahas prinsip dan strategi pelayanan kepada orang dewasa dalam gereja dengan memperhatikan kebutuhan rohani, sosial, dan perkembangan iman mereka.'),
('Pemuridan Transformatif','Mata kuliah ini mempelajari prinsip dan praktik pemuridan yang menekankan perubahan hidup melalui pembelajaran Alkitab, mentoring, dan komunitas iman.'),
('Perancangan Kurikulum & Program Pembinaan','Mata kuliah ini membahas prinsip dan langkah-langkah dalam merancang kurikulum serta program pembinaan rohani yang efektif bagi gereja.'),
('Media Pembelajaran & Teknologi Pendidikan','Mata kuliah ini membahas penggunaan media dan teknologi dalam proses pembelajaran untuk meningkatkan efektivitas pendidikan.'),


('Gereja & Pengembangan Masyarakat','Mata kuliah ini membahas peran gereja dalam pengembangan masyarakat melalui pelayanan sosial, pemberdayaan, dan transformasi komunitas.'),
('Mobilisasi Misi','Mata kuliah ini mempelajari strategi dan prinsip mobilisasi jemaat untuk terlibat secara aktif dalam misi gereja baik lokal maupun global.'),
('Perancangan Kurikulum Pemuridan di Gereja','Mata kuliah ini membahas prinsip dan metode dalam merancang kurikulum pemuridan yang sistematis dan kontekstual di gereja.'),


('Spiritualitas Anak','Mata kuliah ini membahas perkembangan spiritual anak serta pendekatan pembinaan iman yang sesuai dengan tahap perkembangan mereka.'),
('Perancangan Pelayanan Anak Urban','Mata kuliah ini mempelajari strategi merancang pelayanan anak dalam konteks perkotaan yang relevan dan transformatif.'),
('Pendidikan Anak Integral','Mata kuliah ini membahas pendidikan anak secara holistik yang mencakup aspek spiritual, intelektual, emosional, dan sosial.'),


('Praktik Pelayanan Media 1','Mata kuliah praktik yang melatih mahasiswa menggunakan media komunikasi dalam pelayanan gerejawi.'),
('Praktik Pelayanan Media 2','Mata kuliah lanjutan yang memperdalam keterampilan penggunaan media dalam pelayanan gerejawi.'),
('Praktik Pelayanan Akhir Pekan 1','Mata kuliah praktik pelayanan yang memberikan pengalaman langsung dalam kegiatan gereja selama akhir pekan.'),
('Praktik Pelayanan Akhir Pekan 2','Mata kuliah praktik lanjutan dalam pelayanan gereja selama akhir pekan.'),
('Praktik Pelayanan Akhir Pekan 3','Mata kuliah praktik lanjutan dalam pelayanan gereja selama akhir pekan dengan tanggung jawab pelayanan yang lebih besar.'),
('Praktik Pelayanan Akhir Pekan 4','Mata kuliah praktik pelayanan gereja dengan fokus pada pengembangan kepemimpinan pelayanan.'),
('Praktik Pelayanan Akhir Pekan 5','Mata kuliah praktik pelayanan gereja dengan fokus pada integrasi teori dan praktik pelayanan.'),
('Praktik Pelayanan Misi','Mata kuliah praktik yang memberikan pengalaman langsung dalam kegiatan misi gereja.'),
('Praktik Pelayanan 2.5 Bulan','Mata kuliah praktik pelayanan yang memberikan pengalaman pelayanan intensif selama dua setengah bulan.'),
('Praktik Pelayanan 6 Bulan','Mata kuliah praktik pelayanan jangka panjang yang memberikan pengalaman pelayanan langsung di lapangan selama enam bulan.'),


('Sejarah & Filosofi Pendidikan Kristen','Mata kuliah ini mempelajari perkembangan sejarah pendidikan Kristen serta landasan filosofis yang membentuk praktik pendidikan Kristen dari masa gereja mula-mula hingga konteks modern.'),


('Teologi Asuhan Kristen','Mata kuliah ini mempelajari dasar teologis dari praktik asuhan Kristen dalam keluarga dan gereja.'),
('Konseling Pastoral 1 - Dasar Konseling','Mata kuliah ini mempelajari dasar-dasar teori dan praktik konseling pastoral.'),
('Konseling Pastoral 2 - Praktik Konseling Sekolah','Mata kuliah lanjutan yang membahas penerapan konseling pastoral dalam konteks pendidikan.'),
('Pelayanan Orang Tua Transformatif','Mata kuliah ini membahas peran orang tua dalam pembinaan iman anak dan keluarga secara transformatif.'),


('Introduksi Pendidikan Kristen','Mata kuliah ini memberikan pengantar mengenai konsep dasar, tujuan, ruang lingkup, dan peran pendidikan Kristen dalam gereja, keluarga, dan masyarakat.'),
('Psikologi Pendidikan Kristen','Mata kuliah ini membahas prinsip-prinsip psikologi yang berkaitan dengan proses belajar mengajar serta penerapannya dalam konteks pendidikan Kristen.'),
('Teologi Pendidikan Kristen','Mata kuliah ini mempelajari dasar teologis dari pendidikan Kristen yang berakar pada Alkitab serta implikasinya bagi proses pembelajaran dan pembentukan iman.'),
('Pendidikan Kristen','Mata kuliah ini mempelajari konsep dasar, tujuan, dan praktik pendidikan Kristen dalam gereja dan sekolah.'),
('Integrasi Iman & Ilmu','Mata kuliah ini membahas hubungan antara iman Kristen dan ilmu pengetahuan serta bagaimana keduanya dapat diintegrasikan dalam kehidupan akademik dan profesional.'),
('Integrasi Teologi dan Spiritualitas Anak dan Remaja','Mata kuliah ini membahas integrasi antara teologi dan pembinaan spiritual anak serta remaja.'),
('Kurikulum Pendidikan Kristen','Mata kuliah ini mempelajari prinsip dan metode pengembangan kurikulum dalam pendidikan Kristen.'),
('Perencanaan & Evaluasi Pembelajaran','Mata kuliah ini membahas perencanaan pembelajaran serta evaluasi hasil belajar secara sistematis.'),
('Strategi Pembelajaran','Mata kuliah ini mempelajari berbagai strategi pembelajaran yang efektif dalam proses pendidikan.'),
('Media & Teknologi Pembelajaran','Mata kuliah ini membahas penggunaan media dan teknologi dalam mendukung proses pembelajaran.'),
('Manajemen / Administrasi Pendidikan','Mata kuliah ini membahas prinsip-prinsip manajemen dan administrasi dalam lembaga pendidikan.'),
('Micro Teaching 1','Mata kuliah praktik mengajar yang melatih mahasiswa dalam merancang dan menyampaikan pembelajaran.'),
('Micro Teaching 2','Mata kuliah lanjutan praktik mengajar untuk meningkatkan keterampilan pedagogis mahasiswa.'),

('Pandangan Reformed tentang Peran Gereja Dalam Transformasi Masyarakat','Kuliah ini bertujuan membekali mahasiswa dengan pemahaman tentang bagaimana prinsip-prinsip Teologi Reformed dapat diaplikasikan dalam transformasi sosial, ekonomi, dan budaya masyarakat sehingga gereja dapat membawa dampak positif di tengah masyarakat.'),


('Gereja Perkotaan','Mata kuliah ini mengkaji peran, tantangan, dan peluang gereja dalam konteks perkotaan serta strategi pelayanan yang relevan untuk menjawab kebutuhan masyarakat urban.'),
('Sosiologi dan Misi Perkotaan','Mata kuliah ini membekali mahasiswa dengan pemahaman tentang naratif Alkitab dan relevansinya bagi pelayanan misi dalam konteks masyarakat perkotaan modern.'),
('Sejarah Gereja dalam Perspektif Transformasi Sosial Budaya','Mata kuliah ini membahas perjalanan sejarah gereja dari masa awal hingga kontemporer dengan menyoroti peran dan pengaruhnya dalam perubahan sosial dan budaya masyarakat.'),
('Kehidupan Spiritual seorang Gembala','Mata kuliah ini membahas pembentukan kehidupan spiritual seorang pemimpin gereja serta strategi menjaga kesehatan rohani, emosional, dan fisik dalam pelayanan.'),


('Homiletika Lanjutan','Mata kuliah ini memberikan pemahaman teoritis dan keterampilan praktis untuk berkhotbah secara Alkitabiah dengan struktur yang jelas, komunikatif, dan relevan.'),
('Pengembangan Gereja','Mata kuliah ini membekali mahasiswa dengan konsep dan strategi untuk mengembangkan gereja yang relevan dan berdampak di lingkungan masyarakat.'),
('Kepemimpinan & Manajemen Perubahan','Mata kuliah ini mempelajari konsep dan praktik kepemimpinan serta manajemen perubahan dalam organisasi untuk mencapai tujuan pelayanan secara efektif.'),
('Isu-isu Kontemporer Etika Kristen','Mata kuliah ini membahas prinsip-prinsip etika Kristen dan penerapannya dalam berbagai isu sosial dan moral kontemporer.'),
('Pelayanan Antar Generasi','Mata kuliah ini membahas cara membangun hubungan dan pelayanan yang efektif antar generasi dalam konteks gereja dan masyarakat.'),
('Konseling Pastoral','Mata kuliah ini mempersiapkan mahasiswa dengan teori, keterampilan, dan etika untuk melakukan konseling pastoral dalam pelayanan gereja.'),


('Elektif: Pelayanan Kategorial 1','Mata kuliah pilihan yang memungkinkan mahasiswa mengambil mata kuliah pelayanan kategorial dari program studi lain.'),
('Elektif: Pelayanan Kategorial 2','Mata kuliah pilihan yang memungkinkan mahasiswa mengambil mata kuliah pelayanan kategorial dari program studi lain.'),


('Penulisan Akademik','Mata kuliah ini membekali mahasiswa dengan keterampilan menulis karya ilmiah yang sistematis dan berkualitas dalam konteks akademik.'),
('Riset Praktis Dalam Pelayanan Pastoral (Kualitatif)','Mata kuliah ini mempersiapkan mahasiswa melakukan penelitian kualitatif yang relevan dengan pelayanan pastoral di gereja.'),
('Praktik Pelayanan Weekend','Mata kuliah ini memberikan pengalaman praktik pelayanan gereja secara langsung melalui kegiatan pelayanan pada akhir pekan.'),
('Praktik Pelayanan 6 Bulan / Tugas Akhir Penelitian','Mata kuliah praktik pelayanan atau penelitian yang dilakukan selama enam bulan untuk mengintegrasikan teori dan praktik pelayanan.'),


('Mentoring Akademik','Mata kuliah ini memberikan bimbingan akademik kepada mahasiswa mengenai kegiatan perkuliahan pada setiap semester.'),
('Mentoring Spiritual I-Learn','Mata kuliah ini berbentuk persekutuan mahasiswa yang bertujuan membangun pertumbuhan spiritual dan komunitas iman.'),

-- Transformasi Budaya
('Perspektif Teologi Tentang Kemiskinan','Mata kuliah ini mengeksplorasi pandangan teologi dan biblika tentang kemiskinan serta implikasinya bagi pelayanan sosial gereja.'),
('Perspektif Teologi Tentang Dunia Kerja dan Perekonomian','Mata kuliah ini membahas pandangan teologi dan biblika mengenai dunia kerja dan ekonomi serta penerapannya dalam kehidupan profesional.'),
('Perspektif Teologi Tentang Ekologi','Mata kuliah ini membahas pandangan teologi tentang tanggung jawab manusia terhadap lingkungan dan pemeliharaan ciptaan.'),
('Perspektif Teologi Tentang Keadilan dan Kekuasaan','Mata kuliah ini mengkaji konsep keadilan dan kekuasaan dari perspektif teologi dan biblika serta penerapannya dalam kehidupan sosial.'),
('Perspektif Teologi Tentang Kemajemukan','Mata kuliah ini membahas pandangan teologi Kristen terhadap pluralitas masyarakat serta upaya membangun dialog dan toleransi.'),
('Seminar Riset','Mata kuliah ini melatih mahasiswa untuk menyusun, mempresentasikan, dan mengevaluasi proposal atau hasil penelitian secara ilmiah.'),
('Proposal Tesis','Mata kuliah ini mempersiapkan mahasiswa menyusun proposal penelitian sebagai tahap awal penulisan tesis.'),
('Tesis','Mata kuliah penelitian mandiri yang menghasilkan karya ilmiah sebagai tugas akhir program studi.'),

-- Magister Pendidikan Kristen
('Fondasi Teologi Sistematika','Mata kuliah ini memberikan pengantar mengenai doktrin-doktrin dasar kekristenan dalam kerangka teologi sistematika.'),
('Fondasi Perjanjian Baru','Mata kuliah ini mempelajari kitab-kitab Perjanjian Baru dengan fokus pada latar belakang, tema utama, dan teologinya.'),
('Fondasi Perjanjian Lama','Mata kuliah ini mempelajari kitab-kitab Perjanjian Lama dengan perhatian pada sejarah, budaya, dan tema teologisnya.'),
('Teologi Natur Manusia','Mata kuliah ini membahas natur manusia secara teologis dengan pendekatan integratif antara teologi dan ilmu pengetahuan.'),
('Sejarah Filosofi dan Teologi Pendidikan Kristen','Mata kuliah ini mempelajari sejarah perkembangan pendidikan Kristen beserta filosofi dan teologi yang mendasarinya.'),
('Psikologi Perkembangan dan Pendidikan','Mata kuliah ini mempelajari teori perkembangan manusia dan implikasinya bagi praktik pendidikan Kristen.'),
('Transformasi Spiritualitas Pendidikan','Mata kuliah ini membahas perkembangan spiritual dalam konteks pendidikan dan implikasinya bagi proses pembelajaran.'),
('Pendidikan Berbasis Keluarga','Mata kuliah ini membahas prinsip pendidikan Kristen dalam keluarga serta kolaborasi keluarga, gereja, dan sekolah.'),
('Mentoring Perjalanan Studi','Mata kuliah ini membimbing mahasiswa mengevaluasi dan merencanakan perjalanan studinya melalui refleksi dan portofolio pembelajaran.'),
('Desain dan Pengembangan Kurikulum','Mata kuliah ini membahas prinsip dan proses pengembangan kurikulum pendidikan yang berlandaskan nilai Alkitabiah.'),
('Evaluasi Pembelajaran','Mata kuliah ini mempelajari metode asesmen untuk mengukur pencapaian tujuan pembelajaran.'),
('Desain Strategi dan Media Pembelajaran','Mata kuliah ini membahas berbagai strategi pembelajaran dan penggunaan media dalam proses pendidikan.'),
('Fondasi Kepemimpinan Pendidikan Kristen','Mata kuliah ini membekali mahasiswa dengan dasar teologis dan praktis kepemimpinan dalam konteks pendidikan Kristen.'),
('Manajemen Pendidikan Entrepreneurial','Mata kuliah ini membahas manajemen pendidikan dengan pendekatan kewirausahaan untuk mendukung keberlanjutan lembaga pendidikan.'),
('Pengelolaan dan Pengembangan SDM','Mata kuliah ini mempelajari strategi pengelolaan dan pengembangan sumber daya manusia dalam organisasi pendidikan.'),
('Metodologi Pendidikan Kualitatif','Mata kuliah ini mempelajari teori dan praktik penelitian kualitatif dalam konteks pendidikan Kristen.'),
('Metodologi Penelitian Proyek Kreatif','Mata kuliah ini mempelajari metode penelitian berbasis proyek kreatif dalam pengembangan pendidikan Kristen.'),

-- Market Place
('Spiritualitas Dunia Kerja','Mata kuliah ini membahas penerapan spiritualitas Kristen dalam kehidupan dan etika dunia kerja.'),
('Pemuridan Dunia Kerja','Mata kuliah ini membekali mahasiswa dengan prinsip pemuridan dan mentoring dalam konteks dunia kerja.'),
('Misi Integral Dunia Kerja','Mata kuliah ini memperkenalkan dasar biblika misi integral dalam konteks dunia kerja dan masyarakat perkotaan.'),
('Kepemimpinan Transformasi Dunia Kerja','Mata kuliah ini mempelajari prinsip kepemimpinan transformasional dalam konteks profesional dan organisasi.'),
('Teologi Kerja','Mata kuliah ini membahas pekerjaan sebagai panggilan Allah dan bagian dari misi umat percaya di dunia.'),
('Etika Kerja','Mata kuliah ini membahas prinsip etika Kristen dalam pengambilan keputusan dan perilaku di dunia kerja.'),
('Kesehatan Mental dalam Dunia Kerja','Mata kuliah ini membahas pentingnya kesehatan mental dalam lingkungan kerja serta strategi menjaga kesejahteraan psikologis.'),
('Konseling Dasar Dunia Kerja','Mata kuliah ini mempelajari keterampilan dasar konseling untuk menangani masalah psikologis yang umum di dunia kerja.'),
('Isu Kontemporer Dunia Kerja','Mata kuliah ini membahas berbagai isu aktual di dunia kerja dari perspektif teologi dan etika Kristen.'),
('Perspektif Misi Dunia','Mata kuliah ini membahas wawasan misi global dan peran gereja dalam rencana misi Allah bagi dunia.'),
('Mentoring Profesi','Mata kuliah ini memberikan pendampingan mahasiswa oleh mentor profesional untuk mengintegrasikan iman dan profesi.'),
('Proyek Tugas Akhir Marketplace','Mata kuliah ini merupakan proyek akhir berbasis dunia kerja yang mengintegrasikan teori akademik dan praktik profesional.'),

-- Kepemimpinan Pastoral
('Hermeneutika Lanjutan','Mata kuliah ini memperdalam prinsip dan metode penafsiran Alkitab dalam berbagai konteks teks.'),
('Revisit Fondasi Biblika','Mata kuliah ini meninjau kembali metanarasi Alkitab dari penciptaan hingga penebusan dalam Kristus.'),
('Revisit Fondasi Sistematika','Mata kuliah ini meninjau kembali doktrin utama dalam teologi sistematika dalam kerangka teologi Reformed.'),
('Spiritualitas Pemimpin Gereja','Mata kuliah ini membahas pembentukan spiritualitas pemimpin gereja serta implikasinya dalam pelayanan.'),
('Pemuridan Gereja','Mata kuliah ini membahas fondasi Alkitab dan strategi pemuridan dalam konteks pelayanan gereja.'),
('Misi Integral Gereja','Mata kuliah ini membahas kerangka teologis dan strategi penerapan misi integral dalam pelayanan gereja.'),
('Kepemimpinan Transformasional Gereja','Mata kuliah ini mempelajari prinsip kepemimpinan Kristen yang transformatif dalam konteks pelayanan gereja.'),
('Pembinaan Spiritualitas Fase Kehidupan','Mata kuliah ini membahas pembinaan spiritualitas manusia dari masa kanak-kanak hingga usia lanjut.'),
('Teologi Pastoral','Mata kuliah ini mengkaji dasar teologis pelayanan pastoral dan integrasinya dengan praktik pelayanan gereja.'),
('Khotbah Ekspositori Lanjutan','Mata kuliah ini memperdalam teknik penyusunan dan penyampaian khotbah ekspositori.'),
('Ibadah Transformatif','Mata kuliah ini membahas dasar biblika dan praktik merancang liturgi ibadah yang membawa transformasi rohani.'),
('Perintisan dan Pertumbuhan Gereja','Mata kuliah ini membahas prinsip teologis dan strategi praktis dalam perintisan dan pertumbuhan gereja.'),
('Manajemen Perubahan dan Konflik','Mata kuliah ini mempelajari cara memimpin perubahan organisasi dan mengelola konflik secara efektif.'),
('Mentoring Pastoral','Mata kuliah ini membimbing mahasiswa dalam pembentukan karakter dan kehidupan rohani melalui mentoring.'),
('Tugas Akhir Pastoral','Mata kuliah ini merupakan tugas akhir yang berfokus pada penelitian atau proyek kreatif dalam bidang pastoral.'),

-- Ministri Gerejawi
('Fondasi PL Taurat','Mata kuliah ini mempelajari kitab Taurat dengan fokus pada aspek historis, teologis, dan aplikatif.'),
('Fondasi PB Injil','Mata kuliah ini mempelajari kitab-kitab Injil dengan fokus pada aspek historis, teologis, dan aplikatif.'),
('Transformasi Spiritualitas','Mata kuliah ini membahas proses perubahan dan pertumbuhan spiritual dalam kehidupan pribadi dan komunitas.'),
('Pemuridan Gerejawi','Mata kuliah ini membahas prinsip dan praktik pemuridan dalam kehidupan gereja.'),
('Misi Integral Gereja','Mata kuliah ini membahas penerapan konsep misi integral dalam kehidupan dan pelayanan gereja.'),
('Kepemimpinan Transformasional','Mata kuliah ini mempelajari prinsip kepemimpinan transformasional berdasarkan nilai-nilai Alkitab.'),
('Studi PL Kitab Sejarah','Mata kuliah ini mempelajari kitab sejarah Perjanjian Lama dengan fokus historis, teologis, dan aplikatif.'),
('Studi PL Kitab Puisi dan Nabi','Mata kuliah ini mempelajari kitab puisi dan nabi dalam Perjanjian Lama secara historis dan teologis.'),
('Studi PB Para Rasul dan Surat Paulus','Mata kuliah ini mempelajari Kisah Para Rasul dan surat-surat Paulus secara historis dan teologis.'),
('Studi PB Kitab Umum dan Wahyu','Mata kuliah ini mempelajari surat-surat umum dan kitab Wahyu dengan pendekatan teologis dan eksegetis.'),
('Teologi dan Praktik Ibadah','Mata kuliah ini mengkaji aspek teologis dan praktis dari ibadah serta pengembangan liturgi gereja.'),
    ('Tugas Akhir Praktik Pelayanan','Mata kuliah ini memberikan pengalaman praktik pelayanan sebagai integrasi dari seluruh pembelajaran yang telah ditempuh.');
END
GO


-- ============================================
-- Sarjana Teologi (S.Th)
-- ============================================

IF NOT EXISTS (SELECT 1 FROM category_courses WHERE program_category_id = 1 AND course_id = 1)
BEGIN
    INSERT INTO category_courses
    (program_category_id, course_id, credits)
    VALUES

-- ============================================
-- Program Category ID 1
-- Program : Sarjana Teologi
-- Category : Dasar Umum
-- ============================================
(1,1,2),  -- Pancasila dan Kewarganegaraan
(1,2,2),  -- Bahasa Indonesia
(1,3,3),  -- Bahasa Inggris Teologi
(1,4,2),  -- Metode Berpikir
(1,5,2),  -- Psikologi Perkembangan Masa Hidup
(1,6,3),  -- Metode Penulisan & Penelitian


-- ============================================
-- Program Category ID 2
-- Program : Sarjana Teologi
-- Category : Studi Biblika
-- ============================================
(2,7,3),   -- Pengantar Alkitab dan Teologi Biblika
(2,8,3),   -- Studi PL 1: Kitab Taurat
(2,9,3),   -- Studi PL 2: Kitab Sejarah
(2,10,3),  -- Studi PL 3: Kitab Sastra
(2,11,3),  -- Studi PL 4: Kitab Nabi-nabi
(2,12,3),  -- Studi PB 1: Kitab Injil
(2,13,3),  -- Studi PB 2: Kis Para Rasul & Paulus
(2,14,3),  -- Studi PB 3: Surat Umum & Wahyu
(2,15,3),  -- Hermeneutika Biblika
(2,16,3),  -- Bahasa Ibrani
(2,17,2),  -- Bahasa Yunani
(2,18,2),  -- Bahasa Yunani Lanjutan


-- ============================================
-- Program Category ID 3
-- Program : Sarjana Teologi
-- Category : Studi Teologi
-- ============================================
(3,19,3), -- Prolegomena & Doktrin Alkitab
(3,20,3), -- Doktrin Allah Penciptaan & Manusia
(3,21,3), -- Doktrin Kristus, Dosa & Keselamatan
(3,22,3), -- Doktrin Roh Kudus & Akhir Zaman
(3,23,3), -- Doktrin Gereja
(3,24,2), -- Apologetika
(3,25,2), -- Etika Kristen
(3,40,3), -- Teologi Reformed & Injili


-- ============================================
-- Program Category ID 4
-- Program : Sarjana Teologi
-- Category : Sejarah & Budaya
-- ============================================
(4,26,2), -- Sejarah Gereja Dunia
(4,27,2), -- Sejarah Gereja Indonesia
(4,28,3), -- Sejarah Teologi
(4,29,2), -- Fenomenologi Agama
(4,30,2), -- Iman & Kebudayaan


-- ============================================
-- Program Category ID 5
-- Program : Sarjana Teologi
-- Category : Praktika
-- ============================================
(5,41,2), -- Asuhan Kristen
(5,42,2), -- Formasi Spiritualitas
(5,31,3), -- Homiletika 1
(5,32,3), -- Homiletika 2
(5,43,3), -- Pelayanan Ibadah & Musik
(5,33,2), -- Konseling Pastoral Dasar
(5,34,2), -- Konseling Pastoral Pastoral Issues
(5,35,3), -- Misiologi
(5,36,2), -- Pelayanan Penggembalaan
(5,37,2), -- Kepemimpinan Kristen & Manajemen Gereja
(5,44,2), -- Perintisan & Pengembangan Gereja
(5,45,3), -- Pelayanan Anak Transformatif
(5,46,3), -- Pelayanan Kaum Muda Transformatif
(5,47,3), -- Pelayanan Orang Dewasa
(5,48,3), -- Pemuridan Transformatif
(5,49,2), -- Perancangan Kurikulum & Program Pembinaan
(5,50,2), -- Media Pembelajaran & Teknologi Pendidikan


-- ============================================
-- Program Category ID 6
-- Program : Sarjana Teologi
-- Category : Konsentrasi
-- ============================================
(6,51,3), -- Gereja & Pengembangan Masyarakat
(6,52,3), -- Mobilisasi Misi
(6,53,3), -- Perancangan Kurikulum Pemuridan di Gereja
(6,54,3), -- Spiritualitas Anak
(6,55,3), -- Perancangan Pelayanan Anak Urban
(6,56,3), -- Pendidikan Anak Integral


-- ============================================
-- Program Category ID 7
-- Program : Sarjana Teologi
-- Category : Praktik Lapangan
-- ============================================
(7,57,0), -- Praktik Pelayanan Media 1
(7,58,0), -- Praktik Pelayanan Media 2
(7,59,0), -- Praktik Pelayanan Akhir Pekan 1
(7,60,0), -- Praktik Pelayanan Akhir Pekan 2
(7,61,0), -- Praktik Pelayanan Akhir Pekan 3
(7,62,0), -- Praktik Pelayanan Akhir Pekan 4
(7,63,0), -- Praktik Pelayanan Akhir Pekan 5
(7,64,1), -- Praktik Pelayanan Misi
(7,65,2), -- Praktik Pelayanan 2.5 Bulan
(7,66,6), -- Praktik Pelayanan 6 Bulan


-- ============================================
-- Program Category ID 8
-- Program : Sarjana Teologi
-- Category : Tugas Akhir
-- ============================================
    (8,38,3), -- Artikel Jurnal
    (8,39,3); -- Proyek Merancang Program Pembinaan
END
GO


-- ============================================
-- Sarjana Pendidikan Kristen
-- Category : Dasar Umum
-- program_category_id = 9
-- ============================================

IF NOT EXISTS (SELECT 1 FROM category_courses WHERE program_category_id = 9 AND course_id = 1)
BEGIN
    INSERT INTO category_courses
    (program_category_id, course_id, credits)
    VALUES
(9,1,2),  -- Pancasila dan Kewarganegaraan
(9,2,2),  -- Bahasa Indonesia
(9,3,3),  -- Bahasa Inggris Teologi
(9,5,2),  -- Psikologi Perkembangan Masa Hidup
(9,6,3),  -- Metode Penulisan & Penelitian


-- ============================================
-- Sarjana Pendidikan Kristen
-- Category : Studi Biblika
-- program_category_id = 10
-- ============================================
(10,8,3),   -- Studi PL 1
(10,9,3),   -- Studi PL 2
(10,10,3),  -- Studi PL 3
(10,11,3),  -- Studi PL 4
(10,12,3),  -- Studi PB 1
(10,13,3),  -- Studi PB 2
(10,14,3),  -- Studi PB 3
(10,16,3),  -- Bahasa Ibrani
(10,17,2),  -- Bahasa Yunani
(10,15,3),  -- Hermeneutika Biblika

-- ============================================
-- Sarjana Pendidikan Kristen
-- Category : Studi Teologi
-- program_category_id = 11
-- ============================================
(11,19,3), -- Prolegomena & Doktrin Alkitab
(11,20,3), -- Doktrin Allah Penciptaan & Manusia
(11,21,3), -- Doktrin Kristus
(11,22,3), -- Doktrin Roh Kudus & Akhir Zaman
(11,23,3), -- Doktrin Gereja
(11,24,2), -- Apologetika
(11,25,2), -- Etika Kristen

-- ============================================
-- Program Category ID 12
-- Program : Sarjana Pendidikan Kristen
-- Category : Studi Pendidikan
-- ============================================
(12,68,2), -- Teologi Asuhan Kristen
(12,42,2), -- Formasi Spiritualitas
(12,43,3), -- Pelayanan Ibadah & Musik
(12,31,3), -- Homiletika 1
(12,32,3), -- Homiletika 2
(12,69,2), -- Konseling Pastoral Dasar Konseling
(12,70,3), -- Konseling Pastoral Praktik Konseling Sekolah
(12,48,3), -- Pemuridan Transformatif
(12,45,3), -- Pelayanan Anak Transformatif
(12,46,3), -- Pelayanan Kaum Muda Transformatif
(12,71,3), -- Pelayanan Orang Tua Transformatif
(12,72,2), -- Introduksi Pendidikan Kristen
(12,73,3), -- Psikologi Pendidikan Kristen
(12,74,2), -- Teologi Pendidikan Kristen
(12,75,2), -- Pendidikan Kristen
(12,76,3), -- Integrasi Iman & Ilmu
(12,77,3), -- Integrasi Teologi dan Spiritualitas Anak Remaja
(12,78,3), -- Kurikulum Pendidikan Kristen
(12,79,3), -- Perencanaan & Evaluasi Pembelajaran
(12,80,3), -- Strategi Pembelajaran
(12,81,3), -- Media & Teknologi Pembelajaran
(12,82,3), -- Manajemen Administrasi Pendidikan
    (12,83,2), -- Micro Teaching 1
    (12,84,4); -- Micro Teaching 2
END
GO

-- ============================================
-- Magister Teologi Pelayanan Pastoral Gereja Urban
-- ============================================

IF NOT EXISTS (SELECT 1 FROM category_courses WHERE program_category_id = 15 AND course_id = 85)
BEGIN
    INSERT INTO category_courses
    (program_category_id, course_id, credits)
    VALUES

-- Category 15 : Mata Kuliah Inti
(15,85,3), -- Pandangan Reformed Gereja
(15,86,3), -- Gereja Perkotaan
(15,87,3), -- Sosiologi dan Misi Perkotaan
(15,88,3), -- Sejarah Gereja Transformasi Sosial
(15,89,3), -- Kehidupan Spiritual Gembala

-- Category 16 : Konsentrasi
(16,90,3), -- Homiletika Lanjutan
(16,91,3), -- Pengembangan Gereja
(16,92,3), -- Kepemimpinan & Manajemen Perubahan
(16,93,3), -- Isu Etika Kristen
(16,94,3), -- Pelayanan Antar Generasi
(16,95,3), -- Konseling Pastoral

-- Category 17 : Elektif
(17,96,3), -- Elektif Pelayanan Kategorial 1
(17,97,3), -- Elektif Pelayanan Kategorial 2

-- Category 18 : Penelitian
(18,98,3),  -- Penulisan Akademik
(18,99,3), -- Riset Praktis Pelayanan Pastoral
(18,100,3), -- Praktik Pelayanan Weekend
(18,101,6), -- Praktik Pelayanan 6 Bulan / Thesis

-- Category 19 : Mentoring
(19,102,1), -- Mentoring Akademik
(19,103,1), -- Mentoring Spiritual

-- ============================================
-- Magister Teologi Transformasi Budaya Masyarakat
-- ============================================
-- Category 20 : Mata Kuliah Inti
(20,104,3), -- Pandangan Reformed Tentang Peran Gereja Dalam Transformasi Masyarakat
(20,105,3), -- Gereja Perkotaan
(20,106,3), -- Sosiologi dan Misi Perkotaan
(20,107,3), -- Sejarah Gereja Perspektif Transformasi Sosial Budaya
(20,108,3), -- Kehidupan Spiritual Seorang Gembala

-- Category 21 : Mata Kuliah Konsentrasi
(21,109,3), -- Perspektif Teologi/Biblika Tentang Kemiskinan
(21,110,3), -- Perspektif Teologi/Biblika Dunia Kerja & Perekonomian
(21,111,3), -- Perspektif Teologi/Biblika Tentang Ekologi
(21,112,3), -- Perspektif Teologi/Biblika Tentang Keadilan & Kekuasaan
(21,113,3), -- Perspektif Teologi/Biblika Tentang Kemajemukan
(21,114,3), -- Pelayanan Antar Generasi

-- Category 22 : Mata Kuliah Elektif
(22,115,3), -- Elektif 1
(22,116,3), -- Elektif 2

-- Category 23 : Penelitian & Tugas Akhir
(23,117,3), -- Penulisan Ilmiah Akademik
(23,118,3), -- Seminar Riset
(23,119,3), -- Proposal
(23,120,6), -- Tesis

-- Category 24 : Mentoring
(24,121,1), -- Mentoring Akademik
(24,122,1), -- Mentoring Spiritual I-Learn


-- ============================================
-- Magister Pendidikan Kristen
-- ============================================
-- Category 25 : Mata Kuliah Fondasi
(25,123,3), -- Fondasi Teologi Sistematika
(25,124,3), -- Fondasi Perjanjian Baru
(25,125,3), -- Fondasi Perjanjian Lama
(25,126,3), -- Hermeneutika

-- Category 26 : Mata Kuliah Inti
(26,127,3), -- Teologi Natur Manusia
(26,128,3), -- Sejarah Filosofi & Teologi Pendidikan Kristen
(26,129,3), -- Psikologi Perkembangan & Pendidikan
(26,130,3), -- Transformasi Spiritualitas Pendidikan
(26,131,3), -- Pendidikan Berbasis Keluarga
(26,132,1), -- Mentoring Perjalanan Studi


-- Category 27 : Konsentrasi Desain Kurikulum
(27,133,3), -- Desain dan Pengembangan Kurikulum
(27,134,3), -- Evaluasi Pembelajaran
(27,135,3), -- Desain Strategi & Media Pembelajaran


-- Category 28 : Konsentrasi Kepemimpinan Pendidikan
(28,136,3), -- Fondasi Kepemimpinan Pendidikan Kristen
(28,137,3), -- Manajemen Pendidikan Entrepreneurial
(28,138,3), -- Pengelolaan & Pengembangan SDM


-- Category 29 : Mata Kuliah Elektif
(29,139,3), -- Elektif 1
(29,140,3), -- Elektif 2

-- Category 30 : Mata Kuliah Penelitian
(30,141,3), -- Penulisan Akademik
(30,142,3), -- Metodologi Pendidikan Kualitatif
(30,143,3), -- Metodologi Penelitian Proyek Kreatif
(30,144,3), -- Proposal
(30,145,8), -- Tesis / Proyek Kreatif

-- ============================================
-- Magister Ministri Marketplace
-- ============================================

-- Category 31 : Fondasi Biblika
(31,146,3), -- Fondasi Perjanjian Lama
(31,147,3), -- Fondasi Perjanjian Baru
(31,148,3), -- Hermeneutika

-- Category 32 : Fondasi Sistematika - Historika
(32,149,3), -- Allah, Alkitab dan Penciptaan
(32,150,3), -- Kristus dan Keselamatan
(32,151,3), -- Roh Kudus dan Gereja
(32,152,3), -- Gereja dalam Konteks Sosio-Historis

-- Category 33 : Mata Kuliah Inti
(33,153,3), -- Spiritualitas Dunia Kerja
(33,154,3), -- Pemuridan Dunia Kerja
(33,155,3), -- Misi Integral Dunia Kerja
(33,156,3), -- Kepemimpinan Transformasi Dunia Kerja

-- Category 34 : Mata Kuliah Konsentrasi
(34,157,3), -- Teologi Kerja
(34,158,3), -- Etika Kerja

-- Category 35 : Mata Kuliah Elektif
(35,159,3), -- Kesehatan Mental dalam Dunia Kerja
(35,160,3), -- Konseling Dasar untuk Dunia Kerja
(35,161,3), -- Isu Kontemporer Dunia Kerja
(35,162,3), -- Perspektif Misi Dunia
(35,163,3), -- Mata Kuliah Prodi S2 Lain

-- Category 36 : Penelitian / Proyek
(36,164,3), -- Mentoring Profesi
(36,165,6), -- Proyek Tugas Akhir

-- ============================================
-- Magister Ministri Kepemimpinan Pastoral
-- ============================================

-- Category 37 : Fondasi Biblika
(37,141,3), -- Hermeneutika Lanjutan
(37,142,3), -- Revisit Fondasi Biblika
(37,143,3), -- Revisit Fondasi Sistematika

-- Category 38 : Mata Kuliah Inti
(38,144,3), -- Spiritualitas Pemimpin Gereja
(38,145,3), -- Pemuridan Gereja
(38,146,3), -- Misi Integral Gereja
(38,147,3), -- Kepemimpinan Transformasional Gereja
(38,148,3), -- Pembinaan Spiritualitas Fase Kehidupan

-- Category 39 : Mata Kuliah Konsentrasi
(39,149,3), -- Teologi Pastoral
(39,150,3), -- Khotbah Ekspositori Lanjutan
(39,151,3), -- Ibadah Transformatif

-- Category 40 : Mata Kuliah Elektif
(40,152,3), -- Perintisan dan Pertumbuhan Gereja
(40,153,3), -- Manajemen Perubahan dan Konflik

-- Category 41 : Penelitian / Proyek
(41,154,3), -- Mentoring Pastoral
(41,155,6), -- Tugas Akhir Pastoral

-- ============================================
-- Magister Ministri Teologi Pelayanan Gerejawi
-- ============================================

-- Category 42 : Fondasi Biblika
(42,156,3), -- Fondasi PL Taurat
(42,157,3), -- Fondasi PB Injil
(42,158,3), -- Transformasi Spiritualitas

-- Category 43 : Fondasi Sistematika - Historika
(43,159,3), -- Pemuridan Gerejawi
(43,160,3), -- Misi Integral Gereja
(43,161,3), -- Kepemimpinan Transformasional
(43,162,3), -- Studi PL Kitab Sejarah

-- Category 44 : Mata Kuliah Inti
(44,163,3), -- Studi PL Kitab Puisi dan Nabi
(44,164,3), -- Studi PB Para Rasul dan Surat Paulus
(44,165,3), -- Studi PB Kitab Umum dan Wahyu
(44,166,3), -- Teologi dan Praktik Ibadah

-- Category 45 : Mata Kuliah Konsentrasi
(45,167,3), -- Tugas Akhir Praktik Pelayanan

-- Category 46 : Penelitian / Tugas Akhir
    (46,102,1), -- Mentoring Akademik
    (46,98,3),  -- Penulisan Akademik
    (46,167,6); -- Tugas Akhir Praktik Pelayanan
END
GO


IF NOT EXISTS (SELECT 1 FROM program_fee_categories WHERE category_name = 'Administrasi')
BEGIN
    INSERT INTO program_fee_categories (category_name) VALUES
    ('Administrasi'),
    ('Kuliah'),
    ('Lain-lain');
END
GO

IF NOT EXISTS (SELECT 1 FROM program_fees WHERE program_id = 1 AND fee_name = 'Pendaftaran & Tes Masuk')
BEGIN
    INSERT INTO program_fees
    (program_id, fee_category_id, fee_name, amount)
    VALUES
    (1,1,'Pendaftaran & Tes Masuk',500000),
    (1,1,'Administrasi Per Semester',500000),
    (1,2,'Biaya Kuliah Per Semester',9000000),
    (1,2,'Bimbingan Tugas Akhir',1500000),
    (1,3,'Wisuda',2000000),
    (1,3,'Cuti Akademik',500000),

    (2,1,'Pendaftaran & Tes Masuk',500000),
    (2,1,'Administrasi Per Semester',500000),
    (2,2,'Biaya Kuliah Per Semester',9000000),
    (2,2,'Bimbingan Tugas Akhir',1500000),
    (2,3,'Wisuda',2000000),
    (2,3,'Cuti Akademik',500000);
END
GO

-- Insert Events
SET IDENTITY_INSERT events ON;
IF NOT EXISTS (SELECT 1 FROM events WHERE event_id = 1)
    INSERT INTO events (event_id, title, slug, description, location, start_date, end_date, featured_image_id, status, created_by, updated_by, created_at, updated_at) VALUES
    (1, 'Penerimaan Mahasiswa Baru 2025', 'penerimaan-mahasiswa-baru-2025',
     'Kegiatan penerimaan mahasiswa baru tahun akademik 2025/2026. Seluruh calon mahasiswa wajib hadir.',
     'Gedung Aula STTB, Jakarta', '2025-07-14 08:00:00', '2025-07-14 16:00:00', NULL, 'published', 1, NULL, GETDATE(), NULL);
IF NOT EXISTS (SELECT 1 FROM events WHERE event_id = 2)
    INSERT INTO events (event_id, title, slug, description, location, start_date, end_date, featured_image_id, status, created_by, updated_by, created_at, updated_at) VALUES
    (2, 'Seminar Teologi: Peran Gereja di Era Modern', 'seminar-teologi-peran-gereja-era-modern',
     'Seminar teologi yang membahas peran dan tantangan gereja di era modern bersama narasumber nasional.',
     'Aula Utama STTB', '2025-08-20 09:00:00', '2025-08-20 17:00:00', NULL, 'published', 1, NULL, GETDATE(), NULL);
IF NOT EXISTS (SELECT 1 FROM events WHERE event_id = 3)
    INSERT INTO events (event_id, title, slug, description, location, start_date, end_date, featured_image_id, status, created_by, updated_by, created_at, updated_at) VALUES
    (3, 'Retreat Mahasiswa Semester Ganjil 2025', 'retreat-mahasiswa-semester-ganjil-2025',
     'Retreat rohani dan akademik mahasiswa STTB sebagai pembuka tahun akademik baru.',
     'Wisma Bethel, Puncak, Jawa Barat', '2025-09-05 07:00:00', '2025-09-07 18:00:00', NULL, 'published', 2, NULL, GETDATE(), NULL);
IF NOT EXISTS (SELECT 1 FROM events WHERE event_id = 4)
    INSERT INTO events (event_id, title, slug, description, location, start_date, end_date, featured_image_id, status, created_by, updated_by, created_at, updated_at) VALUES
    (4, 'Wisuda Angkatan XIV', 'wisuda-angkatan-xiv',
     'Upacara wisuda angkatan XIV Sekolah Tinggi Teologi Bethel untuk program S1 dan S2.',
     'Gedung Serbaguna STTB, Jakarta', '2025-11-29 09:00:00', '2025-11-29 14:00:00', NULL, 'published', 1, NULL, GETDATE(), NULL);
IF NOT EXISTS (SELECT 1 FROM events WHERE event_id = 5)
    INSERT INTO events (event_id, title, slug, description, location, start_date, end_date, featured_image_id, status, created_by, updated_by, created_at, updated_at) VALUES
    (5, 'Workshop Pelayanan Musik Gereja', 'workshop-pelayanan-musik-gereja',
     'Workshop intensif sehari untuk meningkatkan kualitas pelayanan musik dalam konteks ibadah gereja.',
     'Ruang Serbaguna STTB Lantai 2', '2025-10-11 09:00:00', '2025-10-11 16:00:00', NULL, 'draft', 2, NULL, GETDATE(), NULL);
SET IDENTITY_INSERT events OFF;
GO

-- ============================================
-- CREATE INDEXES FOR PERFORMANCE
-- ============================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_users_email' AND object_id = OBJECT_ID('users'))
    CREATE INDEX idx_users_email ON users(email);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_users_role_id' AND object_id = OBJECT_ID('users'))
    CREATE INDEX idx_users_role_id ON users(role_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pages_slug' AND object_id = OBJECT_ID('pages'))
    CREATE INDEX idx_pages_slug ON pages(slug);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pages_status' AND object_id = OBJECT_ID('pages'))
    CREATE INDEX idx_pages_status ON pages(status);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_posts_slug' AND object_id = OBJECT_ID('posts'))
    CREATE INDEX idx_posts_slug ON posts(slug);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_posts_status' AND object_id = OBJECT_ID('posts'))
    CREATE INDEX idx_posts_status ON posts(status);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_posts_author_id' AND object_id = OBJECT_ID('posts'))
    CREATE INDEX idx_posts_author_id ON posts(author_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_posts_published_at' AND object_id = OBJECT_ID('posts'))
    CREATE INDEX idx_posts_published_at ON posts(published_at);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_audit_logs_user_id' AND object_id = OBJECT_ID('audit_logs'))
    CREATE INDEX idx_audit_logs_user_id ON audit_logs(user_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_audit_logs_entity' AND object_id = OBJECT_ID('audit_logs'))
    CREATE INDEX idx_audit_logs_entity ON audit_logs(entity_type, entity_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_page_views_page_id' AND object_id = OBJECT_ID('page_views'))
    CREATE INDEX idx_page_views_page_id ON page_views(page_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_search_index_entity' AND object_id = OBJECT_ID('search_index'))
    CREATE INDEX idx_search_index_entity ON search_index(entity_type, entity_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_contact_messages_status' AND object_id = OBJECT_ID('contact_messages'))
    CREATE INDEX idx_contact_messages_status ON contact_messages(status);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_study_programs_degree_level' AND object_id = OBJECT_ID('study_programs'))
    CREATE INDEX idx_study_programs_degree_level ON study_programs(degree_level);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_course_categories_name' AND object_id = OBJECT_ID('course_categories'))
    CREATE INDEX idx_course_categories_name ON course_categories(category_name);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_program_course_categories_program' AND object_id = OBJECT_ID('program_course_categories'))
    CREATE INDEX idx_program_course_categories_program ON program_course_categories(program_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_program_course_categories_category' AND object_id = OBJECT_ID('program_course_categories'))
    CREATE INDEX idx_program_course_categories_category ON program_course_categories(category_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_courses_name' AND object_id = OBJECT_ID('courses'))
    CREATE INDEX idx_courses_name ON courses(course_name);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_category_courses_program_category' AND object_id = OBJECT_ID('category_courses'))
    CREATE INDEX idx_category_courses_program_category ON category_courses(program_category_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_category_courses_course' AND object_id = OBJECT_ID('category_courses'))
    CREATE INDEX idx_category_courses_course ON category_courses(course_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_program_fees_program' AND object_id = OBJECT_ID('program_fees'))
    CREATE INDEX idx_program_fees_program ON program_fees(program_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_program_fees_category' AND object_id = OBJECT_ID('program_fees'))
    CREATE INDEX idx_program_fees_category ON program_fees(fee_category_id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_events_slug' AND object_id = OBJECT_ID('events'))
    CREATE INDEX idx_events_slug ON events(slug);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_events_status' AND object_id = OBJECT_ID('events'))
    CREATE INDEX idx_events_status ON events(status);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_events_start_date' AND object_id = OBJECT_ID('events'))
    CREATE INDEX idx_events_start_date ON events(start_date);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_academic_calendars_slug' AND object_id = OBJECT_ID('academic_calendars'))
    CREATE INDEX idx_academic_calendars_slug ON academic_calendars(slug);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_academic_calendars_status' AND object_id = OBJECT_ID('academic_calendars'))
    CREATE INDEX idx_academic_calendars_status ON academic_calendars(status);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_academic_calendars_start_date' AND object_id = OBJECT_ID('academic_calendars'))
    CREATE INDEX idx_academic_calendars_start_date ON academic_calendars(start_date);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_academic_calendars_academic_year' AND object_id = OBJECT_ID('academic_calendars'))
    CREATE INDEX idx_academic_calendars_academic_year ON academic_calendars(academic_year);
GO

-- ============================================
-- VERIFICATION QUERIES
-- ============================================

-- Verify data insertion
SELECT 'Roles' as table_name, COUNT(*) as record_count FROM roles
UNION ALL
SELECT 'Users', COUNT(*) FROM users
UNION ALL
SELECT 'Permissions', COUNT(*) FROM permissions
UNION ALL
SELECT 'Pages', COUNT(*) FROM pages
UNION ALL
SELECT 'Posts', COUNT(*) FROM posts
UNION ALL
SELECT 'Categories', COUNT(*) FROM categories
UNION ALL
SELECT 'Media', COUNT(*) FROM media
UNION ALL
SELECT 'Menus', COUNT(*) FROM menus
UNION ALL
SELECT 'Menu Items', COUNT(*) FROM menu_items
UNION ALL
SELECT 'Contact Messages', COUNT(*) FROM contact_messages
UNION ALL
SELECT 'Study Programs' AS table_name, COUNT(*) AS record_count FROM study_programs
UNION ALL
SELECT 'Course Categories', COUNT(*) FROM course_categories
UNION ALL
SELECT 'Program Course Categories', COUNT(*) FROM program_course_categories
UNION ALL
SELECT 'Courses', COUNT(*) FROM courses
UNION ALL
SELECT 'Category Courses', COUNT(*) FROM category_courses
UNION ALL
SELECT 'Program Fee Categories', COUNT(*) FROM program_fee_categories
UNION ALL
SELECT 'Program Fees', COUNT(*) FROM program_fees
UNION ALL
SELECT 'Events', COUNT(*) FROM events;
GO

-- ============================================
-- SELECT * FOR ALL TABLES
-- ============================================

SELECT * FROM roles;
SELECT * FROM users;
SELECT * FROM permissions;
SELECT * FROM role_permissions;
SELECT * FROM pages;
SELECT * FROM media;
SELECT * FROM posts;
SELECT * FROM categories;
SELECT * FROM post_categories;
SELECT * FROM menus;
SELECT * FROM menu_items;
SELECT * FROM contact_messages;
SELECT * FROM audit_logs;
SELECT * FROM system_logs;
SELECT * FROM page_views;
SELECT * FROM site_settings;
SELECT * FROM search_index;
SELECT * FROM study_programs;
SELECT * FROM course_categories;
SELECT * FROM program_course_categories;
SELECT * FROM courses;
SELECT * FROM category_courses;
SELECT * FROM program_fee_categories;
SELECT * FROM program_fees;
SELECT * FROM events;
SELECT * FROM academic_calendars;
GO



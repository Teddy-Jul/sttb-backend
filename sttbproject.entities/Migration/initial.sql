-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'sttbproject')
BEGIN
    CREATE DATABASE sttbproject;
END
GO

USE sttbproject;
GO

-- ============================================
-- CREATE TABLES
-- ============================================

-- 1. Roles
CREATE TABLE roles (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) UNIQUE NOT NULL,
    description NVARCHAR(MAX) NULL
);
GO

-- 2. Users
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
GO

-- 3. Permissions
CREATE TABLE permissions (
    permission_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) UNIQUE NOT NULL,
    description NVARCHAR(MAX) NULL
);
GO

-- 4. Role Permissions
CREATE TABLE role_permissions (
    role_id INT NOT NULL FOREIGN KEY REFERENCES roles(role_id) ON DELETE CASCADE,
    permission_id INT NOT NULL FOREIGN KEY REFERENCES permissions(permission_id) ON DELETE CASCADE,
    PRIMARY KEY (role_id, permission_id)
);
GO

-- 5. Pages
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
GO

-- 6. Media
CREATE TABLE media (
    media_id INT IDENTITY(1,1) PRIMARY KEY,
    file_name NVARCHAR(255) NULL,
    file_path NVARCHAR(MAX) NULL,
    file_type NVARCHAR(50) NULL,
    file_size BIGINT NULL,
    uploaded_by INT NULL FOREIGN KEY REFERENCES users(user_id),
    created_at DATETIME2 DEFAULT GETDATE()
);
GO

-- 7. Posts (Blog/News)
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
GO

-- 8. Categories
CREATE TABLE categories (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NULL,
    slug NVARCHAR(100) UNIQUE NULL
);
GO

-- 9. Post Categories
CREATE TABLE post_categories (
    post_id INT NOT NULL FOREIGN KEY REFERENCES posts(post_id) ON DELETE CASCADE,
    category_id INT NOT NULL FOREIGN KEY REFERENCES categories(category_id) ON DELETE CASCADE,
    PRIMARY KEY (post_id, category_id)
);
GO

-- 10. Menus
CREATE TABLE menus (
    menu_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NULL
);
GO

-- 11. Menu Items
CREATE TABLE menu_items (
    menu_item_id INT IDENTITY(1,1) PRIMARY KEY,
    menu_id INT NOT NULL FOREIGN KEY REFERENCES menus(menu_id) ON DELETE CASCADE,
    title NVARCHAR(100) NULL,
    url NVARCHAR(255) NULL,
    parent_id INT NULL FOREIGN KEY REFERENCES menu_items(menu_item_id),
    position INT NULL
);
GO

-- 12. Contact Messages
CREATE TABLE contact_messages (
    contact_message_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NULL,
    email NVARCHAR(150) NULL,
    subject NVARCHAR(200) NULL,
    message_text NVARCHAR(MAX) NULL,
    status NVARCHAR(20) DEFAULT 'new',
    created_at DATETIME2 DEFAULT GETDATE()
);
GO

-- 13. Audit Logs
CREATE TABLE audit_logs (
    audit_log_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NULL FOREIGN KEY REFERENCES users(user_id),
    action NVARCHAR(100) NULL,
    entity_type NVARCHAR(50) NULL,
    entity_id INT NULL,
    details NVARCHAR(MAX) NULL,
    created_at DATETIME2 DEFAULT GETDATE()
);
GO

-- 14. System Logs
CREATE TABLE system_logs (
    system_log_id INT IDENTITY(1,1) PRIMARY KEY,
    level NVARCHAR(20) NULL,
    log_message NVARCHAR(MAX) NULL,
    context NVARCHAR(MAX) NULL, -- JSON stored as NVARCHAR(MAX)
    created_at DATETIME2 DEFAULT GETDATE()
);
GO

-- 15. Page Views (Analytics)
CREATE TABLE page_views (
    page_view_id INT IDENTITY(1,1) PRIMARY KEY,
    page_id INT NULL FOREIGN KEY REFERENCES pages(page_id),
    visitor_ip NVARCHAR(50) NULL,
    user_agent NVARCHAR(MAX) NULL,
    viewed_at DATETIME2 DEFAULT GETDATE()
);
GO

-- 16. Site Settings
CREATE TABLE site_settings (
    site_setting_id INT IDENTITY(1,1) PRIMARY KEY,
    setting_key NVARCHAR(100) UNIQUE NOT NULL,
    setting_value NVARCHAR(MAX) NULL,
    updated_at DATETIME2 DEFAULT GETDATE()
);
GO

-- 17. Search Index
CREATE TABLE search_index (
    search_index_id INT IDENTITY(1,1) PRIMARY KEY,
    entity_type NVARCHAR(50) NULL,
    entity_id INT NULL,
    title NVARCHAR(MAX) NULL,
    content NVARCHAR(MAX) NULL,
    updated_at DATETIME2 NULL
);
GO

-- ============================================
-- INSERT DUMMY DATA
-- ============================================

-- Insert Roles
SET IDENTITY_INSERT roles ON;
INSERT INTO roles (role_id, name, description) VALUES
(1, 'super_admin', 'Full system access with all privileges'),
(2, 'admin', 'Administrative access to manage content'),
(3, 'editor', 'Can create, edit, and publish content'),
(4, 'content_creator', 'Can create and edit own content'),
(5, 'marketing', 'Manage promotions and events');
SET IDENTITY_INSERT roles OFF;
GO

-- Insert Permissions
SET IDENTITY_INSERT permissions ON;
INSERT INTO permissions (permission_id, name, description) VALUES
(1, 'manage_users', 'Create, update, and delete users'),
(2, 'manage_roles', 'Manage roles and permissions'),
(3, 'manage_settings', 'Update system settings'),
(4, 'view_logs', 'View audit and system logs'),
(5, 'edit_pages', 'Edit static website pages'),
(6, 'publish_content', 'Publish articles and posts'),
(7, 'manage_media', 'Upload and manage media files'),
(8, 'create_articles', 'Create blog articles'),
(9, 'edit_articles', 'Edit blog articles'),
(10, 'delete_articles', 'Delete blog articles'),
(11, 'manage_events', 'Create and manage events'),
(12, 'manage_menu', 'Manage navigation menus'),
(13, 'view_analytics', 'View website analytics');
SET IDENTITY_INSERT permissions OFF;
GO

-- Insert Role Permissions
-- Super Admin gets all permissions
INSERT INTO role_permissions (role_id, permission_id)
SELECT 1, permission_id FROM permissions;

-- Admin/Editor permissions
INSERT INTO role_permissions (role_id, permission_id) VALUES
(2, 4), (2, 5), (2, 6), (2, 7), (2, 8), (2, 9), (2, 10), (2, 11), (2, 12), (2, 13), -- admin
(3, 5), (3, 6), (3, 7), (3, 8), (3, 9), (3, 10), (3, 13); -- editor

-- Content Creator permissions
INSERT INTO role_permissions (role_id, permission_id) VALUES
(4, 7), (4, 8), (4, 9);

-- Marketing permissions
INSERT INTO role_permissions (role_id, permission_id) VALUES
(5, 7), (5, 11), (5, 13);
GO

-- Insert Users (Password: 'Password123!' - should be hashed in production)
SET IDENTITY_INSERT users ON;
INSERT INTO users (user_id, name, email, password_hash, role_id, status) VALUES
(1, 'Super Admin', 'superadmin@sttb.ac.id', '$2a$11$XGqZpPPbLhKxVjWqWqC7fOKhLWjXzJZjPVzJ3FQlT7RzJzWnZqDOy', 1, 'active'),
(2, 'Admin User', 'admin@sttb.ac.id', '$2a$11$XGqZpPPbLhKxVjWqWqC7fOKhLWjXzJZjPVzJ3FQlT7RzJzWnZqDOy', 2, 'active'),
(3, 'Editor John', 'editor@sttb.ac.id', '$2a$11$XGqZpPPbLhKxVjWqWqC7fOKhLWjXzJZjPVzJ3FQlT7RzJzWnZqDOy', 3, 'active'),
(4, 'Content Creator Jane', 'creator@sttb.ac.id', '$2a$11$XGqZpPPbLhKxVjWqWqC7fOKhLWjXzJZjPVzJ3FQlT7RzJzWnZqDOy', 4, 'active'),
(5, 'Marketing Mike', 'marketing@sttb.ac.id', '$2a$11$XGqZpPPbLhKxVjWqWqC7fOKhLWjXzJZjPVzJ3FQlT7RzJzWnZqDOy', 5, 'active');
SET IDENTITY_INSERT users OFF;
GO

-- Insert Site Settings
INSERT INTO site_settings (setting_key, setting_value) VALUES
('site_name', 'Sekolah Tinggi Teologi Bethel'),
('site_abbreviation', 'STTB'),
('site_email', 'info@sttb.ac.id'),
('contact_phone', '021-8765432'),
('contact_address', 'Jl. Teologia No. 123, Jakarta'),
('facebook_url', 'https://facebook.com/sttb'),
('instagram_url', 'https://instagram.com/sttb'),
('twitter_url', 'https://twitter.com/sttb'),
('maintenance_mode', 'false'),
('registration_open', 'true');
GO

-- Insert Pages
SET IDENTITY_INSERT pages ON;
INSERT INTO pages (page_id, title, slug, content, status, created_by, created_at) VALUES
(1, 'Home', 'home', '<h1>Welcome to STTB</h1><p>Sekolah Tinggi Teologi Bethel is a premier theological institution.</p>', 'published', 1, GETDATE()),
(2, 'About Us', 'about', '<h1>About STTB</h1><p>Founded in 1950, STTB has been providing quality theological education.</p>', 'published', 1, GETDATE()),
(3, 'Academic Programs', 'programs', '<h1>Our Programs</h1><p>We offer Strata 1 and Strata 2 programs in various theological studies.</p>', 'published', 1, GETDATE()),
(4, 'Admission', 'admission', '<h1>Admission Information</h1><p>Learn how to apply to STTB.</p>', 'published', 1, GETDATE()),
(5, 'Contact Us', 'contact', '<h1>Contact Information</h1><p>Get in touch with us.</p>', 'published', 1, GETDATE());
SET IDENTITY_INSERT pages OFF;
GO

-- Insert Media
SET IDENTITY_INSERT media ON;
INSERT INTO media (media_id, file_name, file_path, file_type, file_size, uploaded_by) VALUES
(1, 'logo.png', '/uploads/images/logo.png', 'image/png', 45678, 1),
(2, 'hero-banner.jpg', '/uploads/images/hero-banner.jpg', 'image/jpeg', 234567, 1),
(3, 'campus-building.jpg', '/uploads/images/campus-building.jpg', 'image/jpeg', 345678, 2),
(4, 'students-library.jpg', '/uploads/images/students-library.jpg', 'image/jpeg', 298765, 2),
(5, 'graduation-2025.jpg', '/uploads/images/graduation-2025.jpg', 'image/jpeg', 456789, 3);
SET IDENTITY_INSERT media OFF;
GO

-- Insert Categories
SET IDENTITY_INSERT categories ON;
INSERT INTO categories (category_id, name, slug) VALUES
(1, 'Campus News', 'campus-news'),
(2, 'Academic', 'academic'),
(3, 'Events', 'events'),
(4, 'Announcements', 'announcements'),
(5, 'Student Life', 'student-life'),
(6, 'Research', 'research');
SET IDENTITY_INSERT categories OFF;
GO

-- Insert Posts
SET IDENTITY_INSERT posts ON;
INSERT INTO posts (post_id, title, slug, content, excerpt, featured_image_id, status, author_id, published_at, created_at) VALUES
(1, 'Welcome to New Academic Year 2025/2026', 'welcome-academic-year-2025-2026', 
 '<p>We are excited to welcome all students to the new academic year. This year brings new opportunities and challenges.</p><p>Registration begins March 15, 2026.</p>', 
 'Welcome message for the new academic year starting March 2026', 
 1, 'published', 3, '2026-03-01 09:00:00', '2026-02-28 10:00:00'),

(2, 'Campus Library Renovation Completed', 'library-renovation-completed',
 '<p>Our state-of-the-art library renovation has been completed, featuring modern study spaces and digital resources.</p>',
 'The campus library has undergone a major renovation with new facilities',
 4, 'published', 3, '2026-02-15 14:00:00', '2026-02-14 11:00:00'),

(3, 'Upcoming Theological Symposium 2026', 'theological-symposium-2026',
 '<p>Join us for our annual Theological Symposium on March 25-27, 2026. Distinguished speakers from around the world will participate.</p>',
 'Annual theological symposium announcement with international speakers',
 2, 'published', 5, '2026-02-20 10:00:00', '2026-02-19 09:00:00'),

(4, 'Student Research Excellence Awards', 'student-research-awards',
 '<p>Congratulations to our students who received excellence awards for their outstanding research contributions.</p>',
 'Students honored for their exceptional research work',
 3, 'published', 3, '2026-03-05 15:00:00', '2026-03-04 12:00:00'),

(5, 'Registration for Strata 2 Program Now Open', 'strata-2-registration-open',
 '<p>Applications for our Strata 2 (Master''s) program are now being accepted. Deadline is April 30, 2026.</p>',
 'Master''s program registration announcement for 2026 intake',
 5, 'published', 5, '2026-03-08 08:00:00', '2026-03-07 14:00:00');
SET IDENTITY_INSERT posts OFF;
GO

-- Insert Post Categories
INSERT INTO post_categories (post_id, category_id) VALUES
(1, 1), (1, 4), -- Campus News, Announcements
(2, 1), (2, 5), -- Campus News, Student Life
(3, 3), (3, 2), -- Events, Academic
(4, 5), (4, 6), -- Student Life, Research
(5, 4), (5, 2); -- Announcements, Academic
GO

-- Insert Menus
SET IDENTITY_INSERT menus ON;
INSERT INTO menus (menu_id, name) VALUES
(1, 'Main Navigation'),
(2, 'Footer Menu'),
(3, 'Quick Links');
SET IDENTITY_INSERT menus OFF;
GO

-- Insert Menu Items (Main Navigation)
SET IDENTITY_INSERT menu_items ON;
INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES
(1, 1, 'Home', '/', NULL, 1),
(2, 1, 'About', '/about', NULL, 2),
(3, 1, 'Programs', '/programs', NULL, 3),
(4, 1, 'Admission', '/admission', NULL, 4),
(5, 1, 'News', '/news', NULL, 5),
(6, 1, 'Contact', '/contact', NULL, 6);

-- Insert submenu items
INSERT INTO menu_items (menu_item_id, menu_id, title, url, parent_id, position) VALUES
(7, 1, 'Strata 1', '/programs/strata-1', 3, 1),
(8, 1, 'Strata 2', '/programs/strata-2', 3, 2),
(9, 1, 'Online Registration', '/admission/online', 4, 1),
(10, 1, 'Manual Registration', '/admission/manual', 4, 2);
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

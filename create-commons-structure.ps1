# Navigate to the Commons project directory
$projectPath = "sttbproject.Commons"

# Define the structure
$folders = @(
    "Constants",
    "Extensions",
    "RequestHandlers/Authentication",
    "RequestHandlers/Pages",
    "RequestHandlers/Posts",
    "RequestHandlers/Media",
    "RequestHandlers/Categories",
    "RequestHandlers/Menus",
    "RequestHandlers/ContactMessages",
    "RequestHandlers/Users",
    "Services",
    "Validators/Authentication",
    "Validators/Pages",
    "Validators/Posts",
    "Validators/Media",
    "Validators/Categories",
    "Validators/Menus",
    "Validators/ContactMessages",
    "Validators/Users"
)

$files = @{
    "Constants" = @("ContentStatus.cs", "UserStatus.cs", "MessageStatus.cs", "PermissionNames.cs", "RoleNames.cs")
    "Extensions" = @("ServiceCollectionExtensions.cs", "StringExtensions.cs")
    "RequestHandlers/Authentication" = @("LoginRequestHandler.cs", "RegisterRequestHandler.cs", "ChangePasswordRequestHandler.cs")
    "RequestHandlers/Pages" = @("GetPageListRequestHandler.cs", "GetPageByIdRequestHandler.cs", "GetPageBySlugRequestHandler.cs", "CreatePageRequestHandler.cs", "UpdatePageRequestHandler.cs", "DeletePageRequestHandler.cs", "PublishPageRequestHandler.cs")
    "RequestHandlers/Posts" = @("GetPostListRequestHandler.cs", "GetPostByIdRequestHandler.cs", "GetPostBySlugRequestHandler.cs", "CreatePostRequestHandler.cs", "UpdatePostRequestHandler.cs", "DeletePostRequestHandler.cs", "PublishPostRequestHandler.cs")
    "RequestHandlers/Media" = @("GetMediaListRequestHandler.cs", "GetMediaByIdRequestHandler.cs", "UploadMediaRequestHandler.cs", "DeleteMediaRequestHandler.cs")
    "RequestHandlers/Categories" = @("GetCategoryListRequestHandler.cs", "CreateCategoryRequestHandler.cs", "UpdateCategoryRequestHandler.cs", "DeleteCategoryRequestHandler.cs")
    "RequestHandlers/Menus" = @("GetMenuListRequestHandler.cs", "GetMenuByIdRequestHandler.cs", "CreateMenuRequestHandler.cs", "UpdateMenuRequestHandler.cs", "DeleteMenuRequestHandler.cs")
    "RequestHandlers/ContactMessages" = @("GetContactMessageListRequestHandler.cs", "GetContactMessageByIdRequestHandler.cs", "CreateContactMessageRequestHandler.cs", "UpdateContactMessageStatusRequestHandler.cs", "DeleteContactMessageRequestHandler.cs")
    "RequestHandlers/Users" = @("GetUserListRequestHandler.cs", "GetUserByIdRequestHandler.cs", "CreateUserRequestHandler.cs", "UpdateUserRequestHandler.cs", "DeleteUserRequestHandler.cs", "UpdateUserStatusRequestHandler.cs")
    "Services" = @("IFileStorageService.cs", "FileStorageService.cs", "IEmailService.cs", "EmailService.cs", "IPasswordHashService.cs", "PasswordHashService.cs", "ISlugService.cs", "SlugService.cs")
    "Validators/Authentication" = @("LoginRequestValidator.cs", "RegisterRequestValidator.cs", "ChangePasswordRequestValidator.cs")
    "Validators/Pages" = @("CreatePageRequestValidator.cs", "UpdatePageRequestValidator.cs", "PublishPageRequestValidator.cs")
    "Validators/Posts" = @("CreatePostRequestValidator.cs", "UpdatePostRequestValidator.cs", "PublishPostRequestValidator.cs")
    "Validators/Media" = @("UploadMediaRequestValidator.cs")
    "Validators/Categories" = @("CreateCategoryRequestValidator.cs", "UpdateCategoryRequestValidator.cs")
    "Validators/Menus" = @("CreateMenuRequestValidator.cs", "UpdateMenuRequestValidator.cs")
    "Validators/ContactMessages" = @("CreateContactMessageRequestValidator.cs")
    "Validators/Users" = @("CreateUserRequestValidator.cs", "UpdateUserRequestValidator.cs", "UpdateUserStatusRequestValidator.cs")
}

# Create folders
foreach ($folder in $folders) {
    $fullPath = Join-Path $projectPath $folder
    New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
    Write-Host "Created folder: $fullPath" -ForegroundColor Green
}

# Create files with basic namespace
foreach ($folder in $files.Keys) {
    foreach ($file in $files[$folder]) {
        $fullPath = Join-Path $projectPath "$folder\$file"
        $namespace = "sttbproject.Commons." + $folder.Replace("/", ".")
        $className = [System.IO.Path]::GetFileNameWithoutExtension($file)
        
        $content = @"
namespace $namespace
{
    public class $className
    {
    }
}
"@
        
        Set-Content -Path $fullPath -Value $content -Force
        Write-Host "Created file: $fullPath" -ForegroundColor Cyan
    }
}

Write-Host "`nStructure created successfully!" -ForegroundColor Yellow
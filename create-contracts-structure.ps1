# Navigate to the Contracts project directory
$projectPath = "sttbproject.Contracts"

# Define the structure
$folders = @(
    "RequestModels/Authentication",
    "RequestModels/Pages",
    "RequestModels/Posts",
    "RequestModels/Media",
    "RequestModels/Categories",
    "RequestModels/Menus",
    "RequestModels/ContactMessages",
    "RequestModels/Users",
    "ResponseModels/Authentication",
    "ResponseModels/Pages",
    "ResponseModels/Posts",
    "ResponseModels/Media",
    "ResponseModels/Categories",
    "ResponseModels/Menus",
    "ResponseModels/ContactMessages",
    "ResponseModels/Users"
)

$files = @{
    "RequestModels/Authentication" = @("LoginRequest.cs", "RegisterRequest.cs", "ChangePasswordRequest.cs")
    "RequestModels/Pages" = @("GetPageListRequest.cs", "GetPageByIdRequest.cs", "GetPageBySlugRequest.cs", "CreatePageRequest.cs", "UpdatePageRequest.cs", "DeletePageRequest.cs", "PublishPageRequest.cs")
    "RequestModels/Posts" = @("GetPostListRequest.cs", "GetPostByIdRequest.cs", "GetPostBySlugRequest.cs", "CreatePostRequest.cs", "UpdatePostRequest.cs", "DeletePostRequest.cs", "PublishPostRequest.cs")
    "RequestModels/Media" = @("GetMediaListRequest.cs", "GetMediaByIdRequest.cs", "UploadMediaRequest.cs", "DeleteMediaRequest.cs")
    "RequestModels/Categories" = @("GetCategoryListRequest.cs", "CreateCategoryRequest.cs", "UpdateCategoryRequest.cs", "DeleteCategoryRequest.cs")
    "RequestModels/Menus" = @("GetMenuListRequest.cs", "GetMenuByIdRequest.cs", "CreateMenuRequest.cs", "UpdateMenuRequest.cs", "DeleteMenuRequest.cs")
    "RequestModels/ContactMessages" = @("GetContactMessageListRequest.cs", "GetContactMessageByIdRequest.cs", "CreateContactMessageRequest.cs", "UpdateContactMessageStatusRequest.cs", "DeleteContactMessageRequest.cs")
    "RequestModels/Users" = @("GetUserListRequest.cs", "GetUserByIdRequest.cs", "CreateUserRequest.cs", "UpdateUserRequest.cs", "DeleteUserRequest.cs", "UpdateUserStatusRequest.cs")
    "ResponseModels/Authentication" = @("LoginResponse.cs", "RegisterResponse.cs")
    "ResponseModels/Pages" = @("GetPageListResponse.cs", "PageDetailResponse.cs")
    "ResponseModels/Posts" = @("GetPostListResponse.cs", "PostDetailResponse.cs")
    "ResponseModels/Media" = @("GetMediaListResponse.cs", "MediaDetailResponse.cs", "UploadMediaResponse.cs")
    "ResponseModels/Categories" = @("GetCategoryListResponse.cs", "CategoryDetailResponse.cs")
    "ResponseModels/Menus" = @("GetMenuListResponse.cs", "MenuDetailResponse.cs")
    "ResponseModels/ContactMessages" = @("GetContactMessageListResponse.cs", "ContactMessageDetailResponse.cs")
    "ResponseModels/Users" = @("GetUserListResponse.cs", "UserDetailResponse.cs")
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
        $namespace = "sttbproject.Contracts." + $folder.Replace("/", ".")
        $className = [System.IO.Path]::GetFileNameWithoutExtension($file)
        
        $content = @"
namespace $namespace;

public class $className
{
}
"@
        
        Set-Content -Path $fullPath -Value $content -Force
        Write-Host "Created file: $fullPath" -ForegroundColor Cyan
    }
}

Write-Host "`nContracts structure created successfully!" -ForegroundColor Yellow
Write-Host "Total files created: $($files.Values | ForEach-Object { $_.Count } | Measure-Object -Sum | Select-Object -ExpandProperty Sum)" -ForegroundColor Yellow
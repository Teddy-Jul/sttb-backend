using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using sttbproject.Commons.Configuration;

namespace sttbproject.Commons.Services;

public class CloudflareR2StorageService : IFileStorageService
{
    private readonly CloudflareR2Options _options;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<CloudflareR2StorageService> _logger;

    public CloudflareR2StorageService(
        IOptions<CloudflareR2Options> options,
        IAmazonS3 s3Client,
        ILogger<CloudflareR2StorageService> logger)
    {
        _options = options.Value;
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream, 
        string fileName, 
        string contentType, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate unique file key
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var fileKey = $"media/{DateTime.UtcNow:yyyy/MM}/{uniqueFileName}";

            // Read file into memory to avoid streaming issues with R2
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            // Prepare upload request
            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey,
                InputStream = memoryStream,
                ContentType = contentType,
                DisablePayloadSigning = true, // Required for R2
                UseChunkEncoding = false, // Disable chunked encoding
                Metadata =
                {
                    ["original-filename"] = fileName,
                    ["upload-date"] = DateTime.UtcNow.ToString("O")
                }
            };

            // Upload to R2
            var response = await _s3Client.PutObjectAsync(putRequest, cancellationToken);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("File uploaded to R2 successfully: {FileKey}", fileKey);
                return fileKey;
            }

            throw new Exception($"Failed to upload file to R2. Status: {response.HttpStatusCode}");
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error uploading file: {ErrorCode}", ex.ErrorCode);
            throw new Exception($"Failed to upload file to R2: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to R2");
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = filePath
            };

            var response = await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent ||
                response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("File deleted from R2: {FilePath}", filePath);
                return true;
            }

            _logger.LogWarning("Failed to delete file from R2: {FilePath}", filePath);
            return false;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error deleting file: {ErrorCode}", ex.ErrorCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from R2");
            return false;
        }
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = filePath
            };

            var response = await _s3Client.GetObjectAsync(getRequest, cancellationToken);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error retrieving file: {ErrorCode}", ex.ErrorCode);
            throw new FileNotFoundException($"File not found in R2: {filePath}", ex);
        }
    }

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        // Return public URL for R2
        return $"{_options.PublicUrl}/{filePath}";
    }
}
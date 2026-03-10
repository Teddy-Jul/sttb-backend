namespace sttbproject.Contracts.ResponseModels.Media;

public class UploadMediaResponse
{
    public int MediaId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

namespace sttbproject.Contracts.ResponseModels.Authentication;

public class LoginResponse
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

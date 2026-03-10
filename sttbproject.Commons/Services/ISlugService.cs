namespace sttbproject.Commons.Services;

public interface ISlugService
{
    string GenerateSlug(string text);
    Task<string> GenerateUniqueSlugAsync<TEntity>(string text, Func<string, Task<bool>> existsCheck, CancellationToken cancellationToken = default);
}

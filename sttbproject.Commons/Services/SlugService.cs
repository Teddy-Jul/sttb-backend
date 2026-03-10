using sttbproject.Commons.Extensions;

namespace sttbproject.Commons.Services;

public class SlugService : ISlugService
{
    public string GenerateSlug(string text)
    {
        return text.ToSlug();
    }

    public async Task<string> GenerateUniqueSlugAsync<TEntity>(string text, Func<string, Task<bool>> existsCheck, CancellationToken cancellationToken = default)
    {
        var baseSlug = GenerateSlug(text);
        var slug = baseSlug;
        var counter = 1;

        while (await existsCheck(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }
}

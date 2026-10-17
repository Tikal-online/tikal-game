using Duende.Bff.Endpoints;

namespace BFF;

internal class FrontendHostReturnUrlValidator : IReturnUrlValidator
{
    public Task<bool> IsValidAsync(Uri returnUrl, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}
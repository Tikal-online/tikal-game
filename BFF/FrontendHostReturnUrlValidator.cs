using Duende.Bff.Endpoints;

namespace BFF;

internal class FrontendHostReturnUrlValidator : IReturnUrlValidator
{
    public bool IsValidAsync(Uri returnUrl)
    {
        return true;
    }
}
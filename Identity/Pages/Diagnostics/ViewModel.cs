using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Identity.Pages.Diagnostics;

public class ViewModel
{
    public AuthenticateResult AuthenticateResult { get; }
    public IEnumerable<string> Clients { get; }

    public ViewModel(AuthenticateResult result)
    {
        AuthenticateResult = result;

        if (result?.Properties?.Items.TryGetValue("client_list", out var encoded) == true && encoded != null)
        {
            var bytes = Base64Url.DecodeFromChars(encoded);
            var value = Encoding.UTF8.GetString(bytes);
            Clients = JsonSerializer.Deserialize<string[]>(value) ?? Enumerable.Empty<string>();
            return;
        }

        Clients = [];
    }
}
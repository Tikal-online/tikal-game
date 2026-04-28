using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account.Register;

public sealed record InputModel
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }

    public bool RememberLogin { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Button { get; set; }
}
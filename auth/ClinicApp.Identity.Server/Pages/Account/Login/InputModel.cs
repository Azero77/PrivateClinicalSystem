using System.ComponentModel.DataAnnotations;

namespace QuickStart3.Pages.Login;

public class InputModel
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public bool RememberLogin { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Button { get; set; }
}

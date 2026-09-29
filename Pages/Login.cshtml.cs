using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Taskflow.Pages;

[AllowAnonymous]
public sealed class LoginModel(IConfiguration configuration) : PageModel
{
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var expectedPassword = configuration["APP_PASSWORD"];
        if (string.IsNullOrWhiteSpace(expectedPassword))
        {
            ErrorMessage = "Morrow is not configured with an account password yet.";
            return Page();
        }

        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expectedPassword));
        var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(Password ?? ""));
        if (!CryptographicOperations.FixedTimeEquals(expectedHash, providedHash))
        {
            ErrorMessage = "That password didn’t match. Try again.";
            return Page();
        }

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "Morrow owner")],
            CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);
        return RedirectToPage("/Index");
    }
}

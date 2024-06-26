using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CarRental.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    public const string EmployeeStateClaim = nameof(EmployeeStateClaim); 

    
    public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSessionStorageResult = await _sessionStorage.GetAsync<KontoUzytkownikaModel>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

            if (userSession == null)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, userSession.Login),
                new Claim(ClaimTypes.NameIdentifier, userSession.IdKontoUzytkownika.ToString()),
                new Claim(EmployeeStateClaim, userSession.CzyPracownik.ToString())
            }, "CustomAuth"));

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public async Task LoginAsync(KontoUzytkownikaModel userSession)
    {
        await _sessionStorage.SetAsync("UserSession", userSession);
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, userSession.Login),
            new Claim(ClaimTypes.NameIdentifier, userSession.IdKontoUzytkownika.ToString()),
            new Claim(EmployeeStateClaim, userSession.CzyPracownik.ToString())
        }, "CustomAuth"));

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task LogoutAsync()
    {
        await _sessionStorage.DeleteAsync("UserSession");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}

public static class ClaimExtension
{
    public static string? ReturnClaimState(this ClaimsPrincipal user, string claimName)
    {
        return user.Claims.FirstOrDefault(x => x.Type == claimName)?.Value;
    }
}

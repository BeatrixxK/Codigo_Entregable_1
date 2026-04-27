using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DragonNutrex.Web;

public class UserSession : AuthenticationStateProvider
{
    public string NombreUsuario { get; private set; } = "";
    
    public bool IsAdmin => NombreUsuario.ToLower() == "admin@admin.com" || NombreUsuario.ToLower() == "admin"; 

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity(); 

        if (!string.IsNullOrEmpty(NombreUsuario))
        {
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, NombreUsuario),
                new Claim(ClaimTypes.Role, IsAdmin ? "Admin" : "User") 
            }, "AutenticacionDragon");
        }

        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    public void IniciarSesion(string correo)
    {
        NombreUsuario = correo;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); 
    }

    public void CerrarSesion()
    {
        NombreUsuario = "";
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); 
    }
}
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DragonNutrex.Web;

public class UserSession : AuthenticationStateProvider
{
    public string NombreUsuario { get; private set; } = "";
    
    // Reconoce el formato de correo del admin
    public bool IsAdmin => NombreUsuario.ToLower() == "admin@admin.com" || NombreUsuario.ToLower() == "admin"; 

    // Este es el método que Blazor llama automáticamente cada vez que alguien intenta entrar a una página protegida
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity(); // Por defecto: Anónimo

        if (!string.IsNullOrEmpty(NombreUsuario))
        {
            // Si hay correo, creamos una identidad oficial ("El Pasaporte")
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, NombreUsuario),
                new Claim(ClaimTypes.Role, IsAdmin ? "Admin" : "User") // Asignamos el rol
            }, "AutenticacionDragon");
        }

        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    // Nuevo método para que el Login oficialice la entrada y avise al guardia
    public void IniciarSesion(string correo)
    {
        NombreUsuario = correo;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); // 🔥 ESTO ES LO QUE FALTABA
    }

    public void CerrarSesion()
    {
        NombreUsuario = "";
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); // 🔥 AVISA QUE SALIÓ
    }
}
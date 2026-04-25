namespace DragonNutrex.Web;

public class UserSession
{
    // Propiedades de la sesión
    public string NombreUsuario { get; set; } = "";
    
    // 🔥 CORRECCIÓN: Ahora reconoce el nuevo formato de correo del admin
    public bool IsAdmin => NombreUsuario.ToLower() == "admin@admin.com" || NombreUsuario.ToLower() == "admin"; 

    public void CerrarSesion()
    {
        NombreUsuario = "";
    }
}
namespace DragonNutrex.Web;

public class UserSession
{
    // Propiedades de la sesión
    public string NombreUsuario { get; set; } = "";
    
    // Propiedad calculada: es admin si el usuario actual es "admin"
    public bool IsAdmin => NombreUsuario.ToLower() == "admin"; 

    
    public void CerrarSesion()
    {
     
        NombreUsuario = "";
        
     
    }
}
namespace DragonNutrex.Web;

public class UserSession
{
    public string NombreUsuario { get; set; } = "";
    public bool IsAdmin => NombreUsuario.ToLower() == "admin"; 
}
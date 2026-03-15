using DragonNutrex.App.Models;
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

public class UsuarioController
{
    private readonly UsuarioService _service;

    public UsuarioController(UsuarioService service)
    {
        _service = service;
    }

    public void CrearUsuario(Usuario usuario)
    {
        _service.CrearUsuario(usuario);
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        _service.ActualizarUsuario(usuario);
    }

    public void EliminarUsuario(Guid id)
    {
        _service.EliminarUsuario(id);
    }

    public List<Usuario> ObtenerUsuarios()
    {
        return _service.ObtenerUsuarios();
    }

    public Usuario? ObtenerUsuario(Guid id)
    {
        return _service.ObtenerUsuario(id);
    }
}
/*	•	validaciones
	•	lógica de negocio
	•	conectar controlador con repositorio 
    
    Método y Función
CrearUsuario()
valida y guarda
ActualizarUsuario()
valida y actualiza
EliminarUsuario()
elimina
ObtenerUsuarios()
lista usuarios
ObtenerUsuario()
busca usuario

✔ validaciones
✔ conexión con repositorio
*/

    using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

public class UsuarioService
{
    private readonly IRepository<Usuario> _repository;

    public UsuarioService(IRepository<Usuario> repository)
    {
        _repository = repository;
    }

    public void CrearUsuario(Usuario usuario)
    {
        ValidarUsuario(usuario);
        _repository.Create(usuario);
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        ValidarUsuario(usuario);
        _repository.Update(usuario);
    }

    public void EliminarUsuario(Guid id)
    {
        _repository.Delete(id);
    }

    public List<Usuario> ObtenerUsuarios()
    {
        return _repository.GetAll();
    }

    public Usuario? ObtenerUsuario(Guid id)
    {
        return _repository.GetById(id);
    }

    private void ValidarUsuario(Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nombre))
            throw new Exception("El nombre es obligatorio.");

        if (usuario.Peso <= 0)
            throw new Exception("El peso debe ser mayor que cero.");

        if (usuario.Altura <= 0)
            throw new Exception("La altura debe ser mayor que cero.");

        if (string.IsNullOrWhiteSpace(usuario.Actividad))
            throw new Exception("La actividad es obligatoria.");

        if (string.IsNullOrWhiteSpace(usuario.Objetivo))
            throw new Exception("El objetivo es obligatorio.");

        if (string.IsNullOrWhiteSpace(usuario.TipoDieta))
            throw new Exception("El tipo de dieta es obligatorio.");
    }
}
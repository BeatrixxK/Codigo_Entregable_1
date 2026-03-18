using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using DragonNutrex.App.Utils;

namespace DragonNutrex.App.Repositories;

public class UsuarioRepository : IRepository<Usuario>
{
    private readonly string _filePath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "DragonNutrex.App",
            "Data",
            "usuarios.json"
        )
    );

    public void Create(Usuario entity)
    {
        var usuarios = GetAll();
        usuarios.Add(entity);
        FileStorage.WriteList(_filePath, usuarios);
    }

    public void Update(Usuario entity)
    {
        var usuarios = GetAll();
        var index = usuarios.FindIndex(u => u.Id == entity.Id);

        if (index == -1)
            throw new Exception("Usuario no encontrado.");

        usuarios[index] = entity;
        FileStorage.WriteList(_filePath, usuarios);
    }

    public void Delete(Guid id)
    {
        var usuarios = GetAll();
        var usuario = usuarios.FirstOrDefault(u => u.Id == id);

        if (usuario == null)
            throw new Exception("Usuario no encontrado.");

        usuarios.Remove(usuario);
        FileStorage.WriteList(_filePath, usuarios);
    }

    public List<Usuario> GetAll()
    {
        return FileStorage.ReadList<Usuario>(_filePath);
    }

    public Usuario? GetById(Guid id)
    {
        return GetAll().FirstOrDefault(u => u.Id == id);
    }
}
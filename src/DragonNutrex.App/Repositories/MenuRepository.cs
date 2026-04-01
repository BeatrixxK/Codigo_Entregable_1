using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using DragonNutrex.App.Utils;

namespace DragonNutrex.App.Repositories;

public class MenuRepository : IRepository<Menu>
{
    private readonly string _filePath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "DragonNutrex.App",
            "Data",
            "menus.json"
        )
    );

    public void Create(Menu entity)
    {
        var menus = GetAll();
        menus.Add(entity);
        FileStorage.WriteList(_filePath, menus);
    }

    public void Update(Menu entity)
    {
        var menus = GetAll();
        var index = menus.FindIndex(m => m.Id == entity.Id);

        if (index == -1)
            throw new Exception("Menú no encontrado.");

        menus[index] = entity;
        FileStorage.WriteList(_filePath, menus);
    }

    public void Delete(Guid id)
    {
        var menus = GetAll();
        var menu = menus.FirstOrDefault(m => m.Id == id);

        if (menu == null)
            throw new Exception("Menú no encontrado.");

        menus.Remove(menu);
        FileStorage.WriteList(_filePath, menus);
    }

    public List<Menu> GetAll()
    {
        return FileStorage.ReadList<Menu>(_filePath);
    }

    public Menu? GetById(Guid id)
    {
        return GetAll().FirstOrDefault(m => m.Id == id);
    }
}
<<<<<<< Updated upstream
/*	Create → guarda un producto nuevo
	Update → actualiza un producto existente
	Delete → elimina un producto
	GetAll → devuelve todos los productos
	GetById → busca un producto por id*/

=======
>>>>>>> Stashed changes
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using DragonNutrex.App.Utils;

namespace DragonNutrex.App.Repositories;

public class ProductoRepository : IRepository<Producto>
{
    private readonly string _filePath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "DragonNutrex.App",
            "Data",
            "productos.json"
        )
    );

    public void Create(Producto entity)
    {
        var productos = GetAll();
        productos.Add(entity);
        FileStorage.WriteList(_filePath, productos);
    }

    public void Update(Producto entity)
    {
        var productos = GetAll();
        var index = productos.FindIndex(p => p.Id == entity.Id);

        if (index == -1)
            throw new Exception("Producto no encontrado.");

        productos[index] = entity;
        FileStorage.WriteList(_filePath, productos);
    }

    public void Delete(Guid id)
    {
        var productos = GetAll();
        var producto = productos.FirstOrDefault(p => p.Id == id);

        if (producto == null)
            throw new Exception("Producto no encontrado.");

        productos.Remove(producto);
        FileStorage.WriteList(_filePath, productos);
    }

    public List<Producto> GetAll()
    {
        return FileStorage.ReadList<Producto>(_filePath);
    }

    public Producto? GetById(Guid id)
    {
        return GetAll().FirstOrDefault(p => p.Id == id);
    }
<<<<<<< Updated upstream
}

=======
}
>>>>>>> Stashed changes

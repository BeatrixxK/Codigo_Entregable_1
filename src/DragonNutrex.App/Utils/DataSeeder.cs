using DragonNutrex.App.Models;

namespace DragonNutrex.App.Utils;

public static class DataSeeder
{
    public static void GenerarDatos()
    {
        var random = new Random();

        var usuariosPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..",
                "DragonNutrex.App",
                "Data",
                "usuarios.json"
            )
        );

        var productosPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..",
                "DragonNutrex.App",
                "Data",
                "productos.json"
            )
        );

        var menusPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..",
                "DragonNutrex.App",
                "Data",
                "menus.json"
            )
        );

        var usuarios = new List<Usuario>();
        for (int i = 1; i <= 25; i++)
        {
            usuarios.Add(new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = $"Usuario {i}",
                Peso = random.Next(50, 110),
                Altura = Math.Round((decimal)(1.45 + random.NextDouble() * 0.55), 2),
                Actividad = Actividades[random.Next(Actividades.Length)],
                Objetivo = Objetivos[random.Next(Objetivos.Length)],
                TipoDieta = Dietas[random.Next(Dietas.Length)]
            });
        }

        var productos = new List<Producto>();
        for (int i = 1; i <= 100; i++)
        {
            productos.Add(new Producto
            {
                Id = Guid.NewGuid(),
                Nombre = $"{NombresProducto[random.Next(NombresProducto.Length)]} {i}",
                Calorias = random.Next(50, 500),
                Proteinas = random.Next(0, 40),
                Carbohidratos = random.Next(0, 80),
                Grasas = random.Next(0, 25)
            });
        }

        var menus = new List<Menu>();
        var fechaInicio = DateTime.Today.AddDays(-14);

        foreach (var usuario in usuarios)
        {
            for (int i = 0; i < 15; i++)
            {
                var fecha = fechaInicio.AddDays(i);

                var menu = new Menu
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    Fecha = fecha,
                    Registros = new List<RegistroComida>()
                };

                var cantidadRegistros = random.Next(3, 6);

                for (int j = 0; j < cantidadRegistros; j++)
                {
                    var producto = productos[random.Next(productos.Count)];
                    var cantidad = random.Next(1, 5);

                    menu.Registros.Add(new RegistroComida
                    {
                        Id = Guid.NewGuid(),
                        ProductoId = producto.Id,
                        NombreProducto = producto.Nombre,
                        Cantidad = cantidad,
                        Calorias = producto.Calorias * cantidad,
                        Proteinas = producto.Proteinas * cantidad,
                        Carbohidratos = producto.Carbohidratos * cantidad,
                        Grasas = producto.Grasas * cantidad
                    });
                }

                menu.TotalCalorias = menu.Registros.Sum(r => r.Calorias);
                menu.TotalProteinas = menu.Registros.Sum(r => r.Proteinas);
                menu.TotalCarbohidratos = menu.Registros.Sum(r => r.Carbohidratos);
                menu.TotalGrasas = menu.Registros.Sum(r => r.Grasas);

                menus.Add(menu);
            }
        }

        FileStorage.WriteList(usuariosPath, usuarios);
        FileStorage.WriteList(productosPath, productos);
        FileStorage.WriteList(menusPath, menus);
    }

    private static readonly string[] Actividades =
    {
        "Sedentaria", "Ligera", "Moderada", "Alta"
    };

    private static readonly string[] Objetivos =
    {
        "Bajar peso", "Mantener", "Ganar masa", "Resistencia"
    };

    private static readonly string[] Dietas =
    {
        "Balanceada", "Vegetariana", "Vegana", "Alta proteína", "Keto"
    };

    private static readonly string[] NombresProducto =
    {
        "Arroz", "Pollo", "Avena", "Yogurt", "Banano", "Manzana", "Leche",
        "Atún", "Pan Integral", "Huevo", "Queso", "Frijoles", "Pasta",
        "Salmón", "Pechuga", "Granola", "Almendras", "Aguacate", "Papa", "Carne"
    };
}
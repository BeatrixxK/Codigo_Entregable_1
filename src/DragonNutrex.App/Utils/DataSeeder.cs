// =====================================================
// IMPORTACIONES
// =====================================================

// Modelos de la aplicación (Usuario, Producto, Menu, etc.)
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Utils;

// =====================================================
// CLASE DATA SEEDER
// =====================================================
// Genera datos de prueba (usuarios, productos y menús)
// y los guarda en archivos JSON
public static class DataSeeder
{
    // =====================================================
    // MÉTODO GENERAR DATOS
    // =====================================================
    // Crea datos aleatorios y los guarda en archivos
    public static void GenerarDatos()
    {
        // Generador de números aleatorios
        var random = new Random();

        // =====================================================
        // RUTAS DE ARCHIVOS JSON
        // =====================================================
        // Construye rutas hacia los archivos de datos

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

        // =====================================================
        // GENERACIÓN DE USUARIOS
        // =====================================================
        var usuarios = new List<Usuario>();

        // Crea 25 usuarios aleatorios
        for (int i = 1; i <= 25; i++)
        {
            usuarios.Add(new Usuario
            {
                Id = Guid.NewGuid(), // ID único
                Nombre = $"Usuario {i}", // Nombre dinámico
                Peso = random.Next(50, 110), // Peso aleatorio
                Altura = Math.Round((decimal)(1.45 + random.NextDouble() * 0.55), 2), // Altura en metros
                Actividad = Actividades[random.Next(Actividades.Length)], // Actividad aleatoria
                Objetivo = Objetivos[random.Next(Objetivos.Length)], // Objetivo aleatorio
                TipoDieta = Dietas[random.Next(Dietas.Length)], // Dieta aleatoria
                Password = "Upi.2025" // Contraseña por defecto
            });
        }

        // =====================================================
        // GENERACIÓN DE PRODUCTOS
        // =====================================================
        var productos = new List<Producto>();

        // Crea 100 productos aleatorios
        for (int i = 1; i <= 100; i++)
        {
            productos.Add(new Producto
            {
                Id = Guid.NewGuid(), // ID único
                Nombre = $"{NombresProducto[random.Next(NombresProducto.Length)]} {i}", // Nombre combinado
                Calorias = random.Next(50, 500), // Calorías
                Proteinas = random.Next(0, 40), // Proteínas
                Carbohidratos = random.Next(0, 80), // Carbohidratos
                Grasas = random.Next(0, 25) // Grasas
            });
        }

        // =====================================================
        // GENERACIÓN DE MENÚS
        // =====================================================
        var menus = new List<Menu>();

        // Fecha inicial (últimos 14 días)
        var fechaInicio = DateTime.Today.AddDays(-14);

        // Genera menús para cada usuario
        foreach (var usuario in usuarios)
        {
            for (int i = 0; i < 15; i++)
            {
                var fecha = fechaInicio.AddDays(i);

                // Crear menú
                var menu = new Menu
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    Fecha = fecha,
                    Registros = new List<RegistroComida>() // Lista de comidas
                };

                // Cantidad de comidas en el día (3 a 5)
                var cantidadRegistros = random.Next(3, 6);

                for (int j = 0; j < cantidadRegistros; j++)
                {
                    // Selecciona producto aleatorio
                    var producto = productos[random.Next(productos.Count)];
                    var cantidad = random.Next(1, 5);

                    // Agrega registro de comida
                    menu.Registros.Add(new RegistroComida
                    {
                        Id = Guid.NewGuid(),
                        ProductoId = producto.Id,
                        NombreProducto = producto.Nombre,
                        Cantidad = cantidad,

                        // Cálculo de valores nutricionales
                        Calorias = producto.Calorias * cantidad,
                        Proteinas = producto.Proteinas * cantidad,
                        Carbohidratos = producto.Carbohidratos * cantidad,
                        Grasas = producto.Grasas * cantidad
                    });
                }

                // =====================================================
                // CÁLCULO DE TOTALES DEL MENÚ
                // =====================================================
                menu.TotalCalorias = menu.Registros.Sum(r => r.Calorias);
                menu.TotalProteinas = menu.Registros.Sum(r => r.Proteinas);
                menu.TotalCarbohidratos = menu.Registros.Sum(r => r.Carbohidratos);
                menu.TotalGrasas = menu.Registros.Sum(r => r.Grasas);

                // Agrega menú a la lista
                menus.Add(menu);
            }
        }

        // =====================================================
        // GUARDADO EN ARCHIVOS JSON
        // =====================================================
        FileStorage.WriteList(usuariosPath, usuarios);
        FileStorage.WriteList(productosPath, productos);
        FileStorage.WriteList(menusPath, menus);
    }

    // =====================================================
    // DATOS BASE (CATÁLOGOS)
    // =====================================================

    // Tipos de actividad física
    private static readonly string[] Actividades =
    {
        "Sedentaria", "Ligera", "Moderada", "Alta"
    };

    // Objetivos del usuario
    private static readonly string[] Objetivos =
    {
        "Bajar peso", "Mantener", "Ganar masa", "Resistencia"
    };

    // Tipos de dieta
    private static readonly string[] Dietas =
    {
        "Balanceada", "Vegetariana", "Vegana", "Alta proteína", "Keto"
    };

    // Lista de nombres de productos base
    private static readonly string[] NombresProducto =
    {
        "Arroz", "Pollo", "Avena", "Yogurt", "Banano", "Manzana", "Leche",
        "Atún", "Pan Integral", "Huevo", "Queso", "Frijoles", "Pasta",
        "Salmón", "Pechuga", "Granola", "Almendras", "Aguacate", "Papa", "Carne"
    };
}
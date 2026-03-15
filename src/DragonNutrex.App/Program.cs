/* archivo donde empieza a ejecutarse la aplicación.*/

using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;

try
{
    var usuarioRepository = new UsuarioRepository();
    var usuarioService = new UsuarioService(usuarioRepository);
    var usuarioController = new UsuarioController(usuarioService);

    var usuario = new Usuario
    {
        Nombre = "Stephanie",
        Peso = 70,
        Altura = 1.65m,
        Actividad = "Moderada",
        Objetivo = "Mantener peso",
        TipoDieta = "Balanceada"
    };

    usuarioController.CrearUsuario(usuario);

    Console.WriteLine("Usuario creado correctamente.");

    var usuarios = usuarioController.ObtenerUsuarios();

    Console.WriteLine("\nLista de usuarios:");
    foreach (var u in usuarios)
    {
        Console.WriteLine($"{u.Nombre} - {u.Peso}kg - {u.Altura}m");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
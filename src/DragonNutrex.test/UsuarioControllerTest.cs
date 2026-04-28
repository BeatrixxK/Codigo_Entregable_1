using Xunit;
using Xunit.Abstractions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Services; // 👈 NUEVO: Necesario para instanciar UsuarioService

namespace DragonNutrex.Tests;

/// <summary>
/// Agrupa y ejecuta las pruebas unitarias diseñadas para validar el comportamiento 
/// del controlador de usuarios (UsuarioController). Utiliza el marco xUnit y 
/// la biblioteca Moq para aislar la lógica mediante la simulación de repositorios.
/// </summary>
public class UsuarioControllerTest
{
    /// <summary>
    /// Almacena la instancia del servicio de salida de xUnit, permitiendo 
    /// la inyección de mensajes estructurados en la consola de resultados de las pruebas.
    /// </summary>
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Inicializa una nueva instancia de la clase de pruebas y captura el servicio 
    /// de impresión de consola proporcionado automáticamente por la arquitectura de xUnit.
    /// </summary>
    /// <param name="output">Interfaz de xUnit para la salida de texto durante la prueba.</param>
    public UsuarioControllerTest(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Verifica de manera asincrónica que el método de obtención general del controlador 
    /// retorne exitosamente una lista poblada de usuarios. 
    /// Simula el repositorio de datos, ejecuta la petición y evalúa que el resultado 
    /// no sea nulo y contenga la cantidad exacta de registros esperados.
    /// </summary>
    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarListaDeUsuarios()
    {
        // 1. ARRANGE (Preparación)
        var usuariosSimulados = new List<Usuario>
        {
            new Usuario { Id = System.Guid.NewGuid(), Nombre = "Andrea", Correo = "andrea@test.com" },
            new Usuario { Id = System.Guid.NewGuid(), Nombre = "Tomás", Correo = "tomas@test.com" }
        };

        var mockRepo = new Mock<IRepository<Usuario>>();
        mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(usuariosSimulados);

        // 👈 CORRECCIÓN: Se instancia el servicio con el mock del repositorio, 
        // y luego se inyecta el servicio al controlador.
        var usuarioService = new UsuarioService(mockRepo.Object);
        var controller = new UsuarioController(usuarioService);

        // 2. ACT (Ejecución)
        var resultado = await controller.ObtenerTodosAsync();

        // 3. ASSERT (Validación)
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        
        // 4. IMPRIMIR EL MENSAJE FINAL
        _output.WriteLine("==================================================");
        _output.WriteLine($"✅ TEST EXITOSO: Se cargaron {resultado.Count} usuarios desde el simulador.");
        _output.WriteLine($"👤 Los usuarios '{resultado[0].Nombre}' y '{resultado[1].Nombre}' han sido validados exitosamente.");
        _output.WriteLine("==================================================");
    }
}
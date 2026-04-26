using Xunit;
using Xunit.Abstractions; // 👈 NUEVO: Librería para poder imprimir en consola
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Interfaces;

namespace DragonNutrex.Tests;

public class UsuarioControllerTest
{
    private readonly ITestOutputHelper _output; // 👈 NUEVO: Variable para guardar el impresor

    // 👈 NUEVO: Constructor. xUnit nos inyecta el impresor automáticamente aquí.
    public UsuarioControllerTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarListaDeUsuarios()
    {
        // 1. ARRANGE
        var usuariosSimulados = new List<Usuario>
        {
            new Usuario { Id = System.Guid.NewGuid(), Nombre = "Andrea", Correo = "andrea@test.com" },
            new Usuario { Id = System.Guid.NewGuid(), Nombre = "Tomás", Correo = "tomas@test.com" }
        };

        var mockRepo = new Mock<IRepository<Usuario>>();
        mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(usuariosSimulados);

        var controller = new UsuarioController(mockRepo.Object);

        // 2. ACT
        var resultado = await controller.ObtenerTodosAsync();

        // 3. ASSERT
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        
        // 4. IMPRIMIR EL MENSAJE FINAL (Esto es lo que tú querías 🚀)
        _output.WriteLine("==================================================");
        _output.WriteLine($"✅ TEST EXITOSO: Se cargaron {resultado.Count} usuarios desde el simulador.");
        _output.WriteLine($"👤 Los usuarios '{resultado[0].Nombre}' y '{resultado[1].Nombre}' han sido validados exitosamente.");
        _output.WriteLine("==================================================");
    }
}
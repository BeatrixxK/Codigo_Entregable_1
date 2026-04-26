// =====================================================
// IMPORTACIONES
// =====================================================

// Interfaz del repositorio genérico
using DragonNutrex.App.Interfaces;

// Modelo Producto
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

/// <summary>
/// Servicio de gestión integral de productos que coordina todas las operaciones de negocio relacionadas con la administración de productos nutricionales.
/// Implementa la lógica de validación y persistencia actuando como intermediario entre la capa de presentación y la capa de acceso a datos.
/// Proporciona métodos para crear, actualizar, eliminar y recuperar productos con garantías de integridad de datos.
/// </summary>
public class ProductoService
{
    /// <summary>
    /// Repositorio genérico que encapsula todas las operaciones de persistencia de datos para productos.
    /// Proporciona acceso a la base de datos mediante métodos estándar de CRUD.
    /// </summary>
    private readonly IRepository<Producto> _repository;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de productos inyectando sus dependencias necesarias.
    /// Recibe el repositorio de datos que será utilizado para todas las operaciones de persistencia durante el ciclo de vida del servicio.
    /// </summary>
    /// <param name="repository">Interfaz del repositorio genérico que proporciona los métodos de acceso y manipulación de datos de productos.</param>
    public ProductoService(IRepository<Producto> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Procesa la creación de un nuevo producto en la base de datos siguiendo el flujo establecido de validación y persistencia.
    /// Valida integralmente todos los atributos del producto antes de registrarlo en el sistema.
    /// Si alguna validación falla, detiene el proceso y comunica el error específico encontrado.
    /// </summary>
    /// <param name="producto">Objeto Producto que contiene todos los datos a registrar en el sistema.</param>
    /// <exception cref="Exception">Se lanza cuando algún atributo del producto no cumple las reglas de validación de negocio establecidas.</exception>
    public void CrearProducto(Producto producto)
    {
        ValidarProducto(producto);
        _repository.Create(producto);
    }

    /// <summary>
    /// Procesa la actualización de los datos de un producto existente en la base de datos.
    /// Ejecuta una validación exhaustiva del producto modificado antes de aplicar los cambios al repositorio.
    /// Si la validación no es exitosa, aborta la operación y proporciona información del error encontrado.
    /// </summary>
    /// <param name="producto">Objeto Producto que contiene los atributos actualizados a guardar en el repositorio.</param>
    /// <exception cref="Exception">Se lanza cuando los datos del producto no satisfacen las validaciones de integridad requeridas.</exception>
    public void ActualizarProducto(Producto producto)
    {
        ValidarProducto(producto);
        _repository.Update(producto);
    }

    /// <summary>
    /// Elimina permanentemente un producto del repositorio de datos utilizando su identificador único.
    /// Realiza la operación de eliminación de forma directa sin requerir validación previa.
    /// El producto se remove completamente de la base de datos después de esta operación.
    /// </summary>
    /// <param name="id">Identificador único (GUID) que referencia el producto a eliminar del sistema.</param>
    public void EliminarProducto(Guid id)
    {
        _repository.Delete(id);
    }

    /// <summary>
    /// Recupera el conjunto completo de todos los productos almacenados en la base de datos.
    /// Obtiene la totalidad de registros de productos disponibles en el sistema sin aplicar filtros.
    /// Devuelve una colección que puede ser iterada o procesada por el consumidor del servicio.
    /// </summary>
    /// <returns>Colección de objetos Producto que representa la totalidad de productos registrados en el sistema.</returns>
    public List<Producto> ObtenerProductos()
    {
        return _repository.GetAll();
    }

    /// <summary>
    /// Recupera un producto específico del repositorio identificándolo por su clave única.
    /// Busca en la base de datos un producto que coincida exactamente con el identificador proporcionado.
    /// Devuelve el producto encontrado o null si ningún registro coincide con el identificador especificado.
    /// </summary>
    /// <param name="id">Identificador único (GUID) del producto a recuperar del repositorio.</param>
    /// <returns>Objeto Producto que corresponde al identificador proporcionado; null si no existe en la base de datos.</returns>
    public Producto? ObtenerProducto(Guid id)
    {
        return _repository.GetById(id);
    }

    /// <summary>
    /// Verifica la conformidad integral de los datos del producto contra las reglas de validación de negocio definidas.
    /// Examina cada atributo obligatorio asegurando que contenga información válida y coherente.
    /// Valida que los valores numéricos de nutrientes se encuentren dentro de los rangos permitidos (no negativos).
    /// Si detecta cualquier incumplimiento de las reglas, interrumpe la validación y comunica el problema específico encontrado.
    /// </summary>
    /// <param name="producto">Objeto Producto cuyos atributos serán sometidos a validación según los estándares de negocio.</param>
    /// <exception cref="Exception">Se lanza con un mensaje descriptivo detallando qué regla de validación fue incumplida y por qué.</exception>
    private void ValidarProducto(Producto producto)
    {
        // Valida que el nombre del producto no está vacío ni contiene solo espacios en blanco
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new Exception("El nombre del producto es obligatorio.");

        // Valida que el valor de calorías sea un número no negativo dentro del rango permitido
        if (producto.Calorias < 0)
            throw new Exception("Las calorías no pueden ser negativas.");

        // Valida que el valor de proteínas sea un número no negativo dentro del rango permitido
        if (producto.Proteinas < 0)
            throw new Exception("Las proteínas no pueden ser negativas.");

        // Valida que el valor de carbohidratos sea un número no negativo dentro del rango permitido
        if (producto.Carbohidratos < 0)
            throw new Exception("Los carbohidratos no pueden ser negativos.");

        // Valida que el valor de grasas sea un número no negativo dentro del rango permitido
        if (producto.Grasas < 0)
            throw new Exception("Las grasas no pueden ser negativas.");
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

/// <summary>
/// Servicio de gestión de menús del sistema DragonNutrex.
/// Proporciona operaciones de lectura, creación, actualización y eliminación de menús
/// mediante el acceso a la capa de repositorio, que integra Redis como almacenamiento en caché.
/// </summary>
public class MenuService
{
    private readonly IRepository<Menu> _menuRepository;

    /// <summary>
    /// Inicializa una nueva instancia de la clase MenuService.
    /// </summary>
    /// <param name="menuRepository">Instancia del repositorio genérico utilizado para acceder a los datos de menús.</param>
    public MenuService(IRepository<Menu> menuRepository)
    {
        _menuRepository = menuRepository;
    }

    /// <summary>
    /// Obtiene de forma asíncrona la lista completa de menús disponibles en el repositorio.
    /// Ejecuta una operación no bloqueante que consulta Redis y retorna todos los menús registrados.
    /// </summary>
    /// <returns>Tarea asíncrona que retorna una lista de objetos Menu con todos los menús disponibles.</returns>
    public async Task<List<Menu>> ObtenerMenusAsync()
    {
        return await _menuRepository.GetAllAsync();
    }

    /// <summary>
    /// Obtiene de forma síncrona la lista completa de menús disponibles en el repositorio.
    /// Realiza una operación bloqueante que consulta los datos de menús almacenados.
    /// </summary>
    /// <returns>Lista de objetos Menu con todos los menús disponibles.</returns>
    public List<Menu> ObtenerMenus()
    {
        return _menuRepository.GetAll();
    }

    /// <summary>
    /// Crea e inserta un nuevo menú en el repositorio.
    /// Persiste el objeto Menu proporcionado en la capa de almacenamiento (Redis).
    /// </summary>
    /// <param name="menu">Objeto Menu que contiene la información del menú a crear.</param>
    public void CrearMenu(Menu menu)
    {
        _menuRepository.Create(menu);
    }

    /// <summary>
    /// Actualiza un menú existente en el repositorio.
    /// Modifica los datos del menú especificado con los valores proporcionados.
    /// </summary>
    /// <param name="menu">Objeto Menu con los datos actualizados a persistir.</param>
    public void ActualizarMenu(Menu menu)
    {
        _menuRepository.Update(menu);
    }

    /// <summary>
    /// Elimina un menú del repositorio mediante su identificador único.
    /// Remueve permanentemente el registro del menú especificado del almacenamiento.
    /// </summary>
    /// <param name="id">Identificador único (GUID) del menú a eliminar.</param>
    public void EliminarMenu(Guid id)
    {
        _menuRepository.Delete(id);
    }

    /// <summary>
    /// Obtiene un menú específico del repositorio mediante su identificador único.
    /// Localiza y retorna la información del menú que coincide con el GUID proporcionado.
    /// </summary>
    /// <param name="id">Identificador único (GUID) del menú a recuperar.</param>
    /// <returns>Objeto Menu correspondiente al identificador, o null si no existe.</returns>
    public Menu? ObtenerMenu(Guid id)
    {
        return _menuRepository.GetById(id);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

public class MenuService
{
    private readonly IRepository<Menu> _menuRepository;

    public MenuService(IRepository<Menu> menuRepository)
    {
        _menuRepository = menuRepository;
    }

    // =====================================================
    // EL NUEVO MÉTODO ASÍNCRONO
    // =====================================================
    public async Task<List<Menu>> ObtenerMenusAsync()
    {
        // Llamamos al GetAllAsync que ya configuramos en el Repositorio de Redis
        return await _menuRepository.GetAllAsync();
    }

    // =====================================================
    // MÉTODOS SÍNCRONOS EXISTENTES
    // =====================================================
    public List<Menu> ObtenerMenus()
    {
        return _menuRepository.GetAll();
    }

    public void CrearMenu(Menu menu)
    {
        _menuRepository.Create(menu);
    }

    public void ActualizarMenu(Menu menu)
    {
        _menuRepository.Update(menu);
    }

    public void EliminarMenu(Guid id)
    {
        _menuRepository.Delete(id);
    }

    public Menu? ObtenerMenu(Guid id)
    {
        return _menuRepository.GetById(id);
    }
}
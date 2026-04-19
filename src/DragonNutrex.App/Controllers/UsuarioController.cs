using System;
using System.Collections.Generic;
using DragonNutrex.App.Models;
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

// =====================================================
// CLASE USUARIO CONTROLLER
// =====================================================
public class UsuarioController
{
    private readonly UsuarioService _service;

    public UsuarioController(UsuarioService service)
    {
        _service = service;
    }

    // =====================================================
    // FASE 1: MÉTODOS ORIGINALES (CRUD BASE)
    // =====================================================
    
    public List<Usuario> ObtenerUsuarios()
    {
        return _service.ObtenerUsuarios();
    }

    public void CrearUsuario(Usuario usuario)
    {
        _service.CrearUsuario(usuario);
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        _service.ActualizarUsuario(usuario);
    }

    public void EliminarUsuario(Guid id)
    {
        // Eliminación física (Fase 1)
        _service.EliminarUsuario(id);
    }

    // =====================================================
    // FASE 2: NUEVAS FUNCIONES DE ADMINISTRADOR
    // =====================================================
    
    public void DesactivarUsuario(Guid id)
    {
        // Baja lógica (Soft Delete) - No borra de la BD, solo lo oculta
        var usuario = _service.ObtenerUsuario(id);
        if (usuario != null)
        {
            usuario.Activo = false;
            _service.ActualizarUsuario(usuario);
        }
    }

    public void ResetearPassword(Guid id, string nuevaPasswordPorDefecto = "Upi.2026")
    {
        // Reseteo rápido para que el admin pueda ayudar a usuarios bloqueados
        var usuario = _service.ObtenerUsuario(id);
        if (usuario != null)
        {
            usuario.Password = nuevaPasswordPorDefecto;
            _service.ActualizarUsuario(usuario);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories; // Necesario para acceder al método específico de Redis

namespace DragonNutrex.App.Services;

public class UsuarioService
{
    private readonly IRepository<Usuario> _usuarioRepository;

    public UsuarioService(IRepository<Usuario> usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    // =====================================================
    // 🔥 EL NUEVO MÉTODO PUENTE (Para el Login)
    // =====================================================
    public async Task<Usuario?> GetByCorreoAsync(string correo)
    {
        // Como la interfaz es genérica, la casteamos a UsuarioRedisRepository 
        // para destrabar el método especial ultra rápido que construimos hoy.
        if (_usuarioRepository is UsuarioRedisRepository redisRepo)
        {
            return await redisRepo.GetByCorreoAsync(correo);
        }
        
        return null;
    }

    // =====================================================
    // MOTOR ASÍNCRONO MAESTRO
    // =====================================================
    public async Task<List<Usuario>> ObtenerUsuariosAsync()
    {
        return await _usuarioRepository.GetAllAsync();
    }

    // =====================================================
    // MÉTODOS SÍNCRONOS Y ESCRITURA
    // =====================================================
    public List<Usuario> ObtenerUsuarios()
    {
        return _usuarioRepository.GetAll();
    }

    public void CrearUsuario(Usuario usuario)
    {
        _usuarioRepository.Create(usuario);
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        _usuarioRepository.Update(usuario);
    }

    public void EliminarUsuario(Guid id)
    {
        _usuarioRepository.Delete(id);
    }

    public Usuario? ObtenerUsuario(Guid id)
    {
        return _usuarioRepository.GetById(id);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using BCrypt.Net;

namespace DragonNutrex.App.Controllers;

public class UsuarioController
{
    private readonly IRepository<Usuario> _usuarioRepository;

    public UsuarioController(IRepository<Usuario> usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    // =====================================================
    // OBTENER TODOS
    // =====================================================
    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _usuarioRepository.GetAllAsync();
    }

    // =====================================================
    // OBTENER POR CORREO
    // =====================================================
    public async Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo)
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        return usuarios.FirstOrDefault(u => u.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
    }

    // =====================================================
    // AUTENTICAR (INICIAR SESIÓN CON SEGURIDAD)
    // =====================================================
    public async Task<Usuario?> AutenticarUsuarioAsync(string correo, string password)
    {
        var usuario = await ObtenerUsuarioPorCorreoAsync(correo);
        
        if (usuario == null) return null;

        // Si la contraseña es nula, rechazamos de inmediato
        if (string.IsNullOrWhiteSpace(usuario.Password)) return null;

        // MAGIA DE RETROCOMPATIBILIDAD: 
        // BCrypt siempre genera hashes que empiezan con "$2". 
        // Si no empieza con eso, significa que es un usuario viejo con contraseña en texto plano.
        if (!usuario.Password.StartsWith("$2"))
        {
            return usuario.Password == password ? usuario : null;
        }

        // VALIDACIÓN PROFESIONAL CON BCRYPT
        bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(password, usuario.Password);
        return passwordCorrecta ? usuario : null;
    }

    // =====================================================
    // CREAR USUARIO (REGISTRO)
    // =====================================================
    public void CrearUsuario(Usuario nuevoUsuario)
    {
        // Encriptamos la contraseña justo antes de guardarla en la base de datos
        if (!string.IsNullOrWhiteSpace(nuevoUsuario.Password) && !nuevoUsuario.Password.StartsWith("$2"))
        {
            nuevoUsuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevoUsuario.Password);
        }
        
        _usuarioRepository.Create(nuevoUsuario);
    }

    // =====================================================
    // ACTUALIZAR USUARIO (CAMBIO DE CONTRASEÑA)
    // =====================================================
    public void ActualizarUsuario(Usuario usuario)
    {
        // Si viene de la pantalla de recuperación, la contraseña vendrá en texto plano.
        // Aquí la detectamos y la encriptamos antes de mandarla a Redis.
        if (!string.IsNullOrWhiteSpace(usuario.Password) && !usuario.Password.StartsWith("$2"))
        {
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
        }

        _usuarioRepository.Update(usuario);
    }

    // =====================================================
    // ELIMINAR USUARIO
    // =====================================================
    public void EliminarUsuario(Guid id)
    {
        _usuarioRepository.Delete(id);
    }
}
using System;

namespace DragonNutrex.App.Models;

/// <summary>
/// Representa la entidad de usuario en el sistema DragonNutrex.
/// Encapsula toda la información relacionada con un usuario de la aplicación,
/// incluyendo datos de autenticación, medidas corporales y preferencias nutricionales.
/// Esta clase actúa como modelo de datos para la persistencia y gestión de usuarios
/// en la aplicación.
/// </summary>
public class Usuario
{
    /// <summary>
    /// Obtiene o establece el identificador único del usuario en el sistema.
    /// Utiliza el tipo Guid para garantizar compatibilidad con el resto de la aplicación.
    /// Corresponde a los valores de Redis (10, 11, 26, etc.) convertidos a formato GUID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre completo del usuario.
    /// Se utiliza para identificar al usuario en la interfaz y reportes.
    /// Inicializa con una cadena vacía por defecto.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario.
    /// Este campo corresponde al campo "Usuario" almacenado en Redis.
    /// Se denomina "Correo" en C# para evitar confusiones con el nombre de la clase Usuario.
    /// Funciona como identificador único alternativo y es empleado en el proceso de autenticación.
    /// </summary>
    public string Correo { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la contraseña encriptada del usuario.
    /// Se utiliza en el proceso de autenticación y validación de credenciales.
    /// Debe ser almacenada de forma segura, preferiblemente con algoritmos de hashing.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el peso corporal del usuario expresado en kilogramos.
    /// Dato esencial utilizado en cálculos nutricionales y de metabolismo.
    /// Se emplea para determinar requerimientos calóricos y macronutrientes.
    /// </summary>
    public decimal Peso { get; set; }

    /// <summary>
    /// Obtiene o establece la altura del usuario expresada en metros.
    /// Dato fundamental para calcular el Índice de Masa Corporal (IMC)
    /// y otros parámetros antropométricos relevantes para la nutrición.
    /// </summary>
    public decimal Altura { get; set; }

    /// <summary>
    /// Obtiene o establece el nivel de actividad física del usuario.
    /// Almacena valores predefinidos como "Ligera", "Moderada" o "Intensa".
    /// Se utiliza en la estimación del gasto energético total diario (TEDD)
    /// para personalizar el plan nutricional.
    /// </summary>
    public string Actividad { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el objetivo nutricional del usuario.
    /// Contiene valores como "Bajar peso", "Ganar masa muscular" o "Mantenimiento".
    /// Guía la configuración del plan dietético y la selección de alimentos recomendados.
    /// </summary>
    public string Objetivo { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el tipo de dieta preferido por el usuario.
    /// Almacena restricciones dietéticas como "Vegana", "Vegetariana", "Sin gluten", etc.
    /// Determina los alimentos disponibles y las recomendaciones adaptadas al usuario.
    /// </summary>
    public string TipoDieta { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el estado de actividad de la cuenta del usuario.
    /// Cuando es falso, indica que la cuenta ha sido desactivada o suspendida.
    /// Se utiliza para controlar el acceso a las funcionalidades de la aplicación.
    /// Inicializa como verdadero al crear un nuevo usuario.
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Obtiene un valor booleano que indica si el usuario posee privilegios de administrador.
    /// Genera una verificación basada en el correo electrónico (admin@admin.com) o el nombre de usuario (admin).
    /// Realiza comparaciones case-insensitive para garantizar flexibilidad en la validación.
    /// Esta es una propiedad calculada que facilita verificaciones de permisos en la capa de presentación.
    /// </summary>
    public bool EsAdmin => Correo.ToLower() == "admin@admin.com" || Nombre.ToLower() == "admin";
}
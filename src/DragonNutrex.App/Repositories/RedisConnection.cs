using System;
using StackExchange.Redis;

namespace DragonNutrex.App.Repositories;

/// <summary>
/// Gestiona la conexión con el servidor Redis utilizando el patrón Lazy para garantizar
/// inicialización única y seguridad en concurrencia. Proporciona acceso centralizado a la
/// base de datos Redis con configuración optimizada de timeouts y KeepAlive.
/// </summary>
public class RedisConnection
{
    /// <summary>
    /// Almacena la conexión multiplexor de Redis encapsulada en un Lazy. Garantiza que
    /// la conexión se establezca una única vez, independientemente de cuántos hilos accedan
    /// simultáneamente, eliminando bloqueos innecesarios y asegurando thread-safety.
    /// </summary>
    private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

    /// <summary>
    /// Inicializa una nueva instancia de RedisConnection con la cadena de conexión especificada.
    /// Configura los parámetros de timeout y comportamiento de fallos para optimizar la
    /// estabilidad y velocidad de las operaciones con Redis.
    /// </summary>
    /// <param name="connectionString">Cadena de conexión a Redis en formato estándar (ej: "localhost:6379").</param>
    /// <exception cref="ArgumentException">Se lanza cuando la cadena de conexión está vacía o contiene solo espacios en blanco.</exception>
    public RedisConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("La cadena de conexión de Redis está vacía.");

        _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            var options = ConfigurationOptions.Parse(connectionString);
            
            // 1. Evitamos que la app explote si Redis parpadea
            options.AbortOnConnectFail = false;

            // 2. ⚡ LÍMITES DE VELOCIDAD ESTRICTOS (Adiós a los 99 segundos)
            options.ConnectTimeout = 5000; // Máximo 5 segundos para enganchar
            options.SyncTimeout = 5000;    // Máximo 5 segundos por operación
            options.AsyncTimeout = 5000;   // Máximo 5 segundos en background
            options.KeepAlive = 60;        // Mantiene el túnel despierto cada minuto

            return ConnectionMultiplexer.Connect(options);
        });
    }

    // =================================================================
    // OBTENER BASE DE DATOS (AHORA 100% SEGURO Y RÁPIDO)
    // =================================================================
    public IDatabase GetDatabase()
    {
        // Al llamar a .Value, el patrón Lazy hace su magia. 
        // Si no existe, la crea rapidísimo. Si ya existe, la devuelve en 0.001ms.
        var connection = _lazyConnection.Value;

        if (connection == null)
        {
            throw new Exception("No se pudo establecer la conexión con Redis.");
        }

        return connection.GetDatabase();
    }

    public IDatabase Database => GetDatabase();
}
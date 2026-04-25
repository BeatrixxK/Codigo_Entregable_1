using System;
using StackExchange.Redis;

namespace DragonNutrex.App.Repositories;

public class RedisConnection
{
    // 🔥 EL FRANCOTIRADOR: Lazy garantiza que la conexión se arme UNA SOLA VEZ, 
    // sin importar cuántos hilos la pidan al mismo tiempo. Cero bloqueos.
    private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

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
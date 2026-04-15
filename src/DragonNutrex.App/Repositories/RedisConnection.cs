using System;
using StackExchange.Redis;

namespace DragonNutrex.App.Repositories;

public class RedisConnection
{
    private ConnectionMultiplexer? _connection;
    private readonly string _connectionString;

    public RedisConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("La cadena de conexión de Redis está vacía.");

        _connectionString = connectionString;
    }

    private void Connect()
    {
        var options = ConfigurationOptions.Parse(_connectionString);
        options.AbortOnConnectFail = false;

        _connection = ConnectionMultiplexer.Connect(options);
    }

    // =================================================================
    // SOLUCIÓN CS1061: Agregamos el método que los repositorios buscan
    // =================================================================
    public IDatabase GetDatabase()
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Connect();
        }

        // SOLUCIÓN CS8602: Validamos explícitamente que no sea null
        if (_connection == null)
        {
            throw new Exception("No se pudo establecer la conexión con Redis.");
        }

        return _connection.GetDatabase();
    }

    // Mantenemos la propiedad por si la usas en otra parte del proyecto
    public IDatabase Database => GetDatabase();
}
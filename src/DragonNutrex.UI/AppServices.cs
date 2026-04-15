using System;
using DragonNutrex.App.Repositories;
using DotNetEnv; 

namespace DragonNutrex.UI;

public static class AppServices
{
    private static RedisConnection? _redis;

    public static RedisConnection? Redis
    {
        get
        {
            if (_redis != null)
                return _redis;

            try
            {
                // 1. 🔥 CORRECCIÓN: Usamos TraversePath() para que busque el .env 
                // en carpetas superiores si no lo encuentra en la actual.
                DotNetEnv.Env.TraversePath().Load();

                // 2. Buscamos la variable secreta en el entorno del sistema
                var connectionString = Environment.GetEnvironmentVariable("REDIS_URL");

                // 3. Validación de seguridad para evitar que la app intente conectar al vacío
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("❌ ERROR CRÍTICO: No se encontró la variable REDIS_URL.");
                    Console.WriteLine("Asegúrate de que el archivo .env existe y tiene el formato REDIS_URL=valor");
                    return null;
                }

                // 4. Inicializamos la conexión con la cadena segura
                _redis = new RedisConnection(connectionString);

                Console.WriteLine("✅ Redis conectado correctamente usando variable de entorno segura.");
                return _redis;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error creando la conexión Redis:");
                Console.WriteLine(ex.Message);

                return null;
            }
        }
    }
}
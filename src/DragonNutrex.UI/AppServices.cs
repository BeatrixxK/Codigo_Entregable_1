using System;
using DragonNutrex.App.Repositories;

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
                // ✨ SOLUCIÓN: Todo el texto, incluidos los tiempos extra, están DENTRO de las comillas.
                var connectionString = "redis-11674.c16.us-east-1-2.ec2.cloud.redislabs.com:11674,password=SStOKh8Jr8dlzIndL0eVP37Wy8p4o5xj,abortConnect=false,connectTimeout=15000,syncTimeout=15000";

                _redis = new RedisConnection(connectionString);

                Console.WriteLine("✅ Redis conectado correctamente.");
                return _redis;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error creando la conexión Redis:");
                Console.WriteLine(ex.ToString());

                return null; // evita crash
            }
        }
    }
}
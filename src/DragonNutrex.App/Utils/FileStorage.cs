/* Leer datos de JSON ---- Ejemplo: guardar la lista de usuarios.
✔ lectura JSON
✔ escritura JSON */

using System.Text.Json;

namespace DragonNutrex.App.Utils;

public static class FileStorage
{
    public static List<T> ReadList<T>(string path)
    {
        if (!File.Exists(path))
            return new List<T>();

        var json = File.ReadAllText(path);

        if (string.IsNullOrWhiteSpace(json))
            return new List<T>();

        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public static void WriteList<T>(string path, List<T> data)
    {
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(path, json);
    }
}
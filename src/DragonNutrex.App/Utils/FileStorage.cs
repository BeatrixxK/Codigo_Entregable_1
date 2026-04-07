/* =====================================================
   MANEJO DE ARCHIVOS JSON
   Permite leer y escribir listas de objetos en archivos JSON
   ✔ lectura JSON
   ✔ escritura JSON
   ===================================================== */

// Librería para serializar y deserializar JSON
using System.Text.Json;

namespace DragonNutrex.App.Utils;

// =====================================================
// CLASE FILE STORAGE
// =====================================================
// Clase utilitaria para guardar y leer datos en archivos JSON
public static class FileStorage
{
    // =====================================================
    // MÉTODO READLIST
    // =====================================================
    // Lee un archivo JSON y lo convierte en una lista de objetos
    public static List<T> ReadList<T>(string path)
    {
        // Si el archivo no existe, retorna lista vacía
        if (!File.Exists(path))
            return new List<T>();

        // Lee todo el contenido del archivo
        var json = File.ReadAllText(path);

        // Si el archivo está vacío, retorna lista vacía
        if (string.IsNullOrWhiteSpace(json))
            return new List<T>();

        // Convierte el JSON a lista de tipo T
        // Si falla, retorna lista vacía
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    // =====================================================
    // MÉTODO WRITELIST
    // =====================================================
    // Guarda una lista de objetos en un archivo JSON
    public static void WriteList<T>(string path, List<T> data)
    {
        // Obtiene la carpeta del archivo
        var directory = Path.GetDirectoryName(path);

        // Si la carpeta no existe, la crea
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // Opciones de serialización (formato legible)
        var options = new JsonSerializerOptions
        {
            WriteIndented = true // JSON con formato bonito
        };

        // Convierte la lista a JSON
        var json = JsonSerializer.Serialize(data, options);

        // Escribe el JSON en el archivo
        File.WriteAllText(path, json);
    }
}
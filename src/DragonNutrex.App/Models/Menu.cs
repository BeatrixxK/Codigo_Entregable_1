// =====================================================
// MODELO MENU
// =====================================================
namespace DragonNutrex.App.Models;

/// <summary>
/// Representa un menú diario de un usuario.
/// 
/// Esta clase encapsula la información de un menú perteneciente a un usuario específico
/// en una fecha determinada. Almacena los registros de comida consumida durante el día
/// y mantiene los totales nutricionales acumulados (calorías, proteínas, carbohidratos y grasas).
/// 
/// El menú actúa como contenedor central para la gestión de la ingesta diaria de alimentos
/// y proporciona un resumen nutricional consolidado de todas las comidas registradas.
/// </summary>
public class Menu
{
    /// <summary>
    /// Obtiene o establece el identificador único del menú.
    /// 
    /// Genera automáticamente un GUID nuevo para cada instancia de menú creada,
    /// asegurando la identificación única del registro en el sistema.
    /// Este valor se utiliza como clave primaria en la base de datos.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Obtiene o establece el identificador del usuario propietario del menú.
    /// 
    /// Establece la relación entre el menú y el usuario específico al que pertenece.
    /// Este valor se utiliza para filtrar y asociar menús con sus usuarios correspondientes
    /// en operaciones de consulta y gestión de datos.
    /// </summary>
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha del menú (día de consumo).
    /// 
    /// Registra la fecha específica en la cual los alimentos fueron consumidos.
    /// Permite organizar y consultar menús por períodos de tiempo, facilitando
    /// el seguimiento histórico de la ingesta nutricional del usuario.
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Obtiene o establece la lista de registros de comida consumida.
    /// 
    /// Almacena todos los alimentos y productos registrados como consumidos durante
    /// el día específico del menú. Cada registro contiene información detallada
    /// del producto y la cantidad consumida. Se inicializa como una lista vacía
    /// por defecto para cada nuevo menú.
    /// </summary>
    public List<RegistroComida> Registros { get; set; } = new();

    // =====================================================
    // TOTALES NUTRICIONALES DEL MENÚ
    // =====================================================

    /// <summary>
    /// Obtiene o establece el total de calorías consumidas en el día.
    /// 
    /// Almacena la suma acumulada de todas las calorías provenientes de los
    /// registros de comida del menú. Este valor se calcula sumando las calorías
    /// de cada alimento registrado y se utiliza para el seguimiento de la ingesta
    /// calórica diaria del usuario.
    /// </summary>
    public decimal TotalCalorias { get; set; }

    /// <summary>
    /// Obtiene o establece el total de proteínas consumidas en el día.
    /// 
    /// Mantiene la suma acumulada de proteínas (en gramos) provenientes de todos
    /// los registros de comida. Este macronutriente es esencial para el monitoreo
    /// nutricional y permite evaluar si la ingesta de proteínas cumple con los
    /// objetivos dietéticos establecidos para el usuario.
    /// </summary>
    public decimal TotalProteinas { get; set; }

    /// <summary>
    /// Obtiene o establece el total de carbohidratos consumidos en el día.
    /// 
    /// Registra la suma acumulada de carbohidratos (en gramos) de todos los alimentos
    /// consumidos. Este macronutriente se monitorea para el control glucémico
    /// y nutricional general del usuario.
    /// </summary>
    public decimal TotalCarbohidratos { get; set; }

    /// <summary>
    /// Obtiene o establece el total de grasas consumidas en el día.
    /// 
    /// Almacena la suma acumulada de grasas (en gramos) provenientes de todos
    /// los registros de comida del menú. El seguimiento de grasas es importante
    /// para evaluar la ingesta de lípidos y su impacto en los objetivos nutricionales
    /// del usuario.
    /// </summary>
    public decimal TotalGrasas { get; set; }
}
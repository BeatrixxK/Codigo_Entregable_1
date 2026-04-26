// =====================================================
// MODELO RESUMEN ESTADÍSTICA NUTRICIONAL
// =====================================================
// Representa el análisis nutricional de un usuario
// en un rango de fechas (no solo un día)
// Incluye totales, promedios, objetivos y diferencias
namespace DragonNutrex.App.Models;

/// <summary>
/// Encapsula el análisis estadístico nutricional integral de un usuario en un período determinado.
/// Procesa y consolida la información dietética para proporcionar comparativas entre consumo real
/// y objetivos establecidos según el tipo de dieta seleccionada. Genera métricas de balance
/// calórico y recomendaciones basadas en el desempeño nutricional del usuario.
/// </summary>
public class ResumenEstadisticaNutricional
{
    /// <summary>
    /// Obtiene o establece el nombre identificador del usuario cuyo análisis nutricional se representa.
    /// Almacena la referencia nominal para propósitos de trazabilidad y reportes.
    /// </summary>
    public string NombreUsuario { get; set; } = "";

    /// <summary>
    /// Obtiene o establece el tipo de dieta seleccionada como referencia para la comparación nutricional.
    /// Determina los valores objetivos contra los cuales se evalúa el consumo real del usuario.
    /// </summary>
    public string TipoDieta { get; set; } = "";

    /// <summary>
    /// Obtiene o establece la fecha de inicio del período analizado en el resumen estadístico.
    /// Marca el límite inferior del rango temporal considerado en el cálculo de métricas.
    /// </summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha de finalización del período analizado en el resumen estadístico.
    /// Marca el límite superior del rango temporal considerado en el cálculo de métricas.
    /// </summary>
    public DateTime FechaFin { get; set; }

    /// <summary>
    /// Obtiene o establece la cantidad total de menús evaluados dentro del rango de fechas especificado.
    /// Facilita el cálculo de promedios diarios dividiendo totales acumulados entre este valor.
    /// </summary>
    public int CantidadMenus { get; set; }

    // =====================================================
    // DATOS DEL IMC
    // =====================================================

    /// <summary>
    /// Obtiene o establece el Índice de Masa Corporal (IMC) del usuario analizado.
    /// Representa la relación entre peso corporal y estatura, expresado en kg/m².
    /// Proporciona contexto para la evaluación nutricional personalizada.
    /// </summary>
    public decimal Imc { get; set; }

    /// <summary>
    /// Obtiene o establece la categorización del Índice de Masa Corporal según estándares clínicos.
    /// Clasifica el estado nutricional del usuario (Normal, Sobrepeso, Obesidad, etc.)
    /// para orientar las recomendaciones dietéticas.
    /// </summary>
    public string CategoriaImc { get; set; } = "";

    // =====================================================
    // CONSUMO TOTAL EN EL RANGO
    // =====================================================

    /// <summary>
    /// Obtiene o establece el total acumulado de calorías consumidas durante todo el período analizado.
    /// Procesa la suma integral de kilocalorías registradas en todos los menús del rango de fechas.
    /// Sirve como base para calcular promedios diarios y comparaciones con objetivos.
    /// </summary>
    public decimal CaloriasTotalesConsumidas { get; set; }

    /// <summary>
    /// Obtiene o establece el total acumulado de proteínas consumidas durante el período analizado.
    /// Suma la ingesta íntegra de proteínas en gramos desde todos los registros dietéticos.
    /// Permite evaluar la suficiencia proteica para recuperación y síntesis muscular.
    /// </summary>
    public decimal ProteinasTotalesConsumidas { get; set; }

    /// <summary>
    /// Obtiene o establece el total acumulado de carbohidratos consumidos durante el período analizado.
    /// Consolida la ingesta integral de carbohidratos en gramos desde el conjunto de menús registrados.
    /// Facilita el análisis de aporte energético y estabilidad glucémica.
    /// </summary>
    public decimal CarbohidratosTotalesConsumidos { get; set; }

    /// <summary>
    /// Obtiene o establece el total acumulado de grasas consumidas durante el período analizado.
    /// Suma la ingesta completa de lípidos en gramos desde todos los registros dietéticos del período.
    /// Posibilita la evaluación del aporte lipídico y su influencia calórica total.
    /// </summary>
    public decimal GrasasTotalesConsumidas { get; set; }

    // =====================================================
    // PROMEDIOS DIARIOS
    // =====================================================

    /// <summary>
    /// Obtiene o establece el promedio diario de calorías consumidas en el período analizado.
    /// Calcula la ingesta calórica media dividiendo el total consumido entre la cantidad de menús.
    /// Normaliza el consumo para facilitar comparaciones independientes de la duración del período.
    /// </summary>
    public decimal CaloriasPromedio { get; set; }

    /// <summary>
    /// Obtiene o establece el promedio diario de proteínas consumidas en el período analizado.
    /// Genera la media de ingesta proteica dividiendo el total acumulado entre la cantidad de menús.
    /// Permite evaluar si el usuario alcanza metas proteicas recomendadas diariamente.
    /// </summary>
    public decimal ProteinasPromedio { get; set; }

    /// <summary>
    /// Obtiene o establece el promedio diario de carbohidratos consumidos en el período analizado.
    /// Calcula la ingesta media de carbohidratos normalizando por la duración del análisis.
    /// Facilita el seguimiento de objetivos macronutricionales en base diaria.
    /// </summary>
    public decimal CarbohidratosPromedio { get; set; }

    /// <summary>
    /// Obtiene o establece el promedio diario de grasas consumidas en el período analizado.
    /// Genera la media de ingesta lipídica distribuyendo el total entre la cantidad de menús.
    /// Permite monitorear la adherencia a limitaciones grasa según el tipo de dieta.
    /// </summary>
    public decimal GrasasPromedio { get; set; }

    // =====================================================
    // OBJETIVOS SEGÚN DIETA
    // =====================================================

    /// <summary>
    /// Obtiene o establece el objetivo calórico diario establecido según el tipo de dieta seleccionada.
    /// Proporciona el valor de referencia contra el cual se compara el consumo real promedio.
    /// Orienta la evaluación de balance energético y suficiencia alimentaria.
    /// </summary>
    public decimal CaloriasObjetivoDieta { get; set; }

    /// <summary>
    /// Obtiene o establece el objetivo diario de proteínas según el tipo de dieta seleccionada.
    /// Define el aporte proteico recomendado que el usuario debe alcanzar diariamente.
    /// Facilita el monitoreo de adecuación proteica para objetivos de composición corporal.
    /// </summary>
    public decimal ProteinasObjetivoDieta { get; set; }

    /// <summary>
    /// Obtiene o establece el objetivo diario de carbohidratos según el tipo de dieta seleccionada.
    /// Establece la meta de ingesta de carbohidratos recomendada para el usuario.
    /// Permite evaluar el cumplimiento de la distribución macronutricional planificada.
    /// </summary>
    public decimal CarbohidratosObjetivoDieta { get; set; }

    /// <summary>
    /// Obtiene o establece el objetivo diario de grasas según el tipo de dieta seleccionada.
    /// Define la meta de ingesta lipídica recomendada según la distribución macronutricional.
    /// Proporciona referencia para evaluar la calidad y cantidad del aporte graso.
    /// </summary>
    public decimal GrasasObjetivoDieta { get; set; }

    // =====================================================
    // DIFERENCIAS (PROMEDIO VS OBJETIVO)
    // =====================================================

    /// <summary>
    /// Obtiene o establece la diferencia cuantitativa entre calorías consumidas en promedio y objetivo.
    /// Expresa el desvío calórico (positivo = exceso, negativo = déficit) del período analizado.
    /// Fundamental para determinar el estado de balance energético del usuario.
    /// </summary>
    public decimal DiferenciaCalorias { get; set; }

    /// <summary>
    /// Obtiene o establece la diferencia cuantitativa entre proteínas consumidas en promedio y objetivo.
    /// Indica el surplus o déficit proteico en relación a la meta dietética establecida.
    /// Determina la suficiencia de aporte proteico para objetivos de ganancia o mantenimiento muscular.
    /// </summary>
    public decimal DiferenciaProteinas { get; set; }

    /// <summary>
    /// Obtiene o establece la diferencia cuantitativa entre carbohidratos consumidos en promedio y objetivo.
    /// Expresa el desvío del consumo de carbohidratos respecto a la meta dietética.
    /// Evalúa la adherencia a la distribución macronutricional recomendada.
    /// </summary>
    public decimal DiferenciaCarbohidratos { get; set; }

    /// <summary>
    /// Obtiene o establece la diferencia cuantitativa entre grasas consumidas en promedio y objetivo.
    /// Indica el surplus o déficit en la ingesta lipídica relativo a la meta establecida.
    /// Proporciona insights sobre la calidad de distribución macronutricional lograda.
    /// </summary>
    public decimal DiferenciaGrasas { get; set; }

    // =====================================================
    // RESULTADO FINAL
    // =====================================================

    /// <summary>
    /// Obtiene o establece el estado calórico determinado por el análisis comparativo del período.
    /// Clasifica el balance energético del usuario como déficit, exceso o balance neutro.
    /// Sintetiza el resultado del consumo promedio versus el objetivo calórico en categoría legible.
    /// </summary>
    public string EstadoCalorico { get; set; } = "";

    /// <summary>
    /// Obtiene o establece la recomendación nutricional final generada según el análisis integral realizado.
    /// Proporciona orientación accionable basada en métricas de balance energético y macronutricional.
    /// Constituye la conclusión ejecutiva del informe estadístico nutricional para el usuario.
    /// </summary>
    public string Recomendacion { get; set; } = "";
}
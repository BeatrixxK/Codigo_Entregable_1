/// <summary>Representa el resultado del análisis nutricional diario de un usuario, comparando el consumo real con los objetivos nutricionales.</summary>
namespace DragonNutrex.App.Models;

public class ResumenNutricional
{
    /// <summary>Identifica el nombre del usuario para el que se realiza el análisis.</summary>
    public string NombreUsuario { get; set; } = "";

    /// <summary>Indica la fecha en la que se realizó el análisis nutricional.</summary>
    public DateTime Fecha { get; set; }

    // =====================================================
    // DATOS DEL IMC
    // =====================================================

    /// <summary>Contiene el valor calculado del índice de masa corporal del usuario.</summary>
    public decimal Imc { get; set; }

    /// <summary>Clasifica el IMC en categorías como Normal, Sobrepeso, Obeso, etc.</summary>
    public string CategoriaImc { get; set; } = "";

    // =====================================================
    // CONSUMO REAL DEL USUARIO
    // =====================================================

    /// <summary>Registra las calorías totales consumidas por el usuario en el día.</summary>
    public decimal CaloriasConsumidas { get; set; }

    /// <summary>Registra las proteínas totales consumidas por el usuario en el día.</summary>
    public decimal ProteinasConsumidas { get; set; }

    /// <summary>Registra los carbohidratos totales consumidos por el usuario en el día.</summary>
    public decimal CarbohidratosConsumidos { get; set; }

    /// <summary>Registra las grasas totales consumidas por el usuario en el día.</summary>
    public decimal GrasasConsumidas { get; set; }

    // =====================================================
    // OBJETIVOS NUTRICIONALES
    // =====================================================

    /// <summary>Define las calorías objetivo que el usuario debe consumir diariamente.</summary>
    public decimal CaloriasObjetivo { get; set; }

    /// <summary>Define las proteínas objetivo que el usuario debe consumir diariamente.</summary>
    public decimal ProteinasObjetivo { get; set; }

    /// <summary>Define los carbohidratos objetivo que el usuario debe consumir diariamente.</summary>
    public decimal CarbohidratosObjetivo { get; set; }

    /// <summary>Define las grasas objetivo que el usuario debe consumir diariamente.</summary>
    public decimal GrasasObjetivo { get; set; }

    // =====================================================
    // RESULTADO FINAL
    // =====================================================

    /// <summary>Determina el estado calórico basado en la comparación entre consumo y objetivo, como déficit, exceso o balance.</summary>
    public string EstadoCalorico { get; set; } = "";

    /// <summary>Proporciona una recomendación personalizada basada en el análisis del consumo nutricional.</summary>
    public string Recomendacion { get; set; } = "";
}
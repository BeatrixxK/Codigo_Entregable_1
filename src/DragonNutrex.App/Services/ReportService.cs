using System.Text;
using DragonNutrex.App.Models;
using QuestPDF.Fluent;       // Añade esto
using QuestPDF.Helpers;      // Añade esto
using QuestPDF.Infrastructure; // Añade esto

namespace DragonNutrex.App.Services;

/// <summary>
/// Servicio que proporciona funcionalidades para generar reportes en diferentes formatos, incluyendo CSV, TXT y PDF.
/// </summary>
public class ReportService
{
    // ... tus métodos anteriores de CSV y TXT ...

    /// <summary>
    /// Genera un documento PDF que contiene un reporte de consumo nutricional basado en una lista de menús.
    /// El reporte incluye información sobre calorías, proteínas, carbohidratos y grasas totales por fecha.
    /// Configura la licencia de QuestPDF, define la estructura de la página con encabezado, contenido y pie de página,
    /// y compila el documento en un arreglo de bytes para su descarga o almacenamiento.
    /// </summary>
    /// <param name="menus">Lista de objetos Menu que contienen los datos nutricionales a reportar.</param>
    /// <param name="nombreAdmin">Nombre del administrador responsable de la generación del reporte.</param>
    /// <returns>Un arreglo de bytes que representa el archivo PDF generado.</returns>
    public byte[] GenerarConsumoPDF(List<Menu> menus, string nombreAdmin)
    {
        // Configura la licencia comunitaria obligatoria de QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;

        // Construye el documento maestro
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // Configura los márgenes y tipografía base de la página
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Element(ComposeHeader);
                page.Content().Element(x => ComposeContent(x, menus));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        // Retorna el PDF compilado como un arreglo de bytes
        return document.GeneratePdf();

        /// <summary>
        /// Compone el encabezado del documento PDF, incluyendo el nombre de la empresa, título del reporte, nombre del administrador y fecha de generación.
        /// Utiliza una fila con un elemento relativo para el texto y un elemento constante para un separador visual.
        /// </summary>
        /// <param name="container">Contenedor donde se agrega el encabezado.</param>
        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("DRAGON NUTREX").FontSize(24).SemiBold().FontColor(Colors.Green.Darken2);
                    column.Item().Text("Reporte de Consumo Nutricional").FontSize(14).FontColor(Colors.Grey.Darken2);
                    column.Item().PaddingTop(5).Text($"Generado por: {nombreAdmin}");
                    column.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
                
                // Muestra un recuadro gris como separador visual superior
                row.ConstantItem(100).Height(50).Placeholder(); 
            });
        }

        /// <summary>
        /// Compone el contenido principal del documento PDF, que consiste en una tabla con los datos nutricionales de los menús.
        /// Define las columnas de la tabla, aplica estilos al encabezado y llena las filas con los datos de cada menú.
        /// </summary>
        /// <param name="container">Contenedor donde se agrega el contenido.</param>
        /// <param name="menus">Lista de menús utilizados para poblar la tabla.</param>
        void ComposeContent(IContainer container, List<Menu> menus)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(5);
                column.Item().Table(table =>
                {
                    // Asigna anchos equitativos a las 5 columnas
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(); 
                        columns.RelativeColumn(); 
                        columns.RelativeColumn(); 
                        columns.RelativeColumn(); 
                        columns.RelativeColumn(); 
                    });

                    // Aplica estilo oscuro a la cabecera de la tabla
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Fecha");
                        header.Cell().Element(CellStyle).AlignRight().Text("Calorías");
                        header.Cell().Element(CellStyle).AlignRight().Text("Proteínas");
                        header.Cell().Element(CellStyle).AlignRight().Text("Carbos");
                        header.Cell().Element(CellStyle).AlignRight().Text("Grasas");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    // Itera sobre los registros para llenar las filas
                    foreach (var m in menus)
                    {
                        table.Cell().Element(CellStyle).Text($"{m.Fecha:dd/MM/yyyy}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{m.TotalCalorias:N1}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{m.TotalProteinas:N1} g");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{m.TotalCarbohidratos:N1} g");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{m.TotalGrasas:N1} g");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });
            });
        }
    }
}
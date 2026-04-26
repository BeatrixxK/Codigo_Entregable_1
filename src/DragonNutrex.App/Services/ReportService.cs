using System.Text;
using DragonNutrex.App.Models;
using QuestPDF.Fluent;       // Añade esto
using QuestPDF.Helpers;      // Añade esto
using QuestPDF.Infrastructure; // Añade esto

namespace DragonNutrex.App.Services;

public class ReportService
{
    // ... tus métodos anteriores de CSV y TXT ...

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

        // Arma el encabezado principal con el nombre de la empresa y metadatos
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

        // Arma la tabla de datos central
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
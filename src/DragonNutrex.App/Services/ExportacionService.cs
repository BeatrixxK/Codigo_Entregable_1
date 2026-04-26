using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;
using DragonNutrex.App.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.JSInterop; 

namespace DragonNutrex.App.Services;

public class ExportacionService
{
    private readonly IJSRuntime _js;

    public ExportacionService(IJSRuntime js)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        _js = js;
    }

    // =====================================================
    // EXPORTAR USUARIOS
    // =====================================================
    public byte[] GenerarCsvUsuarios(List<Usuario> usuarios)
    {
        var sb = new StringBuilder();
        
        // Define los encabezados separados por punto y coma (;) para evitar que Excel divida las columnas en los decimales
        sb.AppendLine("ID;Nombre;Peso;Altura;Dieta;Objetivo;Estado");

        foreach (var u in usuarios)
        {
            var estado = u.Activo ? "Activo" : "Inactivo";
            // Construye la fila separando cada propiedad del usuario con punto y coma
            sb.AppendLine($"{u.Id};{u.Nombre};{u.Peso};{u.Altura};{u.TipoDieta};{u.Objetivo};{estado}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public byte[] GenerarTxtUsuarios(List<Usuario> usuarios)
    {
        var sb = new StringBuilder();
        sb.AppendLine("==================================================");
        sb.AppendLine("      REPORTE DE USUARIOS - DRAGON NUTREX");
        sb.AppendLine("==================================================");
        sb.AppendLine();

        foreach (var u in usuarios)
        {
            var estado = u.Activo ? "Activo" : "Inactivo";
            sb.AppendLine($"👤 Nombre: {u.Nombre}");
            sb.AppendLine($"   Peso: {u.Peso}kg | Altura: {u.Altura}cm");
            sb.AppendLine($"   Dieta: {u.TipoDieta} | Objetivo: {u.Objetivo}");
            sb.AppendLine($"   Estado en sistema: {estado}");
            sb.AppendLine("--------------------------------------------------");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // =====================================================
    // EXPORTAR ESTADÍSTICAS GLOBALES (PDF)
    // =====================================================
    public byte[] GenerarPdfEstadisticas(
        Dictionary<string, double> porcentajesDietas,
        List<KeyValuePair<string, int>> topUsuarios,
        List<KeyValuePair<string, decimal>> topProductos, 
        string fechaRango)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                page.Header().BorderBottom(1).PaddingBottom(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("DRAGON NUTREX").FontSize(24).SemiBold().FontColor(Colors.Red.Medium);
                        col.Item().Text("Reporte Global de Estadísticas del Sistema").FontSize(14);
                        col.Item().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Text("1. Top Productos Más Consumidos").SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2);
                    col.Item().Text($"Rango evaluado: {fechaRango}").FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().PaddingTop(5).PaddingBottom(1, Unit.Centimetre).Column(lista =>
                    {
                        int posicion = 1;
                        foreach (var p in topProductos)
                        {
                            lista.Item().Text($"{posicion}. {p.Key} ({p.Value} consumidos)");
                            posicion++;
                        }
                        if (!topProductos.Any()) lista.Item().Text("No hay datos en este rango.").FontColor(Colors.Grey.Medium);
                    });

                    col.Item().Text("2. Distribución de Dietas de Usuarios").SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2);
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Tipo de Dieta").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Porcentaje de Uso").SemiBold();
                        });
                        foreach (var dieta in porcentajesDietas)
                        {
                            table.Cell().PaddingVertical(5).Text(dieta.Key);
                            table.Cell().PaddingVertical(5).Text($"{dieta.Value}%");
                        }
                    });
                    
                    col.Item().PaddingTop(1, Unit.Centimetre).Text("3. Top Usuarios con Más Menús").SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2);
                    col.Item().PaddingTop(5).Column(lista =>
                    {
                        int posicion = 1;
                        foreach (var u in topUsuarios)
                        {
                            lista.Item().Text($"{posicion}. {u.Key} ({u.Value} menús registrados)");
                            posicion++;
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x => { x.Span("Página "); x.CurrentPageNumber(); x.Span(" de "); x.TotalPages(); });
            });
        });

        return document.GeneratePdf();
    }

    // =====================================================
    // EXPORTAR CONSUMOS / MENÚS
    // =====================================================
    public byte[] GenerarCsvMenus(List<Menu> menus, List<Usuario> usuarios)
    {
        var sb = new StringBuilder();
        
        // Define los encabezados del CSV utilizando punto y coma como delimitador principal
        sb.AppendLine("Fecha;Usuario;TotalCalorias;Proteinas;Carbos;Grasas");

        foreach (var m in menus)
        {
            var user = usuarios.FirstOrDefault(u => u.Id == m.UsuarioId);
            var nombreUsuario = user != null ? user.Nombre : "Desconocido";

            // Aplica el formato de fila con la fecha y los datos cruzados del usuario, separando con punto y coma
            sb.AppendLine($"{m.Fecha:dd/MM/yyyy};{nombreUsuario};{m.TotalCalorias};{m.TotalProteinas};{m.TotalCarbohidratos};{m.TotalGrasas}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public byte[] GenerarTxtMenus(List<Menu> menus, List<Usuario> usuarios)
    {
        var sb = new StringBuilder();
        sb.AppendLine("==================================================");
        sb.AppendLine("      REPORTE DE CONSUMO - DRAGON NUTREX");
        sb.AppendLine("==================================================");
        sb.AppendLine();

        foreach (var m in menus)
        {
            var user = usuarios.FirstOrDefault(u => u.Id == m.UsuarioId);
            var nombreUsuario = user != null ? user.Nombre : "Desconocido";

            sb.AppendLine($"📅 Fecha: {m.Fecha:dd/MM/yyyy} | 👤 Usuario: {nombreUsuario}");
            sb.AppendLine($"   Kcal Totales: {m.TotalCalorias}");
            sb.AppendLine($"   Macros: Proteína {m.TotalProteinas}g | Carbohidratos {m.TotalCarbohidratos}g | Grasa {m.TotalGrasas}g");
            sb.AppendLine("--------------------------------------------------");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // =====================================================
    // PDF PERSONALIZADO DE USUARIO
    // =====================================================
    public byte[] GenerarPdfMenus(List<Menu> menus, string nombreUsuario)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().BorderBottom(1).PaddingBottom(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("DRAGON NUTREX").FontSize(24).SemiBold().FontColor(Colors.Green.Darken2);
                        col.Item().Text($"Reporte de Consumo de Menús - {nombreUsuario}").FontSize(14);
                        col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); 
                            columns.RelativeColumn(); 
                            columns.RelativeColumn(); 
                            columns.RelativeColumn(); 
                            columns.RelativeColumn(); 
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Fecha").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Calorías").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Proteínas").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Carbos").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Grasas").SemiBold();
                        });

                        foreach (var m in menus)
                        {
                            decimal calorias = m.Registros?.Sum(r => r.Calorias) ?? 0;
                            decimal proteinas = m.Registros?.Sum(r => r.Proteinas) ?? 0;
                            decimal carbos = m.Registros?.Sum(r => r.Carbohidratos) ?? 0;
                            decimal grasas = m.Registros?.Sum(r => r.Grasas) ?? 0;

                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text($"{m.Fecha:dd/MM/yyyy}");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{calorias:N1}");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{proteinas:N1}g");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{carbos:N1}g");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{grasas:N1}g");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x => { x.Span("Página "); x.CurrentPageNumber(); x.Span(" de "); x.TotalPages(); });
            });
        });

        return document.GeneratePdf();
    }

    // =====================================================
    // PDF PERSONALIZADO DE USUARIOS
    // =====================================================
    public byte[] GenerarPdfUsuarios(List<Usuario> usuarios)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().BorderBottom(1).PaddingBottom(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("DRAGON NUTREX").FontSize(24).SemiBold().FontColor(Colors.Green.Darken2);
                        col.Item().Text("Directorio de Usuarios").FontSize(14);
                        col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); columns.RelativeColumn(); 
                            columns.RelativeColumn(); columns.RelativeColumn(); 
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Nombre").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Medidas").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Dieta").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Objetivo").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Estado").SemiBold();
                        });

                        foreach (var u in usuarios)
                        {
                            var estado = u.Activo ? "Activo" : "Inactivo";
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(u.Nombre);
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text($"{u.Peso}kg / {u.Altura}cm");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(u.TipoDieta ?? "-");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(u.Objetivo ?? "-");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(estado);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x => { x.Span("Página "); x.CurrentPageNumber(); x.Span(" de "); x.TotalPages(); });
            });
        });
        return document.GeneratePdf();
    }

    // =====================================================
    // EXPORTAR PRODUCTOS (Catálogo)
    // =====================================================
    public byte[] GenerarCsvProductos(List<Producto> productos)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ID;Nombre;Calorias;Proteinas;Carbos;Grasas");
        foreach (var p in productos)
        {
            sb.AppendLine($"{p.Id};{p.Nombre};{p.Calorias};{p.Proteinas};{p.Carbohidratos};{p.Grasas}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public byte[] GenerarTxtProductos(List<Producto> productos)
    {
        var sb = new StringBuilder();
        sb.AppendLine("==================================================");
        sb.AppendLine("      CATÁLOGO DE PRODUCTOS - DRAGON NUTREX");
        sb.AppendLine("==================================================");
        sb.AppendLine();
        foreach (var p in productos)
        {
            sb.AppendLine($"🍏 {p.Nombre}");
            sb.AppendLine($"   Kcal: {p.Calorias} | Prot: {p.Proteinas}g | Carb: {p.Carbohidratos}g | Grasas: {p.Grasas}g");
            sb.AppendLine("--------------------------------------------------");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public byte[] GenerarPdfProductos(List<Producto> productos)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().BorderBottom(1).PaddingBottom(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("DRAGON NUTREX").FontSize(24).SemiBold().FontColor(Colors.Green.Darken2);
                        col.Item().Text("Catálogo de Productos").FontSize(14);
                        col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Nombre más ancho
                            columns.RelativeColumn(); columns.RelativeColumn(); 
                            columns.RelativeColumn(); columns.RelativeColumn(); 
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Producto").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Calorías").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Proteínas").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Carbos").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Grasas").SemiBold();
                        });

                        foreach (var p in productos)
                        {
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(p.Nombre);
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{p.Calorias:N1}");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{p.Proteinas:N1}g");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{p.Carbohidratos:N1}g");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{p.Grasas:N1}g");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x => { x.Span("Página "); x.CurrentPageNumber(); x.Span(" de "); x.TotalPages(); });
            });
        });
        return document.GeneratePdf();
    }

    // =====================================================
    // DESCARGA UNIVERSAL (MOTOR JAVASCRIPT)
    // =====================================================
    public async Task DescargarArchivoEnNavegador(string nombreArchivo, byte[] archivoBytes)
    {
        var stream = new MemoryStream(archivoBytes);
        using var streamRef = new DotNetStreamReference(stream);
        await _js.InvokeVoidAsync("downloadFileFromStream", nombreArchivo, streamRef);
    }
}
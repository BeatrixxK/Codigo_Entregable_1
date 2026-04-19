using System;
using System.Collections.Generic;
using System.Text;
using DragonNutrex.App.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DragonNutrex.App.Services;

public class ExportacionService
{
    public ExportacionService()
    {
        // Requisito obligatorio de QuestPDF para proyectos académicos/gratuitos
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // =====================================================
    // EXPORTAR A CSV (Texto separado por comas)
    // =====================================================
    public byte[] GenerarCsvUsuarios(List<Usuario> usuarios)
    {
        var sb = new StringBuilder();
        
        // Encabezados de las columnas
        sb.AppendLine("ID,Nombre,Peso,Altura,Dieta,Objetivo,Estado");

        foreach (var u in usuarios)
        {
            var estado = u.Activo ? "Activo" : "Inactivo";
            sb.AppendLine($"{u.Id},{u.Nombre},{u.Peso},{u.Altura},{u.TipoDieta},{u.Objetivo},{estado}");
        }

        // Convertimos el texto a un archivo de bytes (UTF8 para que acepte tildes)
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // =====================================================
    // EXPORTAR A TXT (Texto tabulado legible)
    // =====================================================
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
    // EXPORTAR A PDF 🏆 (Bonus de 10 puntos)
    // =====================================================
    public byte[] GenerarPdfEstadisticas(
        Dictionary<string, double> porcentajesDietas,
        List<KeyValuePair<string, int>> topUsuarios,
        string productoMasConsumido,
        string fechaRango)
    {
        // QuestPDF usa un "dibujante" fluido
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                // ENCABEZADO
                page.Header().BorderBottom(1).PaddingBottom(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("DRAGON NUTREX").FontSize(24).SemiBold().FontColor(Colors.Red.Medium);
                        col.Item().Text("Reporte Global de Estadísticas del Sistema").FontSize(14);
                        col.Item().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                    });
                });

                // CONTENIDO
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    // Bloque 1: Producto
                    col.Item().Text("1. Producto Más Consumido").SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2);
                    col.Item().Text($"Rango evaluado: {fechaRango}").FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().PaddingBottom(1, Unit.Centimetre).Text(productoMasConsumido);

                    // Bloque 2: Porcentajes de Dieta (Tabla)
                    col.Item().Text("2. Distribución de Dietas de Usuarios").SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2);
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

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
                    
                    // Bloque 3: Top Usuarios
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

                // PIE DE PÁGINA
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        // Retorna el archivo PDF compilado en bytes
        return document.GeneratePdf();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediGest.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.pdf.draw;
using System.Windows;
using System.Diagnostics;

namespace MediGest
{
    public class FacturaGenerator
    {
         private readonly MediGestContext db = new MediGestContext();

        public void GenerarFactura(int idPaciente, DateTime fechaInicio, DateTime fechaFin, string rutaDestino)
        {
            using (var db = new MediGestContext())
            {
                // Obtener las citas del paciente entre las fechas
                String estado = "realizada";
                var citas = (from c in db.Cita
                             join m in db.Medico on c.Id_medico equals m.Id_medico
                             join e in db.Especialidad on m.Id_especialidad equals e.Id_especialidad
                             where c.Id_paciente == idPaciente &&
                                   c.Fecha >= fechaInicio && c.Fecha <= fechaFin && c.Estado == estado
                             select new
                             {
                                 c.Fecha,
                                 Medico = m.Nombre + " " + m.Apellidos,
                                 Especialidad = e.Nombre,
                                 e.Precio,
                                 c.Observaciones
                             }).ToList();

                if (!citas.Any())
                    throw new Exception("No hay citas realizadas con el paciente en ese rango de fechas.");


                // Obtener datos del paciente
                var paciente = db.Paciente.FirstOrDefault(p => p.Id_paciente == idPaciente);
                if (paciente == null)
                    throw new Exception("No se encontró el paciente.");

                // Crear pdf del informe
                Document doc = new Document(PageSize.A4, 50, 50, 50, 50);

                using (FileStream fs = new FileStream(rutaDestino, FileMode.Create))
                {
                    PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // ==================== LOGO ====================
                    string projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                    string rutaLogo = System.IO.Path.Combine(projectPath, "Resources", "logo.jpg");  // Ajusta tu ruta
                    if (File.Exists(rutaLogo))
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(rutaLogo);

                        // Puedes ajustar el tamaño
                        logo.ScaleToFit(120f, 120f);

                        // Alinear a la izquierda o centro:
                        logo.Alignment = Element.ALIGN_LEFT; // o Element.ALIGN_CENTER

                        doc.Add(logo);
                        doc.Add(new Paragraph("\n"));
                    }
                    // ==============================================

                    // Fuentes
                    var tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);

                    // Título principal
                    Paragraph titulo = new Paragraph("FACTURA MÉDICA", tituloFont);
                    titulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(titulo);

                    doc.Add(new Paragraph("\n"));
                    doc.Add(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -2));

                    // Datos del paciente
                    doc.Add(new Paragraph($"Paciente: {paciente.Nombre} {paciente.Apellidos}", normalFont));
                    doc.Add(new Paragraph($"DNI: {paciente.Dni}", normalFont));
                    doc.Add(new Paragraph($"Periodo: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}", normalFont));
                    doc.Add(new Paragraph("\n"));

                    // Título de sección
                    Paragraph detalleTitulo = new Paragraph("Detalle de citas:", boldFont);
                    detalleTitulo.SpacingBefore = 10;
                    detalleTitulo.SpacingAfter = 5;
                    doc.Add(detalleTitulo);

                    // Crear tabla
                    PdfPTable tabla = new PdfPTable(5);
                    tabla.WidthPercentage = 100;
                    tabla.SetWidths(new float[] { 25f, 35f, 25f, 15f,35f });

                    // Encabezados
                    tabla.AddCell(new PdfPCell(new Phrase("Fecha", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabla.AddCell(new PdfPCell(new Phrase("Médico", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabla.AddCell(new PdfPCell(new Phrase("Especialidad", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabla.AddCell(new PdfPCell(new Phrase("Precio", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabla.AddCell(new PdfPCell(new Phrase("Observaciones", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                    decimal total = 0;
                    foreach (var cita in citas)
                    {
                        tabla.AddCell(new Phrase(cita.Fecha.ToString("dd/MM/yyyy"), normalFont));
                        tabla.AddCell(new Phrase(cita.Medico, normalFont));
                        tabla.AddCell(new Phrase(cita.Especialidad, normalFont));
                        tabla.AddCell(new Phrase($"{cita.Precio:C}", normalFont));
                        tabla.AddCell(new Phrase(cita.Observaciones,normalFont));
                        total += cita.Precio;
                    }

                    doc.Add(tabla);
                    doc.Add(new Paragraph("\n"));

                    // Monto
                    Paragraph totalTxt = new Paragraph($"TOTAL A PAGAR: {total:C}", boldFont);
                    totalTxt.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(totalTxt);

                    doc.Add(new Paragraph("\n"));
                    doc.Add(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -2));

                    // Fecha de emisión
                    Paragraph fechaEmision = new Paragraph($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont);
                    fechaEmision.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(fechaEmision);

                    // Cierre
                    doc.Close();

                    // ✅ Confirmación al usuario
                    MessageBox.Show($"Factura generada correctamente en:\n{rutaDestino}",
                                    "Factura creada", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = rutaDestino,
                        UseShellExecute = true
                    };
                    Process.Start(psi);

                    MessageBox.Show(
                        $"Informe facturacion generado correctamente y abierto.\n\nUbicación: {rutaDestino}",
                        "Informe creado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"El informe se genero correctamente en:\n{rutaDestino}\n\nPero no se pudo abrir automaticamente:\n{ex.Message}",
                        "Informe creado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
        }
    }
}

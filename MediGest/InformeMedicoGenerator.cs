using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediGest.Data;
using MediGest.Clases;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.pdf.draw;
using System.Windows;

namespace MediGest
{
    public class InformeMedicoGenerator
    {
        public void GenerarInformeMedicoPDF(int idPaciente, string rutaDestino)
        {
            using (var db = new MediGestContext())
            {
                // Obtener datos del paciente
                var paciente = db.Paciente.FirstOrDefault(p => p.Id_paciente == idPaciente);
                if (paciente == null)
                    throw new Exception("No se encontro el paciente.");

                // Obtener todos los informes m�dicos del paciente
                var informes = (from informe in db.Informe_Medico
                                join medico in db.Medico on informe.Id_medico equals medico.Id_medico
                                join cita in db.Cita on informe.Id_cita equals cita.Id_cita
                                join especialidad in db.Especialidad on medico.Id_especialidad equals especialidad.Id_especialidad
                                where informe.Id_paciente == idPaciente
                                orderby informe.Fecha_emision descending
                                select new
                                {
                                    informe.Id_informe,
                                    informe.Fecha_emision,
                                    informe.Motivo_consulta,
                                    informe.Diagnostico,
                                    informe.Tratamiento,
                                    informe.Observaciones,
                                    MedicoNombre = medico.Nombre + " " + medico.Apellidos,
                                    medico.Num_colegiado,
                                    Especialidad = especialidad.Nombre,
                                    cita.Fecha,
                                    cita.Hora
                                }).ToList();

                if (!informes.Any())
                    throw new Exception("No hay informes m�dicos registrados para este paciente.");

                // Crear documento PDF
                Document doc = new Document(PageSize.A4, 50, 50, 50, 50);

                using (FileStream fs = new FileStream(rutaDestino, FileMode.Create))
                {
                    PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Fuentes
                    var tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                    var subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.BLACK);
                    var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY);

                    // T�tulo principal
                    Paragraph titulo = new Paragraph("HISTORIAL MEDICO DEL PACIENTE", tituloFont);
                    titulo.Alignment = Element.ALIGN_CENTER;
                    titulo.SpacingAfter = 15;
                    doc.Add(titulo);

                    doc.Add(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -2));
                    doc.Add(new Paragraph("\n"));

                    // Datos del paciente
                    Paragraph datosPaciente = new Paragraph("DATOS DEL PACIENTE", subtituloFont);
                    datosPaciente.SpacingBefore = 5;
                    datosPaciente.SpacingAfter = 10;
                    doc.Add(datosPaciente);

                    doc.Add(new Paragraph($"Nombre completo: {paciente.Nombre} {paciente.Apellidos}", normalFont));
                    doc.Add(new Paragraph($"DNI: {paciente.Dni}", normalFont));
                    doc.Add(new Paragraph($"CIPA: {paciente.Cipa}", normalFont));
                    doc.Add(new Paragraph($"Nº Historia Clinica: {paciente.Num_historia_clinica}", normalFont));
                    doc.Add(new Paragraph($"Nº Seguridad Social: {paciente.Num_seguridad_social}", normalFont));
                    doc.Add(new Paragraph($"Fecha de Nacimiento: {paciente.Fecha_nacimiento:dd/MM/yyyy}", normalFont));

                    int edad = DateTime.Now.Year - paciente.Fecha_nacimiento.Year;
                    if (DateTime.Now.DayOfYear < paciente.Fecha_nacimiento.DayOfYear)
                        edad--;
                    doc.Add(new Paragraph($"Edad: {edad} años", normalFont));

                    doc.Add(new Paragraph("\n"));
                    doc.Add(new LineSeparator(0.5f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2));
                    doc.Add(new Paragraph("\n"));

                    // Resumen de consultas
                    Paragraph resumen = new Paragraph("RESUMEN DE CONSULTAS", subtituloFont);
                    resumen.SpacingAfter = 10;
                    doc.Add(resumen);

                    doc.Add(new Paragraph($"Total de consultas registradas: {informes.Count}", normalFont));
                    doc.Add(new Paragraph($"Primera consulta: {informes.Last().Fecha_emision:dd/MM/yyyy}", normalFont));
                    doc.Add(new Paragraph($"�ltima consulta: {informes.First().Fecha_emision:dd/MM/yyyy}", normalFont));

                    doc.Add(new Paragraph("\n"));
                    doc.Add(new LineSeparator(0.5f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2));
                    doc.Add(new Paragraph("\n"));

                    // Detalle de cada informe m�dico
                    Paragraph detalleInformes = new Paragraph("HISTORIAL DE INFORMES MEDICOS", subtituloFont);
                    detalleInformes.SpacingAfter = 15;
                    doc.Add(detalleInformes);

                    int contador = 1;
                    foreach (var informe in informes)
                    {
                        // Encabezado del informe
                        Paragraph numeroInforme = new Paragraph($"INFORME #{contador} - ID: {informe.Id_informe}", boldFont);
                        numeroInforme.SpacingBefore = 10;
                        numeroInforme.SpacingAfter = 5;
                        doc.Add(numeroInforme);

                        // Crear tabla para informaci�n estructurada
                        PdfPTable tablaInfo = new PdfPTable(2);
                        tablaInfo.WidthPercentage = 100;
                        tablaInfo.SetWidths(new float[] { 35f, 65f });
                        tablaInfo.SpacingAfter = 10;

                        // Fecha de emisi�n
                        PdfPCell celdaTituloFecha = new PdfPCell(new Phrase("Fecha de emisión:", boldFont));
                        celdaTituloFecha.Border = Rectangle.NO_BORDER;
                        celdaTituloFecha.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaTituloFecha);

                        PdfPCell celdaFecha = new PdfPCell(new Phrase(informe.Fecha_emision.ToString("dd/MM/yyyy"), normalFont));
                        celdaFecha.Border = Rectangle.NO_BORDER;
                        celdaFecha.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaFecha);

                        // Fecha de cita
                        PdfPCell celdaTituloCita = new PdfPCell(new Phrase("Fecha de cita:", boldFont));
                        celdaTituloCita.Border = Rectangle.NO_BORDER;
                        celdaTituloCita.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaTituloCita);

                        PdfPCell celdaCita = new PdfPCell(new Phrase($"{informe.Fecha:dd/MM/yyyy} - {informe.Hora}", normalFont));
                        celdaCita.Border = Rectangle.NO_BORDER;
                        celdaCita.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaCita);

                        // M�dico
                        PdfPCell celdaTituloMedico = new PdfPCell(new Phrase("Médico:", boldFont));
                        celdaTituloMedico.Border = Rectangle.NO_BORDER;
                        celdaTituloMedico.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaTituloMedico);

                        PdfPCell celdaMedico = new PdfPCell(new Phrase(informe.MedicoNombre, normalFont));
                        celdaMedico.Border = Rectangle.NO_BORDER;
                        celdaMedico.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaMedico);

                        // Especialidad
                        PdfPCell celdaTituloEsp = new PdfPCell(new Phrase("Especialidad:", boldFont));
                        celdaTituloEsp.Border = Rectangle.NO_BORDER;
                        celdaTituloEsp.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaTituloEsp);

                        PdfPCell celdaEsp = new PdfPCell(new Phrase(informe.Especialidad, normalFont));
                        celdaEsp.Border = Rectangle.NO_BORDER;
                        celdaEsp.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaEsp);

                        // N� Colegiado
                        PdfPCell celdaTituloCol = new PdfPCell(new Phrase("Nº Colegiado:", boldFont));
                        celdaTituloCol.Border = Rectangle.NO_BORDER;
                        celdaTituloCol.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaTituloCol);

                        PdfPCell celdaCol = new PdfPCell(new Phrase(informe.Num_colegiado, normalFont));
                        celdaCol.Border = Rectangle.NO_BORDER;
                        celdaCol.PaddingBottom = 5;
                        tablaInfo.AddCell(celdaCol);

                        doc.Add(tablaInfo);

                        // Motivo de consulta
                        if (!string.IsNullOrWhiteSpace(informe.Motivo_consulta))
                        {
                            Paragraph motivoTitulo = new Paragraph("Motivo de consulta:", boldFont);
                            motivoTitulo.SpacingBefore = 5;
                            motivoTitulo.SpacingAfter = 3;
                            doc.Add(motivoTitulo);
                            doc.Add(new Paragraph(informe.Motivo_consulta, normalFont));
                        }

                        // Diagn�stico
                        if (!string.IsNullOrWhiteSpace(informe.Diagnostico))
                        {
                            Paragraph diagnosticoTitulo = new Paragraph("Diagnostico:", boldFont);
                            diagnosticoTitulo.SpacingBefore = 5;
                            diagnosticoTitulo.SpacingAfter = 3;
                            doc.Add(diagnosticoTitulo);
                            doc.Add(new Paragraph(informe.Diagnostico, normalFont));
                        }

                        // Tratamiento
                        if (!string.IsNullOrWhiteSpace(informe.Tratamiento))
                        {
                            Paragraph tratamientoTitulo = new Paragraph("Tratamiento prescrito:", boldFont);
                            tratamientoTitulo.SpacingBefore = 5;
                            tratamientoTitulo.SpacingAfter = 3;
                            doc.Add(tratamientoTitulo);
                            doc.Add(new Paragraph(informe.Tratamiento, normalFont));
                        }

                        // Observaciones
                        if (!string.IsNullOrWhiteSpace(informe.Observaciones))
                        {
                            Paragraph observacionesTitulo = new Paragraph("Observaciones:", boldFont);
                            observacionesTitulo.SpacingBefore = 5;
                            observacionesTitulo.SpacingAfter = 3;
                            doc.Add(observacionesTitulo);
                            doc.Add(new Paragraph(informe.Observaciones, normalFont));
                        }

                        // Separador entre informes
                        if (contador < informes.Count)
                        {
                            doc.Add(new Paragraph("\n"));
                            doc.Add(new LineSeparator(0.5f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2));
                        }

                        contador++;
                    }

                    // Pie de p�gina
                    doc.Add(new Paragraph("\n\n"));
                    doc.Add(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -2));

                    Paragraph fechaGeneracion = new Paragraph(
                        $"Documento generado el {DateTime.Now:dd/MM/yyyy} a las {DateTime.Now:HH:mm}",
                        smallFont
                    );
                    fechaGeneracion.Alignment = Element.ALIGN_CENTER;
                    fechaGeneracion.SpacingBefore = 10;
                    doc.Add(fechaGeneracion);

                    Paragraph confidencial = new Paragraph(
                        "DOCUMENTO CONFIDENCIAL - USO MEDICO EXCLUSIVO",
                        smallFont
                    );
                    confidencial.Alignment = Element.ALIGN_CENTER;
                    confidencial.SpacingBefore = 5;
                    doc.Add(confidencial);

                    doc.Close();
                }

                // ?? Abrir el PDF autom�ticamente despues de crearlo
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = rutaDestino,
                        UseShellExecute = true
                    };
                    Process.Start(psi);

                    MessageBox.Show(
                        $"Informe m�dico generado correctamente y abierto.\n\nUbicaci�n: {rutaDestino}",
                        "Informe creado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"El informe se gener� correctamente en:\n{rutaDestino}\n\nPero no se pudo abrir autom�ticamente:\n{ex.Message}",
                        "Informe creado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
        }
    }
}
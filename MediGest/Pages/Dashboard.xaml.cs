using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using MediGest.Clases;
using MediGest.Data;

namespace MediGest.Pages
{
    /// <summary>
    /// Lógica de interacción para Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            CitasDiarias();
            ConsultasMensuales();
            PacientesMensuales();
            FacturacionMensual();
            MostrarCitasDeHoy();
        }

        private void CitasDiarias() {
            using (var db = new MediGestContext())
            {
                var hoy = DateTime.Today;
                var ayer = hoy.AddDays(-1);
                if (SessionManager.Rol == "Medico")
                {
                    int citasHoy = db.Cita.Count(c => c.Fecha.Date == hoy && c.Id_medico == SessionManager.IdUsuario);
                    int citasAyer = db.Cita.Count(c => c.Fecha.Date == ayer && c.Id_medico == SessionManager.IdUsuario);
                    double variacion = CalcularVariacion(citasHoy, citasAyer);
                    string tendencia = ObtenerTendencia(variacion);
                    txtCitasDiarias.Text = citasHoy.ToString();
                    variacionCitas.Text = $"{tendencia} {variacion:+0.##% mas que ayer;-0.##% menos que ayer;Rendimiento igual al que hubo ayer}";
                }
                else {
                    int citasHoy = db.Cita.Count(c => c.Fecha.Date == hoy && c.Id_recepcionista == SessionManager.IdUsuario);
                    int citasAyer = db.Cita.Count(c => c.Fecha.Date == ayer && c.Id_recepcionista == SessionManager.IdUsuario);

                    double variacion = CalcularVariacion(citasHoy, citasAyer);
                    string tendencia = ObtenerTendencia(variacion);
                    txtCitasDiarias.Text = citasHoy.ToString();
                    variacionCitas.Text = $"{tendencia} {variacion:+0.##% mas que ayer;-0.##% menos que ayer;Rendimiento igual al que hubo ayer}";
                }
                
            }
        }

        private void ConsultasMensuales()
        {
            using (var db = new MediGestContext())
            {
                DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                if (SessionManager.Rol == "Medico")
                {
                    int totalInformesMensuales = db.Informe_Medico.Count(i => i.Fecha_emision >= inicioMes && i.Id_medico == SessionManager.IdUsuario);
                    txtConsultasMensuales.Text = totalInformesMensuales.ToString();
                    int mesAnterior = inicioMes.AddMonths(-1).Month;
                    int anioActual = inicioMes.Year;
                    int informesMensualesAnteriores = db.Informe_Medico.Count(i => i.Fecha_emision.Month == mesAnterior && i.Fecha_emision.Year == anioActual && i.Id_medico == SessionManager.IdUsuario);
                    double variacion = CalcularVariacion(totalInformesMensuales, informesMensualesAnteriores);
                    string tendencia = ObtenerTendencia(variacion);
                    variacionInformes.Text = $"{tendencia} {variacion:+0.##% mas que el mes pasado;-0.##% menos que el mes pasado;Rendimiento igual al del mes pasado}";
                }
                else {
                    int totalInformesMensuales = db.Informe_Medico.Count(i => i.Fecha_emision >= inicioMes);
                    txtConsultasMensuales.Text = totalInformesMensuales.ToString();
                    int mesAnterior = inicioMes.AddMonths(-1).Month;
                    int anioActual = inicioMes.Year;
                    int informesMensualesAnteriores = db.Informe_Medico.Count(i => i.Fecha_emision.Month == mesAnterior && i.Fecha_emision.Year == anioActual);
                    double variacion = CalcularVariacion(totalInformesMensuales, informesMensualesAnteriores);
                    string tendencia = ObtenerTendencia(variacion);
                    variacionInformes.Text = $"{tendencia} {variacion:+0.##% mas que el mes pasado;-0.##% menos que el mes pasado;Rendimiento igual al del mes pasado}";
                }
            }
        }

        private void PacientesMensuales() {

        }

        private void FacturacionMensual() {
            string rutaFacturas = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Facturaciones");
            if (!Directory.Exists(rutaFacturas))
            {
                txtFacturacionMensual.Text = "0 €";
                return;
            }

            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            double totalFacturado = 0;
            DateTime mesAnterior = inicioMes.AddMonths(-1);
            double totalFacturadoAnterior = 0; 

            foreach (string archivo in Directory.GetFiles(rutaFacturas, "*.pdf"))
            {
                DateTime fechaArchivo = File.GetCreationTime(archivo);
                if (fechaArchivo >= inicioMes)
                {
                   
                    try
                    {
                        using (PdfReader reader = new PdfReader(archivo))
                        {
                            StringBuilder texto = new StringBuilder();

                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                texto.Append(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i));
                            }

                            string contenido = texto.ToString();

                            // 🔹 Buscamos la línea que contiene "TOTAL A PAGAR"
                            var match = System.Text.RegularExpressions.Regex.Match(
                                contenido,
                                @"TOTAL A PAGAR:\s*([\d,.]+)",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase
                            );

                            if (match.Success)
                            {
                                string valor = match.Groups[1].Value
                                    .Replace(",", ".")
                                    .Trim();

                                if (double.TryParse(valor, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out double montoEncontrado))
                                {
                                    totalFacturado += montoEncontrado;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }

                if(fechaArchivo.Month == mesAnterior.Month && fechaArchivo.Year == mesAnterior.Year)
                {
                    try
                    {
                        using (PdfReader reader = new PdfReader(archivo))
                        {
                            StringBuilder texto = new StringBuilder();

                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                texto.Append(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i));
                            }

                            string contenido = texto.ToString();

                            // 🔹 Buscamos la línea que contiene "TOTAL A PAGAR"
                            var match = System.Text.RegularExpressions.Regex.Match(
                                contenido,
                                @"TOTAL A PAGAR:\s*([\d,.]+)",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase
                            );

                            if (match.Success)
                            {
                                string valor = match.Groups[1].Value
                                    .Replace(",", ".")
                                    .Trim();

                                if (double.TryParse(valor, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out double montoEncontrado))
                                {
                                    totalFacturadoAnterior += montoEncontrado;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }

            }

            txtFacturacionMensual.Text = $"{Math.Round(totalFacturado, 2):0.00} €";
            double variacion = CalcularVariacion(totalFacturado,totalFacturadoAnterior);
            String tendencia = ObtenerTendencia(variacion);
            variacionFacturaciones.Text = $"{tendencia} {variacion:+0.##% mas que el mes pasado;-0.##% menos que el mes pasado;Rendimiento igual al del mes pasado}";
        }


        private void MostrarCitasDeHoy()
        {
            using (var db = new MediGestContext())
            {
                var hoy = DateTime.Today;
                if (SessionManager.Rol == "Medico")
                {
                    var citasHoy = (from cita in db.Cita
                                    join paciente in db.Paciente
                                    on cita.Id_paciente equals paciente.Id_paciente
                                    where cita.Fecha.Date == hoy && cita.Id_medico == SessionManager.IdUsuario
                                    orderby cita.Fecha
                                    select new
                                    {
                                        Nombre = paciente.Nombre + " " + paciente.Apellidos,
                                        Hora = cita.Hora,
                                        Motivo = cita.Observaciones,
                                        Estado = cita.Estado
                                    }).ToList();

                    panelCitasDeHoy.Children.Clear();

                    if (!citasHoy.Any())
                    {
                        panelCitasDeHoy.Children.Add(new TextBlock
                        {
                            Text = "No hay citas programadas para hoy.",
                            FontSize = 14,
                            Foreground = new SolidColorBrush(Colors.Gray),
                            Margin = new Thickness(0, 10, 0, 0)
                        });
                        return;
                    }

                    foreach (var cita in citasHoy)
                    {
                        // Color según estado
                        Brush fondoEstado = cita.Estado.ToLower() switch
                        {
                            "confirmada" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dcfce7")),
                            "pendiente" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fef9c3")),
                            "cancelada" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fee2e2")),
                            _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f1f5f9"))
                        };

                        // Contenedor individual
                        Border border = new Border
                        {
                            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e2e8f0")),
                            BorderThickness = new Thickness(0, 0, 0, 1),
                            Padding = new Thickness(0, 0, 0, 16),
                            Margin = new Thickness(0, 0, 0, 16)
                        };

                        Grid grid = new Grid();
                        StackPanel info = new StackPanel { Orientation = Orientation.Horizontal };

                        Ellipse avatar = new Ellipse
                        {
                            Width = 36,
                            Height = 36,
                            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eff6ff")),
                            Margin = new Thickness(0, 0, 12, 0)
                        };

                        StackPanel datos = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                        datos.Children.Add(new TextBlock
                        {
                            Text = cita.Nombre,
                            FontWeight = FontWeights.Medium
                        });
                        datos.Children.Add(new TextBlock
                        {
                            Text = $"{cita.Hora} - {cita.Estado}",
                            FontSize = 12,
                            Margin = new Thickness(0, 2, 0, 0)
                        });

                        info.Children.Add(avatar);
                        info.Children.Add(datos);

                        Border badge = new Border
                        {
                            Background = fondoEstado,
                            CornerRadius = new CornerRadius(12),
                            Padding = new Thickness(8, 4, 8, 4),
                            HorizontalAlignment = HorizontalAlignment.Right
                        };
                        badge.Child = new TextBlock
                        {
                            Text = cita.Estado.ToLower(),
                            FontSize = 11,
                            FontWeight = FontWeights.Medium
                        };

                        grid.Children.Add(info);
                        grid.Children.Add(badge);

                        border.Child = grid;
                        panelCitasDeHoy.Children.Add(border);
                    }
                }
                else
                {
                    var citasHoy = (from cita in db.Cita
                                    join paciente in db.Paciente
                                    on cita.Id_paciente equals paciente.Id_paciente
                                    where cita.Fecha.Date == hoy && cita.Id_recepcionista == SessionManager.IdUsuario
                                    orderby cita.Fecha
                                    select new
                                    {
                                        Nombre = paciente.Nombre + " " + paciente.Apellidos,
                                        Hora = cita.Hora,
                                        Motivo = cita.Observaciones,
                                        Estado = cita.Estado
                                    }).ToList();

                    panelCitasDeHoy.Children.Clear();

                    if (!citasHoy.Any())
                    {
                        panelCitasDeHoy.Children.Add(new TextBlock
                        {
                            Text = "No hay citas programadas para hoy.",
                            FontSize = 14,
                            Foreground = new SolidColorBrush(Colors.Gray),
                            Margin = new Thickness(0, 10, 0, 0)
                        });
                        return;
                    }

                    foreach (var cita in citasHoy)
                    {
                        // Color según estado
                        Brush fondoEstado = cita.Estado.ToLower() switch
                        {
                            "confirmada" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dcfce7")),
                            "pendiente" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fef9c3")),
                            "cancelada" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fee2e2")),
                            _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f1f5f9"))
                        };

                        // Contenedor individual
                        Border border = new Border
                        {
                            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e2e8f0")),
                            BorderThickness = new Thickness(0, 0, 0, 1),
                            Padding = new Thickness(0, 0, 0, 16),
                            Margin = new Thickness(0, 0, 0, 16)
                        };

                        Grid grid = new Grid();
                        StackPanel info = new StackPanel { Orientation = Orientation.Horizontal };

                        Ellipse avatar = new Ellipse
                        {
                            Width = 36,
                            Height = 36,
                            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eff6ff")),
                            Margin = new Thickness(0, 0, 12, 0)
                        };

                        StackPanel datos = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                        datos.Children.Add(new TextBlock
                        {
                            Text = cita.Nombre,
                            FontWeight = FontWeights.Medium
                        });
                        datos.Children.Add(new TextBlock
                        {
                            Text = $"{cita.Hora} - {cita.Estado}",
                            FontSize = 12,
                            Margin = new Thickness(0, 2, 0, 0)
                        });

                        info.Children.Add(avatar);
                        info.Children.Add(datos);

                        Border badge = new Border
                        {
                            Background = fondoEstado,
                            CornerRadius = new CornerRadius(12),
                            Padding = new Thickness(8, 4, 8, 4),
                            HorizontalAlignment = HorizontalAlignment.Right
                        };
                        badge.Child = new TextBlock
                        {
                            Text = cita.Estado.ToLower(),
                            FontSize = 11,
                            FontWeight = FontWeights.Medium
                        };

                        grid.Children.Add(info);
                        grid.Children.Add(badge);

                        border.Child = grid;
                        panelCitasDeHoy.Children.Add(border);
                    }
                }
                }
            }


        private void RefrescarDashboard() {
            CitasDiarias();
            ConsultasMensuales();
            PacientesMensuales();
            FacturacionMensual();
            MostrarCitasDeHoy();
        }

        private void BtnRegistarPacienteClick(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Rol == "Medico")
            {
                MessageBox.Show("No tienes permisos para Realizar esta Accion");
            }
            else {
                var form = new PanelPaciente();
                form.ShowDialog();
                RefrescarDashboard();
            }
        }

        private void BtnAgendarCitaClick(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Rol == "Medico")
            {
                MessageBox.Show("No tienes permisos para Realizar esta Accion");
            }
            else {
                var form = new AgendarCita();
                form.ShowDialog();
                RefrescarDashboard();
            }
        }

        private void BtnGenerarFacturaClick(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Rol == "Medico")
            {
                MessageBox.Show("No tienes permisos para Realizar esta Accion");
                return;
            }
            var form = new GenerarFactura();
            form.ShowDialog();
            RefrescarDashboard();
        }

        private double CalcularVariacion(double actual, double anterior)
        {
            if (anterior == 0) {
                return actual / 100.0;
            }
            return ((actual - anterior) / anterior) * 100.0;
        }

        private string ObtenerTendencia(double variacion)
        {
            if (variacion > 0) return "🔼";
            else if (variacion < 0) return "🔽";
            else return "⏸️";
        }
    }
}

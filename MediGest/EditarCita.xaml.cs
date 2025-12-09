using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MediGest.Clases;
using MediGest.Data;
using MediGest.Pages;
using MediGest.Servicios;

namespace MediGest
{
    /// <summary>
    /// Lógica de interacción para EditarCita.xaml
    /// </summary>
    public partial class EditarCita : Window
    {
        private Cita citaActual;
        public EditarCita(Cita cita)
        {
            InitializeComponent();
            citaActual = cita;
            CargarHoras();
            CargarEstados();
            CargarInformacion();
        }

        private void CargarInformacion() {
            if (SessionManager.Rol == "Medico")
            {
                cmbEstado.SelectedItem = cmbEstado.Items.Cast<string>()
          .FirstOrDefault(e => e.Equals(citaActual.Estado, StringComparison.OrdinalIgnoreCase));
                dpFechaCita.IsEnabled = false;
                cmbHora.IsEnabled = false;
                txtObservaciones.IsEnabled = false;
            }
            else {

                dpFechaCita.SelectedDate = citaActual.Fecha;
                cmbHora.SelectedItem = citaActual.Hora.ToString(@"hh\:mm");
                cmbEstado.SelectedItem = cmbEstado.Items.Cast<string>()
           .FirstOrDefault(e => e.Equals(citaActual.Estado, StringComparison.OrdinalIgnoreCase));
                txtObservaciones.Text = citaActual.Observaciones;
            }
        }

        private void CargarHoras()
        {
            for (int hora = 9; hora < 21;hora++)
            {
                cmbHora.Items.Add($"{hora:00}:00");
                cmbHora.Items.Add($"{hora:00}:30");
            }
        }

        private void CargarEstados() {
            cmbEstado.Items.Add("Pendiente");
            cmbEstado.Items.Add("Confirmada");
            cmbEstado.Items.Add("Cancelada");
            cmbEstado.Items.Add("Realizada");
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "¿Estas seguro de Cancelar?",
                "Confirmacion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Rol == "Medico" && cmbEstado.Text != "Realizada") {
                MessageBox.Show("Solo tienes permiso para Realizar Citas no otra cosa.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SessionManager.Rol == "Recepcionista" && dpFechaCita.SelectedDate == null)
            {
                MessageBox.Show("Selecciona una fecha para la cita.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SessionManager.Rol == "Recepcionista" && string.IsNullOrEmpty(cmbHora.Text))
            {
                MessageBox.Show("Selecciona una hora válida.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new MediGestContext())
                {
                    var citaBD = db.Cita.FirstOrDefault(c => c.Id_cita == citaActual.Id_cita);

                    if (citaBD == null)
                    {
                        MessageBox.Show("No se encontró la cita en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (SessionManager.Rol == "Recepcionista")
                    {

                        //Verificacion de que no sea un duplicado
                        bool existeCitaDuplicada = db.Cita.Any(c =>
                        c.Id_medico == citaActual.Id_medico &&
                        c.Fecha == dpFechaCita.SelectedDate.Value &&
                        c.Hora == TimeSpan.Parse(cmbHora.Text) &&
                        c.Estado == cmbEstado.Text &&
                        c.Id_cita != citaActual.Id_cita // ← evita que se compare consigo misma
                        );

                        if (existeCitaDuplicada)
                        {
                            MessageBox.Show("⚠️ Ya existe una cita para este médico en esa fecha y hora.",
                                            "Cita duplicada", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Actualizamos campos modificables
                    if (SessionManager.Rol == "Medico")
                    {
                        citaBD.Estado = cmbEstado.Text;
                    }
                    else {
                        citaBD.Fecha = dpFechaCita.SelectedDate.Value;
                        citaBD.Hora = TimeSpan.Parse(cmbHora.Text);
                        citaBD.Estado = cmbEstado.Text;
                        citaBD.Observaciones = txtObservaciones.Text;
                    }
                    if (citaBD.Estado.Equals("Realizada", StringComparison.OrdinalIgnoreCase))
                    {
                        if (citaBD.Fecha > DateTime.Now)
                        {
                            MessageBox.Show("Una cita no puede haberse realizado despues de hoy");
                            return;
                        }
                        MessageBox.Show("Cita Realizada procediendo a la Creacion de su respectivo informe");
                        var nuevoInforme = new CrearInformeMedico(citaBD);
                        nuevoInforme.ShowDialog();
                    }

                    db.SaveChanges();
                    MessageBox.Show("✅ Cita actualizada correctamente.\n\n" +
                       "Se enviará un correo automáticamente al paciente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    //enviar correo
                    try
                    {
                        var paciente = db.Paciente.FirstOrDefault(x => x.Id_paciente == citaBD.Id_paciente);
                        var medico = db.Medico.FirstOrDefault(x => x.Id_medico == citaBD.Id_medico);

                        string projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                        string rutaPlantilla = System.IO.Path.Combine(projectPath, "Resources", "citaEditada.html");
                        var emailService = new EmailService("medicosmedigestinforma@gmail.com");

                        string html = emailService.CargarPlantilla(rutaPlantilla);

                        String mensajeOpcional = "";

                        switch (citaBD.Estado) {
                            case "Cancelada":
                                mensajeOpcional = "<p style='color:#ef4444; font-weight:bold;'>⚠ Tu cita ha sido cancelada.</p>";
                                break;
                            case "Realizada":
                                mensajeOpcional = "<p style='color:#10b981; font-weight:bold;'>😊 Tu cita ha sido Realizada.</p>";
                                break;
                            default:
                                mensajeOpcional = "<p style='color:#10b981; font-weight:bold;'>😊 Tu cita ha sido actualizada correctamente.</p>";
                                break;
                        }

                        html = html.Replace("{{PacienteNombre}}", paciente.Nombre + " " + paciente.Apellidos)
                                   .Replace("{{MedicoNombre}}", medico.Nombre + " " + medico.Apellidos)
                                   .Replace("{{Fecha}}", citaBD.Fecha.ToString("dd/MM/yyyy"))
                                   .Replace("{{Hora}}", citaBD.Hora.ToString(@"hh\:mm"))
                                   .Replace("{{NuevoEstado}}", citaBD.Estado)
                                   .Replace("{{MensajeOpcional}}", mensajeOpcional);

                        string rutaLogo = System.IO.Path.Combine(projectPath, "Resources", "logo.jpg");
                        emailService.EnviarCorreo(paciente.Correo, "Actualización de tu cita médica", html, rutaLogo);
                        MessageBox.Show("Correo Enviado Exitosamente");
                    }
                    catch { /* si falla el correo, no afecta el guardado */ }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los cambios:\n{ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

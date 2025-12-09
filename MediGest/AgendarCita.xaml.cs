using MediGest.Clases;
using MediGest.Data;
using MediGest.Servicios;
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
using System.Windows.Shapes;
using MediGest.Clases;
using MediGest.Data;
using MediGest.Servicios;
using Org.BouncyCastle.Crypto.Macs;

namespace MediGest
{
    /// <summary>
    /// Lógica de interacción para AgendarCita.xaml
    /// </summary>
    public partial class AgendarCita : Window
    {
        public AgendarCita()
        {
            InitializeComponent();
            CalendarioCitas.DisplayDateStart = DateTime.Today;
            CalendarioCitas.DisplayDate = DateTime.Today;
            CargarPacientes();
            CargarMedicos();
            CargarHoras();
        }

        private void CargarPacientes() {

            using (var db = new MediGestContext()) {

                var pacientes = db.Paciente
                    .Select(p => new { p.Id_paciente, NombreCompleto = p.Nombre + " " + p.Apellidos})
                    .ToList();

                CmbPaciente.ItemsSource = pacientes;
                CmbPaciente.DisplayMemberPath = "NombreCompleto";
                CmbPaciente.SelectedValuePath = "Id_paciente";
                CmbPaciente.SelectedItem = 1;
            }
        }

        // 🔹 Cargar lista de médicos desde la BD
        private void CargarMedicos()
        {
            using (var db = new MediGestContext())
            {
                var medicos = db.Medico
                    .Select(m => new { m.Id_medico, NombreCompleto = m.Nombre + " " + m.Apellidos })
                    .ToList();

                CmbMedico.ItemsSource = medicos;
                CmbMedico.DisplayMemberPath = "NombreCompleto";
                CmbMedico.SelectedValuePath = "Id_medico";
                CmbMedico.SelectedItem = 1;
            }
        }

        // 🔹 Cargar franjas horarias
        private void CargarHoras()
        {
            for (int hora = 9; hora < 21; hora++)
            {
                CmbHora.Items.Add($"{hora:00}:00");
                CmbHora.Items.Add($"{hora:00}:30");
                CmbHora.SelectedItem = 1;
            }
        }

        

        private void CalendarioCitas_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarioCitas.SelectedDate != null)
            {
                DateTime fechaSeleccionada = CalendarioCitas.SelectedDate.Value;
                MessageBox.Show($"Has seleccionado el día {fechaSeleccionada:dd/MM/yyyy}",
                                "Fecha seleccionada",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAgendar_Click(object sender, RoutedEventArgs e)
        {
            if (CalendarioCitas.SelectedDate == null)
            {
                MessageBox.Show("Selecciona una fecha en el calendario.");
                return;
            }

            if (CmbMedico.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un médico.");
                return;
            }

            if (string.IsNullOrEmpty(CmbHora.Text))
            {
                MessageBox.Show("Selecciona una hora.");
                return;
            }


            try
            {
                using (var db = new MediGestContext())
                {
                    
                    var nuevaCita = new Cita
                    {
                        Id_paciente = (int)CmbPaciente.SelectedValue,
                        Id_medico = (int)CmbMedico.SelectedValue,
                        Id_recepcionista = SessionManager.IdUsuario,
                        Fecha = CalendarioCitas.SelectedDate.Value,
                        Hora = TimeSpan.Parse(CmbHora.Text),
                        Estado = CmbEstado.Text,
                        Observaciones = TxtObservaciones.Text
                    };

                    bool existeCitaDuplicada = db.Cita.Any(c =>
                   c.Id_medico == nuevaCita.Id_medico &&
                   c.Fecha == nuevaCita.Fecha &&
                   c.Hora == nuevaCita.Hora &&
                   c.Estado == nuevaCita.Estado&&
                   c.Id_cita != nuevaCita.Id_cita // ← evita que se compare consigo misma
                   );

                   if (existeCitaDuplicada)
                   {
                       MessageBox.Show("⚠️ Ya existe una cita para este médico en esa fecha y hora.",
                                        "Cita duplicada", MessageBoxButton.OK, MessageBoxImage.Warning);
                       return;
                   }

                    db.Cita.Add(nuevaCita);
                    db.SaveChanges();

                    MessageBox.Show("✅ Cita agendada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    //enviar correo al paciente
                    try
                    {
                        var paciente = db.Paciente.FirstOrDefault(x => x.Id_paciente == nuevaCita.Id_paciente);
                        var medico = db.Medico.FirstOrDefault(x => x.Id_medico == nuevaCita.Id_medico);
                        var especialidad = db.Especialidad.FirstOrDefault(x => x.Id_especialidad == medico.Id_especialidad);

                        if (paciente != null && medico != null && !string.IsNullOrEmpty(paciente.Correo))
                        {
                            var emailService = new EmailService("medicosmedigestinforma@gmail.com");

                            string projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));

                            string html = emailService.CargarPlantilla(System.IO.Path.Combine(projectPath, "Resources", "citaprogramada.html"));

                            html = html.Replace("{{PacienteNombre}}", paciente.Nombre + " " + paciente.Apellidos)
                                       .Replace("{{Fecha}}", nuevaCita.Fecha.ToString("dd/MM/yyyy"))
                                       .Replace("{{Hora}}", nuevaCita.Hora.ToString(@"hh\:mm"))
                                       .Replace("{{MedicoNombre}}", medico.Nombre + " " + medico.Apellidos)
                                       .Replace("{{Especialidad}}", especialidad.Nombre)
                                       .Replace("{{Observaciones}}", nuevaCita.Observaciones)
                                       .Replace("{{Estado}}", nuevaCita.Estado);

                            string rutaLogo = System.IO.Path.Combine(projectPath, "Resources", "logo.jpg");
                            emailService.EnviarCorreo(
                                paciente.Correo,
                                "Cita Médica Programada",
                                html,
                                rutaLogo
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("⚠️ La cita se guardó pero hubo un error al enviar el correo:\n" + ex.Message);
                    }

                    MessageBox.Show("Se envio el Correo al Paciente");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la cita:\n{ex.InnerException?.Message ?? ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

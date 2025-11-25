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
                    MessageBox.Show("✅ Cita actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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

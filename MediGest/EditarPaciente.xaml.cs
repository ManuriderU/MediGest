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

namespace MediGest
{
    /// <summary>
    /// Lógica de interacción para EditarPaciente.xaml
    /// </summary>
    public partial class EditarPaciente : Window
    {
        Paciente pacienteActual;
        public EditarPaciente(Paciente p)
        {
            InitializeComponent();
            this.pacienteActual = p;
            CargarInformacion();
        }

        private void CargarInformacion()
        {
                txtNombre.Text = pacienteActual.Nombre;
                txtApellidos.Text = pacienteActual.Apellidos;
                txtDNI.Text = pacienteActual.Dni;
                txtCIPA.Text = pacienteActual.Cipa;
                txtHistoria.Text = pacienteActual.Num_historia_clinica;
                txtSeguridadSocial.Text = pacienteActual.Num_seguridad_social;
                dpFechaNacimiento.SelectedDate = pacienteActual.Fecha_nacimiento;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtApellidos.Text == "" && txtCIPA.Text == "" && txtDNI.Text == "" && txtHistoria.Text == "" && txtNombre.Text == "" && txtSeguridadSocial.Text == "")
            {
                MessageBox.Show("Rellene todos los Campos", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpFechaNacimiento.SelectedDate > DateTime.Now) {
                MessageBox.Show("No puede elegir una fecha de nacimiento posterior a la de hoy.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new MediGestContext())
                {
                    var pacienteDB = db.Paciente.FirstOrDefault(c => c.Id_paciente == pacienteActual.Id_paciente);

                    if (pacienteDB == null)
                    {
                        MessageBox.Show("No se encontró el paciente en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Actualizamos campos modificables
                    pacienteDB.Nombre = txtNombre.Text;
                    pacienteDB.Apellidos = txtApellidos.Text;
                    pacienteDB.Dni = txtDNI.Text;
                    pacienteDB.Cipa = txtCIPA.Text;
                    pacienteDB.Num_historia_clinica = txtHistoria.Text;
                    pacienteDB.Num_seguridad_social = txtSeguridadSocial.Text;
                    pacienteDB.Fecha_nacimiento = dpFechaNacimiento.SelectedDate.Value;
                    db.SaveChanges();
                    MessageBox.Show("✅ Paciente actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los cambios:\n{ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
    }
}

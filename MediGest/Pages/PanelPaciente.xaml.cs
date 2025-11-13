using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MediGest.Pages
{
    /// <summary>
    /// Lógica de interacción para PanelPaciente.xaml
    /// </summary>
    public partial class PanelPaciente : Window
    {
        public PanelPaciente()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "¿Estas seguro de Cancelar?",
                "Confirmacion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) {
                this.Close();
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try {
                using (var db = new MediGestContext()) {
                    Paciente nuevoPaciente = new Paciente {
                        Nombre = txtNombre.Text.Trim(),
                        Apellidos = txtApellidos.Text.Trim(),
                        Dni = txtDNI.Text.Trim().ToUpper(),
                        Cipa = txtCIPA.Text.Trim(),
                        Num_historia_clinica = txtHistoria.Text.Trim(),
                        Num_seguridad_social = txtSeguridadSocial.Text.Trim(),
                        Fecha_nacimiento = dpFechaNacimiento.SelectedDate.Value
                    };
                    db.Paciente.Add(nuevoPaciente);
                    db.SaveChanges();
                }
                MessageBox.Show("Paciente registrado correctamente.",
                             "Éxito",
                             MessageBoxButton.OK,
                             MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar paciente: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                MessageBox.Show($"Error al guardar paciente:\n{ex.InnerException?.Message ?? ex.Message}");
            }
        }

    }
}

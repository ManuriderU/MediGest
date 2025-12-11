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
    /// Lógica de interacción para EditarRecepcionista.xaml
    /// </summary>
    public partial class EditarRecepcionista : Window
    {
        private Recepcionista recepcionistaActual;

        public EditarRecepcionista(Recepcionista pRecepcionista)
        {
            InitializeComponent();
            recepcionistaActual = pRecepcionista;
            CargarInformacion();

        }

        public void CargarInformacion() {

            txtNombre.Text = recepcionistaActual.Nombre;
            txtApellidos.Text = recepcionistaActual.Apellidos;
            txtTelefono.Text = recepcionistaActual.Telefono;
            txtEmail.Text = recepcionistaActual.Email;
        }


        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

            if (txtNombre.Text == "" || txtApellidos.Text == "" || txtTelefono.Text == "" || txtEmail.Text == "") {

                MessageBox.Show("Los campos deben estar rellenos");
                return;
            }
            try
            {
                using (var db = new MediGestContext())
                {

                    var recepcionistaDB = db.Recepcionista.FirstOrDefault(r => r.Id_recepcionista == recepcionistaActual.Id_recepcionista);

                    if (recepcionistaDB == null)
                    {
                        MessageBox.Show("No se encontró el usuario en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    recepcionistaDB.Nombre = txtNombre.Text;
                    recepcionistaDB.Apellidos = txtApellidos.Text;
                    recepcionistaDB.Telefono = txtTelefono.Text;
                    recepcionistaDB.Email = txtEmail.Text;
                    db.SaveChanges();
                    MessageBox.Show("Recepcionista Actualizado");
                    this.Close();
                }

            }
            catch (Exception ex) {

                MessageBox.Show($"Error al guardar los cambios:\n{ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btn_Cancelar_Click(object sender, RoutedEventArgs e)
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

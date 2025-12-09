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
    /// Lógica de interacción para EditarLogin.xaml
    /// </summary>
    public partial class EditarLogin : Window
    {
        private Usuario usuarioActual;
        public EditarLogin(Usuario pUsario)
        {
            InitializeComponent();
            usuarioActual = pUsario;
            CargarLogin();
        }

        public void CargarLogin() {
            txtLogin.Text = usuarioActual.Login;
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
            try
            {
                using (var db = new MediGestContext())
                {
                    var usuarioDB = db.Usuario.FirstOrDefault(c => c.Id_usuario == usuarioActual.Id_usuario);

                    if (usuarioDB == null)
                    {
                        MessageBox.Show("No se encontró el usuario en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Actualizamos campos modificables
                    usuarioDB.Login = txtLogin.Text;
                    db.SaveChanges();
                    MessageBox.Show("✅ Login del Usuario actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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

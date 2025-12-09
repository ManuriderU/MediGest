using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Lógica de interacción para CambiarPassword.xaml
    /// </summary>
    public partial class CambiarPassword : Window
    {
        private Usuario usuarioActual;
        public CambiarPassword(Usuario pUsuario)
        {
            InitializeComponent();
            usuarioActual = pUsuario;
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
            if (txtPassword.Text == "") {
                MessageBox.Show("La contraseña no puede estar Vacia");
                return;
            }
            var passwordHash = CalcularSHA256(txtPassword.Text);
            using (var db = new MediGestContext())
            {
                var user = db.Usuario.FirstOrDefault(u =>
                      u.Id_usuario == usuarioActual.Id_usuario);

                if (user == null) {
                    MessageBox.Show("No se encontró el usuario en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.Password == passwordHash)
                {
                    MessageBox.Show("Esa ya es la Contraseña Actual introduzca una diferente");
                    return;
                }

                user.Password = passwordHash;
                db.SaveChanges();
                MessageBox.Show("Contraseña Actualizada");
                this.Close();
            }
        }


        private string CalcularSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2")); // convierte a hexadecimal
                return sb.ToString();
            }
        }

    }
}

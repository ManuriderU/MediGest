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

namespace MediGest
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            PlaceholderUsuario.Visibility = string.IsNullOrWhiteSpace(TxtUsuario.Text) ? Visibility.Visible : Visibility.Hidden;
            PlaceholderPassword.Visibility = string.IsNullOrWhiteSpace(TxtPassword.Password) ? Visibility.Visible : Visibility.Hidden;

        }

        //Eventos para Manipular Placeholders
        private void TxtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderUsuario.Visibility =
                string.IsNullOrWhiteSpace(TxtUsuario.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceholderPassword.Visibility =
                string.IsNullOrWhiteSpace(TxtPassword.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Ejemplo simple: valida usuario y contraseña (A falta de Base de Datos)
            if (TxtUsuario.Text == "admin" && TxtPassword.Password == "root")
            {
                MessageBox.Show("Accesos de Administrador conseguidos,iniciando sesion...");
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            } else if (TxtUsuario.Text == "Pepe" && TxtPassword.Password == "71991839E") 
            {
                MessageBox.Show("¡Bienvenido " + TxtUsuario.Text + "! iniciando sesion...");
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

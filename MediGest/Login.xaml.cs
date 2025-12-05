using MediGest.Data;
using MediGest.Clases;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Configuration;

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

        // --- Eventos para placeholders ---
        private void TxtUsuario_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PlaceholderUsuario.Visibility = string.IsNullOrWhiteSpace(TxtUsuario.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceholderPassword.Visibility = string.IsNullOrWhiteSpace(TxtPassword.Password)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        // --- Botón de inicio de sesión ---
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = TxtUsuario.Text.Trim();
            string password = TxtPassword.Password.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, introduce usuario y contraseña.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Calculamos hash SHA256 del password ingresado
            string passwordHash = CalcularSHA256(password);

            try
            {
                using (var db = new MediGestContext())
                {
                    // Buscamos usuario por login y contraseña hasheada
                    var user = db.Usuario.FirstOrDefault(u =>
                        u.Login == usuario && u.Password == passwordHash);

                    if (user != null)
                    {
                        if (user.Rol == "medico") {
                            var userName = db.Medico.FirstOrDefault(m => m.Id_usuario == user.Id_usuario);
                            if (userName != null)
                            {
                                MessageBox.Show($"¡Bienvenido {userName.Nombre}  {userName.Apellidos}!", "Inicio de sesión exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                                SessionManager.IdUsuario = userName.Id_medico;
                                SessionManager.Nombre = userName.Nombre;
                                SessionManager.Apellidos = userName.Apellidos;
                                SessionManager.Rol = "Medico";
                                MainWindow main = new MainWindow();
                                main.Show();
                                this.Close();
                            }
                        }

                        if (user.Rol == "recepcionista") {
                            var userName = db.Recepcionista.FirstOrDefault(r => r.Id_usuario == user.Id_usuario);
                            if (userName != null)
                            {
                                MessageBox.Show($"¡Bienvenido {userName.Nombre}  {userName.Apellidos}!", "Inicio de sesión exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                                SessionManager.IdUsuario = userName.Id_recepcionista;
                                SessionManager.Nombre = userName.Nombre;
                                SessionManager.Apellidos = userName.Apellidos;
                                SessionManager.Rol = "Recepcionista";
                                MainWindow main = new MainWindow();
                                main.Show();
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Función auxiliar para generar hash SHA256 ---
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

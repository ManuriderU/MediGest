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
    /// Lógica de interacción para CrearUsuario.xaml
    /// </summary>
    public partial class CrearUsuario : Window
    {
        public CrearUsuario()
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

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtLogin.Text == "" && txtPassword.Text == "" && txtPasswordConfirmacion.ToString() == "" && cmbRol.SelectedItem == null) {
                MessageBox.Show("Rellena los Campos");
                return;
            }

            if (txtPasswordConfirmacion.ToString() == txtPassword.Text) {
                MessageBox.Show("La contraseña no coincide en el Campo Contraseña o Confirmacion");
                return;
            }

            Usuario nuevoUsuario = new Usuario
            {
                Login = txtLogin.Text,
                Password = txtPassword.Text,
                Rol = cmbRol.Text.ToLower()
            };

            using (var db = new MediGestContext())
            {
                var usuario = db.Usuario;
                usuario.Add(nuevoUsuario);
                db.SaveChanges();

                int idUsuario = db.Usuario
                .OrderByDescending(u => u.Id_usuario)
                .First()
                .Id_usuario;

                if (nuevoUsuario.Rol == "medico")
                {
                    MessageBox.Show("Procediendo a la Creacion del Medico...");
                    var crearMedico = new CrearMedico(idUsuario);
                    crearMedico.ShowDialog();
                    this.Close();
                    
                }
                else if (nuevoUsuario.Rol == "recepcionista")
                {
                    MessageBox.Show("Procediendo a la Creacion del Recepcionista...");
                    var crearRecepcionista = new CrearRecepcionista(idUsuario);
                    crearRecepcionista.ShowDialog();
                    this.Close();
                }
                else
                {
                    this.Close();
                }
            }

        }
    }
}

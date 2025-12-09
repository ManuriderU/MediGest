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
    /// Lógica de interacción para CrearRecepcionista.xaml
    /// </summary>
    public partial class CrearRecepcionista : Window
    {
        private int idNuevoUsuario = 0;
        public CrearRecepcionista(int idUsuario)
        {
            InitializeComponent();
            idNuevoUsuario = idUsuario;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtApellidos.Text == "" && txtApellidos.Text == "" && txtTelefono.Text == "" && txtEmail.Text == "") {
                MessageBox.Show("Rellena los campos");
                return;
            }

            try
            {
                using (var db = new MediGestContext()) 
                {

                    Recepcionista nuevoRecepcionista = new Recepcionista 
                    {
                        Id_usuario = idNuevoUsuario,
                        Nombre = txtNombre.Text,
                        Apellidos = txtApellidos.Text,
                        Telefono = txtTelefono.Text,
                        Email = txtEmail.Text
                    };

                    db.Recepcionista.Add(nuevoRecepcionista);
                    db.SaveChanges();
                    MessageBox.Show("Recepcionista Creado");
                    this.Close();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error al crear recepcionista:\n{ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}

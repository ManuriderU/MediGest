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
    /// Lógica de interacción para CrearMedico.xaml
    /// </summary>
    public partial class CrearMedico : Window
    {
        private int nuevoIdUsuario = 0;
        public CrearMedico(int idUsuario)
        {
            InitializeComponent();
            nuevoIdUsuario = idUsuario;
            CargarEspecialidades();
        }

        private void CargarEspecialidades() {
            using (var db = new MediGestContext()) {
                var especialidades = db.Especialidad.Select(e => new { e.Id_especialidad,e.Nombre}).ToList();
                cmbEspecialidad.ItemsSource = especialidades;
                cmbEspecialidad.SelectedValuePath = "Id_especialidad";
                cmbEspecialidad.DisplayMemberPath = "Nombre";
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtApellidos.Text == "" && txtApellidos.Text == "" && txtNumeroC.Text == "" && txtEmail.Text == "" && cmbEspecialidad.SelectedItem == null)
            {
                MessageBox.Show("Rellena los campos");
                return;
            }

            try
            {
                using (var db = new MediGestContext())
                {

                    Medico nuevoMedico = new Medico
                    {
                        Id_usuario = nuevoIdUsuario,
                        Id_especialidad = (int)cmbEspecialidad.SelectedValue,
                        Nombre = txtNombre.Text,
                        Apellidos = txtApellidos.Text,
                        Num_colegiado = txtNumeroC.Text,
                        Correo_corporativo = txtEmail.Text
                    };

                    db.Medico.Add(nuevoMedico);
                    db.SaveChanges();
                    MessageBox.Show("Medico Creado");
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

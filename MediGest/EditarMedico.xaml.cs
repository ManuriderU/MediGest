using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    /// Lógica de interacción para EditarMedico.xaml
    /// </summary>
    public partial class EditarMedico : Window
    {
        private Medico medicoActual;
        public EditarMedico(Medico pMedico)
        {
            InitializeComponent();
            medicoActual = pMedico;
            CargarEspecialidades();
            CargarInformacion();
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

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtNombre.Text == "" || txtApellidos.Text == "" || txtNumeroC.Text == "" || txtEmail.Text == "" || (int)cmbEspecialidad.SelectedValue == 0) {

                MessageBox.Show("Rellene los Campos y elija una especialidad adecuada");
                return;
            }

            try
            {
                using (var db = new MediGestContext())
                {
                    var medicoDB = db.Medico.FirstOrDefault(c => c.Id_medico == medicoActual.Id_medico);

                    if (medicoDB == null)
                    {
                        MessageBox.Show("No se encontró el usuario en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Actualizamos campos modificables
                    medicoDB.Nombre = txtNombre.Text;
                    medicoDB.Apellidos = txtApellidos.Text;
                    medicoDB.Correo_corporativo = txtEmail.Text;
                    medicoDB.Num_colegiado = txtNumeroC.Text;
                    medicoDB.Id_especialidad = (int)cmbEspecialidad.SelectedValue;
                    db.SaveChanges();
                    MessageBox.Show("✅ Medico actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los cambios:\n{ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void CargarEspecialidades()
        {
            using (var db = new MediGestContext())
            {
                var especialidades = db.Especialidad
                   .Select(p => new { p.Id_especialidad, p.Nombre })
                   .ToList();

                var lista = new List<object>();

                // Item por defecto (deshabilitado)
                lista.Add(new { Id_especialidad = 0, Nombre = "Especialidad", IsEnabled = false });

                // Agregar el resto
                lista.AddRange(especialidades);

                // Asignar al ComboBox
                cmbEspecialidad.ItemsSource = lista;
                cmbEspecialidad.SelectedValuePath = "Id_especialidad";
                cmbEspecialidad.DisplayMemberPath = "Nombre";
                cmbEspecialidad.SelectedValue = 0;
            }
        }


        private void CargarInformacion() {

            txtNombre.Text = medicoActual.Nombre;
            txtApellidos.Text = medicoActual.Apellidos;
            txtNumeroC.Text = medicoActual.Num_colegiado;
            txtEmail.Text = medicoActual.Correo_corporativo;
            cmbEspecialidad.SelectedValue = medicoActual.Id_especialidad;
        }

    }
}

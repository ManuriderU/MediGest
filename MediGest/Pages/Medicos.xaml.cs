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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediGest.Clases;
using MediGest.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MediGest.Pages
{
    /// <summary>
    /// Lógica de interacción para Medicos.xaml
    /// </summary>
    public partial class Medicos : Page
    {
        public Medicos()
        {
            InitializeComponent();
            CargarMedicos();
            CargarEspecialidades();
        }


        private void CargarEspecialidades() {
            using (var db = new MediGestContext()) {
                var especialidades = db.Especialidad
                   .Select(p => new { p.Id_especialidad,p.Nombre})
                   .ToList();

                var lista = new List<object>();

                // Item por defecto (deshabilitado)
                lista.Add(new { Id_especialidad = 0, Nombre = "Especialidad", IsEnabled = false });

                // Agregar el resto
                lista.AddRange(especialidades);

                // Asignar al ComboBox
                CmbEspecialidad.ItemsSource = lista;
                CmbEspecialidad.SelectedValuePath = "Id_especialidad";
                CmbEspecialidad.DisplayMemberPath = "Nombre";
                CmbEspecialidad.SelectedValue = 0;
            }
        }

        private void DataGridMedicos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var medicoAnonimo = DataGridMedicos.SelectedItem;
            if (medicoAnonimo == null)
                return;

            var prop = medicoAnonimo.GetType().GetProperty("Id_medico");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador del Usuario seleccionada.");
                return;
            }

            int idMedico = (int)prop.GetValue(medicoAnonimo);

            using (var db = new MediGestContext())
            {
                var medicoSeleccionado = db.Medico.FirstOrDefault(c => c.Id_medico == idMedico);

                if (medicoSeleccionado != null)
                {
                    var ventanaEditar = new EditarMedico(medicoSeleccionado);
                    ventanaEditar.ShowDialog();
                    CargarMedicos(); // refrescar la tabla
                }
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            CargarMedicos();
            CmbEspecialidad.SelectedValue = 0;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new MediGestContext()) {
                string nombre = txtBuscarMedicos.Text.Trim().ToLower();
                var query = db.Medico.AsQueryable();

                if (!CmbEspecialidad.SelectedValue.Equals(0))
                {
                    query = query.Where(m => m.Id_especialidad == (int)CmbEspecialidad.SelectedValue);
                }
                else {
                    MessageBox.Show("Esa no es una Especialidad Valida");
                    return;
                }

                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(m => (m.Nombre + " " + m.Apellidos).ToLower().Contains(nombre));
                }

                var resultado = query
                .Select(m => new 
                {
                    m.Id_medico,
                    Nombre = m.Nombre + " " + m.Apellidos,
                    m.Num_colegiado,
                    m.Correo_corporativo
                })
                .ToList();

                DataGridMedicos.ItemsSource = resultado;
            }
        }

        private void CargarMedicos()
        {
            using (var db = new MediGestContext()) {

                var lista = (from p in db.Medico
                             select new
                             {
                                 p.Id_medico,
                                 Nombre = p.Nombre + " " + p.Apellidos,
                                 p.Num_colegiado,
                                 p.Correo_corporativo
                             })
                             .ToList();
                DataGridMedicos.ItemsSource = lista;
            }
        }

    }
}

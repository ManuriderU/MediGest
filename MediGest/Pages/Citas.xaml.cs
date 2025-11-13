using MediGest.Data;
using MediGest.Clases;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MediGest.Pages
{
    public partial class Citas : Page
    {
        public Citas()
        {
            InitializeComponent();
            CargarCitas();
        }

        
        private void CargarCitas()
        {
            using (var db = new MediGestContext())
            {
                var citas = (from c in db.Cita
                             join p in db.Paciente on c.Id_paciente equals p.Id_paciente
                             join m in db.Medico on c.Id_medico equals m.Id_medico
                             select new
                             {
                                 c.Id_cita,
                                 c.Fecha,
                                 c.Hora,
                                 PacienteNombre = p.Nombre + " " + p.Apellidos,
                                 MedicoNombre = m.Nombre + " " + m.Apellidos,
                                 c.Estado,
                                 c.Observaciones
                             }).ToList();

                DataGridCitas.ItemsSource = citas;
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = TxtBuscarPaciente.Text.ToLower().Trim();
            DateTime? fecha = DateBuscar.SelectedDate;
            String estado = cmbEstado.Text;

            using (var db = new MediGestContext())
            {
                var query = from c in db.Cita
                            join p in db.Paciente on c.Id_paciente equals p.Id_paciente
                            join m in db.Medico on c.Id_medico equals m.Id_medico
                            select new
                            {
                                c.Id_cita,
                                c.Fecha,
                                c.Hora,
                                PacienteNombre = p.Nombre + " " + p.Apellidos,
                                MedicoNombre = m.Nombre + " " + m.Apellidos,
                                c.Estado,
                                c.Observaciones
                            };
                
                if (!string.IsNullOrEmpty(nombre))
                    query = query.Where(c => c.PacienteNombre.ToLower().Contains(nombre));

                if (fecha != null)
                    query = query.Where(c => c.Fecha.Date == fecha.Value.Date);

                if (estado != null)
                    query = query.Where(c => c.Estado.ToLower() == estado);

                DataGridCitas.ItemsSource = query.ToList();
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarPaciente.Clear();
            DateBuscar.SelectedDate = null;
            CargarCitas();
        }

        private void DataGridCitas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var citaAnonima = DataGridCitas.SelectedItem;
            if (citaAnonima == null)
                return;

            var prop = citaAnonima.GetType().GetProperty("Id_cita");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador de la cita seleccionada.");
                return;
            }

            int idCita = (int)prop.GetValue(citaAnonima);

            using (var db = new MediGestContext())
            {
                var citaSeleccionada = db.Cita.FirstOrDefault(c => c.Id_cita == idCita);
                if (citaSeleccionada.Estado == "realizada")
                {
                    MessageBox.Show("Esta cita ya fue realizada no se puede modificar", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (citaSeleccionada != null)
                {
                    var ventanaEditar = new EditarCita(citaSeleccionada);
                    ventanaEditar.ShowDialog();
                    CargarCitas(); // refrescar la tabla
                }
            }

        }

        private void DataGridCitas_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
            "¿Quieres Eliminar esta cita?",
            "Confirmacion",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var citaAnonima = DataGridCitas.SelectedItem;
                if (citaAnonima == null)
                    return;

                var prop = citaAnonima.GetType().GetProperty("Id_cita");
                if (prop == null)
                {
                    MessageBox.Show("No se encontró el identificador de la cita seleccionada.");
                    return;
                }

                int idCita = (int)prop.GetValue(citaAnonima);


                using (var db = new MediGestContext())
                {
                    var citaSeleccionada = db.Cita.FirstOrDefault(c => c.Id_cita == idCita);
                    if (citaSeleccionada != null)
                    {
                        db.Remove(citaSeleccionada);
                        db.SaveChanges();
                        CargarCitas(); // refrescar la tabla
                    }
                }
            }
        }
    }
}
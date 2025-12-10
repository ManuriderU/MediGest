using MediGest.Data;
using MediGest.Clases;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static iText.Commons.Utils.PlaceHolderTextUtil;
using System.Windows.Media;

namespace MediGest.Pages
{
    public partial class Citas : Page
    {
        String placeholderText = "Introduce nombre del Paciente relacionado a Buscar";
        public Citas()
        {
            InitializeComponent();
            CargarCitas();
            SetPlaceholder();
        }

        private void SetPlaceholder()
        {
            if (string.IsNullOrEmpty(TxtBuscarPaciente.Text))
            {
                TxtBuscarPaciente.Text = placeholderText;
                TxtBuscarPaciente.Foreground = Brushes.Gray;
            }
        }

        private void TxtBuscarPaciente_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtBuscarPaciente.Text == placeholderText)
            {
                TxtBuscarPaciente.Text = "";
                TxtBuscarPaciente.Foreground = Brushes.Black;
            }
        }


        private void CargarCitas()
        {
            using (var db = new MediGestContext())
            {
                if (SessionManager.Rol == "Medico")
                {
                    var citas = (from c in db.Cita
                                 join p in db.Paciente on c.Id_paciente equals p.Id_paciente
                                 join m in db.Medico on c.Id_medico equals m.Id_medico
                                 where c.Id_medico == SessionManager.IdUsuario
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
                else {
                    var citas = (from c in db.Cita
                                 join p in db.Paciente on c.Id_paciente equals p.Id_paciente
                                 join m in db.Medico on c.Id_medico equals m.Id_medico
                                 where c.Id_recepcionista == SessionManager.IdUsuario
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
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = TxtBuscarPaciente.Text.ToLower().Trim();
            DateTime? fecha = DateBuscar.SelectedDate;
            string estado = cmbEstado.Text;

            using (var db = new MediGestContext())
            {
                if (SessionManager.Rol == "Medico")
                {
                    var query = from c in db.Cita
                                join p in db.Paciente on c.Id_paciente equals p.Id_paciente
                                join m in db.Medico on c.Id_medico equals m.Id_medico
                                where c.Id_medico == SessionManager.IdUsuario
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

                    if (!string.IsNullOrEmpty(nombre) && nombre != placeholderText.ToLower())
                        query = query.Where(c => c.PacienteNombre.ToLower().Contains(nombre));

                    if (fecha != null)
                        query = query.Where(c => c.Fecha.Date == fecha.Value.Date);

                    if (cmbEstado.SelectedIndex > 0)
                        query = query.Where(c => c.Estado.ToLower() == estado);

                    DataGridCitas.ItemsSource = query.ToList();
                }
                else {
                    var query = from c in db.Cita
                                join p in db.Paciente on c.Id_paciente equals p.Id_paciente
                                join m in db.Medico on c.Id_medico equals m.Id_medico
                                where c.Id_recepcionista == SessionManager.IdUsuario
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

                    if (!string.IsNullOrEmpty(nombre) && nombre != placeholderText.ToLower())
                        query = query.Where(c => c.PacienteNombre.ToLower().Contains(nombre));

                    if (fecha != null)
                        query = query.Where(c => c.Fecha.Date == fecha.Value.Date);

                    if (cmbEstado.SelectedIndex > 0)
                        query = query.Where(c => c.Estado.ToLower() == estado);

                    DataGridCitas.ItemsSource = query.ToList();
                }
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            SetPlaceholder();
            DateBuscar.SelectedDate = null;
            cmbEstado.SelectedIndex = 0;
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
            if (SessionManager.Rol == "Medico") {
                MessageBox.Show("No tienes permisos para borrar Citas");
                return;
            }

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
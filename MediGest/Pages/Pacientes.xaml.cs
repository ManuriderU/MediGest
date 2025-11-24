using MediGest.Clases;
using MediGest.Data;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MediGest.Pages
{
    public partial class Pacientes : Page
    {
        public Pacientes()
        {
            InitializeComponent();
            CargarAñosYMeses();
            CargarPacientes();
        }

        // 🗓️ Cargar años y meses
        private void CargarAñosYMeses()
        {
            for (int año = 1900; año <= DateTime.Now.Year; año++)
                CmbAño.Items.Add(año);

            var meses = System.Globalization.DateTimeFormatInfo.InvariantInfo.MonthNames
                .Where(m => !string.IsNullOrEmpty(m)).ToList();
            foreach (var mes in meses)
                CmbMes.Items.Add(mes);
        }

        // 👥 Cargar pacientes
        private void CargarPacientes()
        {
            using (var db = new MediGestContext())
            {
                if (SessionManager.Rol == "Medico")
                {
                       var lista = (from p in db.Paciente
                                    join i in db.Informe_Medico on p.Id_paciente equals i.Id_paciente
                                    where i.Id_medico == SessionManager.IdUsuario
                                    select new
                                    {
                                        p.Id_paciente,
                                        Nombre = p.Nombre + " " + p.Apellidos,
                                        p.Dni,
                                        p.Cipa,
                                        p.Num_historia_clinica,
                                        p.Num_seguridad_social,
                                        p.Fecha_nacimiento
                                    })
                                    .ToList();

                    DataGridPacientes.ItemsSource = lista;
                }
                else {
                    var lista = db.Paciente
                        .Select(p => new
                        {
                            p.Id_paciente,
                            Nombre = p.Nombre + " " + p.Apellidos,
                            p.Dni,
                            p.Cipa,
                            p.Num_historia_clinica,
                            p.Num_seguridad_social,
                            p.Fecha_nacimiento
                        })
                        .ToList();

                    DataGridPacientes.ItemsSource = lista;
                }
            }
        }

        // 🔍 Buscar pacientes
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = TxtBuscarPaciente.Text.Trim().ToLower();
            DateTime? fechaSeleccionada = DateBuscar.SelectedDate;
            int? año = CmbAño.SelectedItem as int?;
            int mes = CmbMes.SelectedIndex + 1;

            using (var db = new MediGestContext())
            {
                var query = db.Paciente.AsQueryable();

                if (!string.IsNullOrEmpty(nombre))
                    query = query.Where(p => (p.Nombre + " " + p.Apellidos).ToLower().Contains(nombre));

                if (fechaSeleccionada.HasValue)
                {
                    DateTime fecha = fechaSeleccionada.Value.Date;
                    query = query.Where(p => p.Fecha_nacimiento.Date == fecha);
                }
                else
                {
                    if (año.HasValue)
                        query = query.Where(p => p.Fecha_nacimiento.Year == año.Value);

                    if (CmbMes.SelectedIndex != -1)
                        query = query.Where(p => p.Fecha_nacimiento.Month == mes);
                }

                if (SessionManager.Rol == "Medico") {
                       var resultado = (from p in query
                                    join i in db.Informe_Medico on p.Id_paciente equals i.Id_paciente
                                    where i.Id_medico == SessionManager.IdUsuario
                                    select new
                                    {
                                        p.Id_paciente,
                                        Nombre = p.Nombre + " " + p.Apellidos,
                                        p.Dni,
                                        p.Cipa,
                                        p.Num_historia_clinica,
                                        p.Num_seguridad_social,
                                        p.Fecha_nacimiento
                                    })
                                    .ToList();

                    DataGridPacientes.ItemsSource = resultado;
                }
                else
                {
                    var resultado = query
                       .Select(p => new
                       {
                           p.Id_paciente,
                           Nombre = p.Nombre + " " + p.Apellidos,
                           p.Dni,
                           p.Cipa,
                           p.Num_historia_clinica,
                           p.Num_seguridad_social,
                           p.Fecha_nacimiento
                       })
                       .ToList();

                    DataGridPacientes.ItemsSource = resultado;
                }
            }
        }

        // 🔄 Limpiar filtros
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarPaciente.Clear();
            DateBuscar.SelectedDate = null;
            CmbAño.SelectedIndex = -1;
            CmbMes.SelectedIndex = -1;

            DateBuscar.IsEnabled = true;
            CmbAño.IsEnabled = true;
            CmbMes.IsEnabled = true;

            CargarPacientes();
        }

        // 🚫 No permitir usar ambos filtros a la vez
        private void DateBuscar_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateBuscar.SelectedDate != null)
            {
                CmbAño.IsEnabled = false;
                CmbMes.IsEnabled = false;
            }
            else
            {
                CmbAño.IsEnabled = true;
                CmbMes.IsEnabled = true;
            }
        }

        private void CmbAño_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbAño.SelectedIndex != -1 || CmbMes.SelectedIndex != -1)
                DateBuscar.IsEnabled = false;
            else
                DateBuscar.IsEnabled = true;
        }

        private void CmbMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbAño.SelectedIndex != -1 || CmbMes.SelectedIndex != -1)
                DateBuscar.IsEnabled = false;
            else
                DateBuscar.IsEnabled = true;
        }

        private void DataGridPacientes_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SessionManager.Rol == "Medico") {
                MessageBox.Show("No tienes permisos para editar Clientes");
                return;
            }

            var pacienteAnonimo = DataGridPacientes.SelectedItem;
            if (pacienteAnonimo == null)
                return;

            var prop = pacienteAnonimo.GetType().GetProperty("Id_paciente");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador del paciente seleccionado.");
                return;
            }

            int idPaciente = (int)prop.GetValue(pacienteAnonimo);

            using (var db = new MediGestContext())
            {
                var pacienteSeleccionado = db.Paciente.FirstOrDefault(c => c.Id_paciente == idPaciente);

                if (pacienteSeleccionado != null)
                {
                    var ventanaEditar = new EditarPaciente(pacienteSeleccionado);
                    ventanaEditar.ShowDialog();
                    CargarPacientes(); // refrescar la tabla
                }
            }
        }

        private void DataGridPacientes_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pacienteAnonimo = DataGridPacientes.SelectedItem;
            if (pacienteAnonimo == null)
                return;

            ContextMenu menu = new ContextMenu();
            
            MenuItem informesItem = new MenuItem { Header = "Ver Informes Médicos" };
            informesItem.Click += (s, args) => VerInformes(pacienteAnonimo);
            menu.Items.Add(informesItem);

            // Nueva opción: Generar PDF de Informes Médicos
            MenuItem generarPdfItem = new MenuItem { Header = "Generar PDF de Informes Médicos" };
            generarPdfItem.Click += (s, args) => GenerarPDFInformesMedicos(pacienteAnonimo);
            menu.Items.Add(generarPdfItem);

            // Mostramos el menú contextual manualmente
            menu.IsOpen = true;
        }

        private void GenerarPDFInformesMedicos(object pacienteAnonimo)
        {
            var prop = pacienteAnonimo.GetType().GetProperty("Id_paciente");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador del paciente seleccionado.");
                return;
            }

            int idPaciente = (int)prop.GetValue(pacienteAnonimo);

            try
            {
                using (var db = new MediGestContext())
                {
                    var paciente = db.Paciente.FirstOrDefault(p => p.Id_paciente == idPaciente);
                    if (paciente == null)
                    {
                        MessageBox.Show("No se encontró el paciente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Crear carpeta para informes médicos si no existe
                    string carpetaInformes = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InformesMedicos");
                    if (!Directory.Exists(carpetaInformes))
                        Directory.CreateDirectory(carpetaInformes);

                    // Generar nombre de archivo
                    string nombreArchivo = $"InformeMedico_{paciente.Nombre}_{paciente.Apellidos}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    // Limpiar caracteres no válidos del nombre de archivo
                    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    {
                        nombreArchivo = nombreArchivo.Replace(c, '_');
                    }

                    string rutaCompleta = System.IO.Path.Combine(carpetaInformes, nombreArchivo);

                    // Generar el PDF usando el nuevo generador
                    InformeMedicoGenerator generator = new InformeMedicoGenerator();
                    generator.GenerarInformeMedicoPDF(idPaciente, rutaCompleta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al generar el informe médico:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void VerInformes(object pacienteAnonimo)
        {
            if (SessionManager.Rol == "Recepcionista") {
                MessageBox.Show("Solo los Medicos pueden revisar los informes medicos asociados a ellos mismos de los Pacientes");
                return;
            }
            var prop = pacienteAnonimo.GetType().GetProperty("Id_paciente");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador del paciente seleccionado.");
                return;
            }

            int idPaciente = (int)prop.GetValue(pacienteAnonimo);

            using (var db = new MediGestContext())
            {
                var pacienteSeleccionado = db.Paciente.FirstOrDefault(c => c.Id_paciente == idPaciente);

                if (pacienteSeleccionado != null)
                {
                    var ventanaInformes = new InformesPaciente(pacienteSeleccionado);
                    ventanaInformes.ShowDialog();
                }
            }
        }
    }
}

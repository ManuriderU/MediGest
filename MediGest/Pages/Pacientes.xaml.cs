using MediGest.Clases;
using MediGest.Data;
using MediGest.Servicios;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
            MenuItem informesItem = new MenuItem { Header = "Ver Informes Médicos"};
            MenuItem correoItem = new MenuItem { Header = "Enviar correo" };


            informesItem.Click += (s, args) => VerInformes(pacienteAnonimo);
            correoItem.Click += (s, args) => EnviarCorreo(pacienteAnonimo);
            menu.Items.Add(informesItem);
            menu.Items.Add(correoItem);
            // Mostramos el menú contextual manualmente
            menu.IsOpen = true;
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

        private void EnviarCorreo(object pacienteAnonimo)
        {
            var prop = pacienteAnonimo.GetType().GetProperty("Id_paciente");
            if (prop == null) return;

            int idPaciente = (int)prop.GetValue(pacienteAnonimo);

            using (var db = new MediGestContext())
            {
                var paciente = db.Paciente.FirstOrDefault(x => x.Id_paciente == idPaciente);
                var medico = db.Medico.FirstOrDefault(x => x.Id_medico == SessionManager.IdUsuario);

                if (paciente == null || medico == null ||
                    string.IsNullOrWhiteSpace(paciente.Correo) || string.IsNullOrWhiteSpace(medico.Correo_corporativo))
                {
                    MessageBox.Show("No se pudo obtener la información del paciente o médico.");
                    return;
                }

                // Abrir ventana para escribir mensaje
                var ventanaMensaje = new CorreoMensaje("Escribe aquí tu mensaje...");
                ventanaMensaje.Owner = Application.Current.MainWindow;
                bool? resultado = ventanaMensaje.ShowDialog();

                if (resultado != true || string.IsNullOrWhiteSpace(ventanaMensaje.Mensaje))
                    return;

                try
                {
                    string rutaPlantilla = "Resources\\correo.html";
                    var emailService = new EmailService(medico.Correo_corporativo);

                    string html = emailService.CargarPlantilla(rutaPlantilla);


                    html = html.Replace("{{PacienteNombre}}", paciente.Nombre + " " + paciente.Apellidos)
                               .Replace("{{MedicoNombre}}", medico.Nombre + " " + medico.Apellidos)
                               .Replace("{{Mensaje}}", ventanaMensaje.Mensaje);

                    html = emailService.InsertarLogo(html, "Resources\\logo.png");

                    emailService.EnviarCorreo(paciente.Correo, "Información de consulta médica", html);

                    MessageBox.Show("Correo enviado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al enviar correo: " + ex.Message);
                }
            }
        }
    }
}



//contraseña de aplicación: bydh ghmt ufrw lbmc
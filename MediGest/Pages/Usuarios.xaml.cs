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
    /// Lógica de interacción para Usuarios.xaml
    /// </summary>
    public partial class Usuarios : Page
    {
        public Usuarios()
        {
            InitializeComponent();
            CargarUsuarios();
            if (DataGridUsuarios.Columns.Count > 1 && DataGridUsuarios.Columns[1] is DataGridTextColumn pwdCol)
            {
                // Style para el elemento (TextBlock) que muestra el cell content
                var elementStyle = new Style(typeof(TextBlock));
                // EventSetter que ejecuta MaskPassword cuando el TextBlock se carga visualmente
                elementStyle.Setters.Add(new EventSetter(FrameworkElement.LoadedEvent, new RoutedEventHandler(MaskPassword)));

                pwdCol.ElementStyle = elementStyle;
            }
        }

        private void MaskPassword(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                if (!string.IsNullOrEmpty(tb.Text))
                    tb.Text = new string('*', 6);
            }
        }

        private void DataGridUsuarios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var usuarioAnonimo = DataGridUsuarios.SelectedItem;
            if (usuarioAnonimo == null)
                return;

            var prop = usuarioAnonimo.GetType().GetProperty("Id_usuario");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador del Usuario seleccionada.");
                return;
            }

            int idUsuario = (int)prop.GetValue(usuarioAnonimo);

            using (var db = new MediGestContext())
            {
                var usuarioSeleccionado = db.Usuario.FirstOrDefault(c => c.Id_usuario == idUsuario);

                if (usuarioSeleccionado != null)
                {
                    var ventanaEditar = new EditarLogin(usuarioSeleccionado);
                    ventanaEditar.ShowDialog();
                    CargarUsuarios(); // refrescar la tabla
                }
            }
        }

        private void CargarUsuarios() {

            using (var db = new MediGestContext())
            {
                var usuarios = (from u in db.Usuario
                                select new
                                {
                                    u.Id_usuario,
                                    u.Login,
                                    u.Password,
                                    u.Rol
                                }).ToList();
                DataGridUsuarios.ItemsSource = usuarios;
                
            }
        }

        private void DataGridUsuarios_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var usuarioAnonimo = DataGridUsuarios.SelectedItem;
            if (usuarioAnonimo == null)
                return;

            ContextMenu menu = new ContextMenu();
            MenuItem passwordItem = new MenuItem { Header = "Cambiar Contraseña" };
            passwordItem.Click += (s, args) => CambiarContrasenia(usuarioAnonimo);
            MenuItem borrarItem = new MenuItem { Header = "Borrar Usuario" };
            borrarItem.Click += (s, args) => EliminarUsuario(usuarioAnonimo);
            menu.Items.Add(passwordItem);
            menu.Items.Add(borrarItem);
            // Mostramos el menú contextual manualmente
            menu.IsOpen = true;
        }

        public void CambiarContrasenia(object usuarioAnonimo)
        {
            var prop = usuarioAnonimo.GetType().GetProperty("Id_usuario");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador del Usuario seleccionada.");
                return;
            }

            int idUsuario = (int)prop.GetValue(usuarioAnonimo);

            using (var db = new MediGestContext())
            {
                var usuarioSeleccionado = db.Usuario.FirstOrDefault(c => c.Id_usuario == idUsuario);

                if (usuarioSeleccionado != null)
                {
                    var cambiarContrasenia = new CambiarPassword(usuarioSeleccionado);
                    cambiarContrasenia.ShowDialog();
                    CargarUsuarios(); // refrescar la tabla
                }
            }
        }

        public void EliminarUsuario(object usuarioAnonimo) {

            MessageBoxResult result = MessageBox.Show(
              "¿Estas seguro de Borrar este Usuario?",
              "Confirmacion",
              MessageBoxButton.YesNo,
              MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }
            else {
                MessageBoxResult result2 = MessageBox.Show(
                "¿Seguro,Seguro que quieres Borrar este Usuario?",
                "Confirmacion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if (result2 == MessageBoxResult.No) {
                    return;
                }
            }

            var prop = usuarioAnonimo.GetType().GetProperty("Id_usuario");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador del Usuario seleccionada.");
                return;
            }

            int idUsuario = (int)prop.GetValue(usuarioAnonimo);

            if (idUsuario == SessionManager.IdUsuario) {
                MessageBox.Show("No te puedes borrar a ti mismo");
                return;
            }

            using (var db = new MediGestContext()) {
                var usuarioSeleccionado = db.Usuario.FirstOrDefault(u => u.Id_usuario == idUsuario);

                if (usuarioSeleccionado != null)
                {

                    if (usuarioSeleccionado.Rol.ToLower() == "medico")
                    {

                        var medicoSeleccionado = db.Medico.FirstOrDefault(m => m.Id_usuario == idUsuario);
                        if (medicoSeleccionado != null)
                        {
                            var reemplazoMedico = new ReemplazarMedico(medicoSeleccionado);
                            reemplazoMedico.ShowDialog();
                            db.Remove(usuarioSeleccionado);
                            db.SaveChanges();
                            MessageBox.Show("Usuario y Medico Eliminados");
                            CargarUsuarios();
                        }
                    }
                    else if (usuarioSeleccionado.Rol.ToLower() == "recepcionista")
                    {
                        var recepcionistaSeleccionado = db.Recepcionista.FirstOrDefault(m => m.Id_usuario == idUsuario);
                        if (recepcionistaSeleccionado != null)
                        {
                            var reemplazoRecepcionista = new ReemplazarRecepcionista(recepcionistaSeleccionado);
                            reemplazoRecepcionista.ShowDialog();
                            db.Remove(usuarioSeleccionado);
                            db.SaveChanges();
                            MessageBox.Show("Usuario y Recepcionista Eliminados");
                            CargarUsuarios();
                        }
                    }
                    else
                    {
                        db.Remove(usuarioSeleccionado);
                        db.SaveChanges();
                        MessageBox.Show("Usuario Administrador Borrado");
                        CargarUsuarios();
                    }
                }
                else {

                    MessageBox.Show("No se encontró el Usuario seleccionado.");
                    return;
                }
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            CargarUsuarios();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string rol = cmbRol.Text;
            using (var db = new MediGestContext())
            {
                var query = from u in db.Usuario
                            where u.Rol == rol
                            select new
                            {
                                u.Id_usuario,
                                u.Login,
                                u.Password,
                                u.Rol
                            };

                if (rol != null)
                    query = query.Where(c => c.Rol.ToLower() == rol);

                DataGridUsuarios.ItemsSource = query.ToList();
            }
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            var crearUsuario = new CrearUsuario();
            crearUsuario.ShowDialog();
            CargarUsuarios();
        }
    }
}

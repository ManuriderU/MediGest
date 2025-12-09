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
using MediGest.Data;

namespace MediGest.Pages
{
    /// <summary>
    /// Lógica de interacción para Recepcionistas.xaml
    /// </summary>
    public partial class Recepcionistas : Page
    {
        string placeholderText = "Introduce nombre del Recepcionista a Buscar";
        public Recepcionistas()
        {
            InitializeComponent();
            CargarRecepcionistas();
            SetPlaceholder();
        }

        private void SetPlaceholder()
        {
            if (string.IsNullOrEmpty(txtBuscarRecepcionistas.Text))
            {
                txtBuscarRecepcionistas.Text = placeholderText;
                txtBuscarRecepcionistas.Foreground = Brushes.Gray;
            }
        }

        private void TxtBuscarPaciente_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscarRecepcionistas.Text == placeholderText)
            {
                txtBuscarRecepcionistas.Text = "";
                txtBuscarRecepcionistas.Foreground = Brushes.Black;
            }
        }

        private void DataGridRecepcionistas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var recepcionistaAnonimo = DataGridRecepcionistas.SelectedItem;
            if (recepcionistaAnonimo == null)
                return;

            var prop = recepcionistaAnonimo.GetType().GetProperty("Id_Recepcionista");
            if (prop == null)
            {
                MessageBox.Show("No se encontró el identificador del Usuario seleccionada.");
                return;
            }

            int idRecepcionista = (int)prop.GetValue(recepcionistaAnonimo);

            using (var db = new MediGestContext())
            {
                var recepcionistaSeleccionado = db.Recepcionista.FirstOrDefault(c => c.Id_recepcionista == idRecepcionista);

                if (recepcionistaSeleccionado != null)
                {
                    var ventanaEditar = new EditarRecepcionista(recepcionistaSeleccionado);
                    ventanaEditar.ShowDialog();
                    CargarRecepcionistas(); // refrescar la tabla
                }
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            CargarRecepcionistas();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new MediGestContext()) {

                string nombre = txtBuscarRecepcionistas.Text.Trim().ToLower();
                var query = db.Recepcionista.AsQueryable();

                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(m => (m.Nombre + " " + m.Apellidos).ToLower().Contains(nombre));
                }

                var resultado = query
               .Select(m => new
               {
                   m.Id_recepcionista,
                   Nombre = m.Nombre + " " + m.Apellidos,
                   m.Telefono,
                   m.Email
               })
               .ToList();

                DataGridRecepcionistas.ItemsSource = resultado;
            }
        }

        private void CargarRecepcionistas() {

            using (var db = new MediGestContext()) {

                var lista = (from p in db.Recepcionista
                             select new
                             {
                                 p.Id_recepcionista,
                                 Nombre = p.Nombre + " " + p.Apellidos,
                                 p.Telefono,
                                 p.Email
                             })
             .ToList();
                DataGridRecepcionistas.ItemsSource = lista;
            }

        }

    }
}

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
    /// Lógica de interacción para ReemplazarRecepcionista.xaml
    /// </summary>
    public partial class ReemplazarRecepcionista : Window
    {
        private Recepcionista recepcionistaActual;
        public ReemplazarRecepcionista(Recepcionista pRecepcionista)
        {
            InitializeComponent();
            recepcionistaActual = pRecepcionista;
            CargarRecepcionistas();
        }

        private void CargarRecepcionistas() {
            using (var db = new MediGestContext())
            {
                var recepcionistas = db.Recepcionista
                    .Select(m => new { m.Id_recepcionista, NombreCompleto = m.Nombre + " " + m.Apellidos })
                    .Where(m => m.Id_recepcionista != recepcionistaActual.Id_recepcionista)
                    .ToList();
                cmbRecepcionistas.ItemsSource = recepcionistas;
                cmbRecepcionistas.DisplayMemberPath = "NombreCompleto";
                cmbRecepcionistas.SelectedValuePath = "Id_recepcionista";
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRecepcionistas.SelectedItem != null) {
                using (var db = new MediGestContext()) {
                    var citas = db.Cita
                        .Where(c => c.Id_recepcionista == recepcionistaActual.Id_recepcionista)
                        .ToList();

                    if (citas.Count > 0) {

                        foreach (var cita in citas) {

                            cita.Id_recepcionista = (int)cmbRecepcionistas.SelectedValue;
                        }

                        db.SaveChanges();
                        MessageBox.Show("Recepcionista Reemplazado procediendo a borrar el Usuario y el Recepcionista relacionado a este...");
                        this.Close();
                    }

                    else {
                        MessageBox.Show("No hay Citas relacionadas al Recepcionista a borrar asi que procedemos a su eliminacion y a la de su Usuario...");
                        this.Close();
                    }
                }
            }
        }
    }
}

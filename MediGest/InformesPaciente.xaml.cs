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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MediGest.Clases;
using MediGest.Data;

namespace MediGest
{
    /// <summary>
    /// Lógica de interacción para InformesPaciente.xaml
    /// </summary>
    public partial class InformesPaciente : Window
    {
        Paciente pacienteActual;
        public InformesPaciente(Paciente p)
        {
            InitializeComponent();
            this.pacienteActual = p;
            CargarInformacion();
        }

        private void CargarInformacion()
        {
            try
            {
                this.Title = "Informes de " + pacienteActual.Nombre + " " + pacienteActual.Apellidos;
                using (var db = new MediGestContext())
                {
                    var lista = db.Informe_Medico
                        .Where(i => i.Id_paciente == pacienteActual.Id_paciente && i.Id_medico == SessionManager.IdUsuario)
                        .Select(i => new
                        {
                            i.Fecha_emision,
                            i.Motivo_consulta,
                            i.Diagnostico,
                            i.Tratamiento,
                            i.Observaciones
                        })
                        .ToList();
                    DataGridInformes.ItemsSource = lista;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al Buscar los Informes:\n{ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

    }
}

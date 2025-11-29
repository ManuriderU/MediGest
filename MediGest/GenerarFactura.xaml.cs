using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MediGest.Data;
using MediGest.Clases;

namespace MediGest
{
    /// <summary>
    /// Lógica de interacción para GenerarFactura.xaml
    /// </summary>
    public partial class GenerarFactura : Window
    {
        public GenerarFactura()
        {
            InitializeComponent();
            CargarPacientes();
            
        }

        private void CargarPacientes()
        {
            using (var db = new MediGestContext())
            {
                var pacientes = db.Paciente
                    .Select(p => new { p.Id_paciente, NombreCompleto = p.Nombre + " " + p.Apellidos })
                    .ToList();

                CmbPaciente.ItemsSource = pacientes;
                CmbPaciente.DisplayMemberPath = "NombreCompleto";
                CmbPaciente.SelectedValuePath = "Id_paciente";
            }
        }

        private void BtnGenerarFactura_Click(object sender, RoutedEventArgs e)
        {
            if (CmbPaciente.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un paciente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!FechaInicioPicker.SelectedDate.HasValue || !FechaFinPicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Selecciona las fechas de inicio y fin.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime inicio = FechaInicioPicker.SelectedDate.Value;
            DateTime fin = FechaFinPicker.SelectedDate.Value;

            if (inicio > fin)
            {
                MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int idPaciente = (int)CmbPaciente.SelectedValue;

            try
            {
                // Crear la ruta a la carpeta dentro del proyecto
                string carpetaFacturas = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Facturaciones");

                // Crear la carpeta si no existe
                if (!Directory.Exists(carpetaFacturas))
                    Directory.CreateDirectory(carpetaFacturas);

                // Nombre del archivo PDF
                string nombreArchivo = $"Factura_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                // Ruta completa final
                string ruta = System.IO.Path.Combine(carpetaFacturas, nombreArchivo);

                FacturaGenerator gen = new FacturaGenerator();
                gen.GenerarFactura(idPaciente, inicio, fin, ruta);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar la factura:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

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

namespace MediGest.Pages
{
    /// <summary>
    /// Lógica de interacción para Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void BtnRegistarPacienteClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Paciente Registrado");
        }

        private void BtnAgendarCitaClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cita Agendada");
        }

        private void BtnGenerarFacturaClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Factura Generada");
        }
    }
}

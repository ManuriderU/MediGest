using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediGest.Pages;

namespace MediGest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Brush activeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155"));
        private readonly Brush activeForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
        private readonly Brush defaultColor = Brushes.Transparent;
        private readonly Brush defaultForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));

        public MainWindow()
        {
            InitializeComponent();
            //Por Defecto Inicializamos la Page de Dashboard
            MainFrame.Navigate(new Dashboard());
            SetActiveButton(BtnDashboard);
        }

        //Eventos para las Pages
        private void BtnMostrarDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Dashboard());
            SetActiveButton(BtnDashboard);
        }

        private void BtnMostrarPacientes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pacientes());
            SetActiveButton(BtnPacientes);
        }

        private void BtnMostrarCitas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Citas());
            SetActiveButton(BtnCitas);
        }

        private void BtnMostrarFacturacion_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Facturacion());
            SetActiveButton(BtnFacturacion);
        }

        private void SetActiveButton(Button activeButton)
        {
            // Resetear colores
            BtnPacientes.Background = defaultColor;
            BtnCitas.Background = defaultColor;
            BtnFacturacion.Background = defaultColor;
            BtnDashboard.Background = defaultColor;
            
            //Resetear Foregrounds
            BtnFacturacion.Foreground = defaultForeground;
            BtnDashboard.Foreground = defaultForeground;
            BtnPacientes.Foreground = defaultForeground;
            BtnCitas.Foreground = defaultForeground;

            // Activar el botón actual

            activeButton.Background = activeColor;
            activeButton.Foreground = activeForeground;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Pacientes)
                SetActiveButton(BtnPacientes);
            else if (e.Content is Dashboard)
                SetActiveButton(BtnDashboard);
            else if (e.Content is Citas)
                SetActiveButton(BtnCitas);
            else if (e.Content is Facturacion)
                SetActiveButton(BtnFacturacion);
        }

    }
}
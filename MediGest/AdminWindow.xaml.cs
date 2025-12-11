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
using MediGest.Pages;

namespace MediGest
{
    /// <summary>
    /// Lógica de interacción para AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly Brush activeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155"));
        private readonly Brush activeForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
        private readonly Brush defaultColor = Brushes.Transparent;
        private readonly Brush defaultForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));

        public AdminWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Usuarios());
            SetActiveButton(BtnUsuarios);
        }

        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Usuarios());
            SetActiveButton(BtnUsuarios);
        }

        private void BtnMedicos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Medicos());
            SetActiveButton(BtnMedicos);
        }

        private void BtnRecepcionistas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Recepcionistas());
            SetActiveButton(BtnRecepcionistas);
        }


        private void SetActiveButton(Button activeButton)
        {
            // Resetear colores
            BtnMedicos.Background = defaultColor;
            BtnRecepcionistas.Background = defaultColor;
            BtnUsuarios.Background = defaultColor;

            //Resetear Foregrounds
            BtnUsuarios.Foreground = defaultForeground;
            BtnMedicos.Foreground = defaultForeground;
            BtnRecepcionistas.Foreground = defaultForeground;

            // Activar el botón actual

            activeButton.Background = activeColor;
            activeButton.Foreground = activeForeground;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Usuarios)
                SetActiveButton(BtnUsuarios);
            else if (e.Content is Medicos)
                SetActiveButton(BtnMedicos);
            else if (e.Content is Recepcionistas)
                SetActiveButton(BtnRecepcionistas);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Mostrar cuadro de confirmación
            var result = MessageBox.Show(
                "¿Deseas cerrar sesión?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true; // Cancelar cierre
                return;
            }

            // Si el usuario confirmó, limpiamos la sesión
            SessionManager.Reset();

            // Mostramos el Login otra vez
            Login login = new Login();
            login.Show();
        }

    }
}

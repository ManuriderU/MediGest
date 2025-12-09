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

namespace MediGest
{
    /// <summary>
    /// Lógica de interacción para CorreoMensaje.xaml
    /// </summary>
    public partial class CorreoMensaje : Window
    {
        public string Mensaje { get; private set; }

        public CorreoMensaje(string textoInicial = "")
        {
            InitializeComponent();
            TxtMensaje.Text = textoInicial;
            TxtMensaje.Focus();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Mensaje = TxtMensaje.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

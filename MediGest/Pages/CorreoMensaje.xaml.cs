using System.Windows;

namespace MediGest.Pages
{
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

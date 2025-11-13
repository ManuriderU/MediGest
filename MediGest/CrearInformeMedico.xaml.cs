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
    /// Lógica de interacción para CrearInformeMedico.xaml
    /// </summary>
    public partial class CrearInformeMedico : Window
    {
        private bool puedeCerrar = false;

        private Cita citaModificada;
        public CrearInformeMedico(Cita citaP)
        {
            InitializeComponent();
            citaModificada = citaP;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtDiagnostico.Text == "" && txtTratamiento.Text == "" && txtObservaciones.Text == "")
            {
                MessageBox.Show("Se deben Rellenar todos los Campos");
                return;
            }


            try
            {
                using (var db = new MediGestContext())
                {

                    var nuevoInforme = new Informe_Medico
                    {
                        Id_paciente = citaModificada.Id_paciente,
                        Id_medico = citaModificada.Id_medico,
                        Id_cita = citaModificada.Id_cita,
                        Fecha_emision = DateTime.Now,
                        Motivo_consulta = citaModificada.Observaciones,
                        Diagnostico = txtDiagnostico.Text,
                        Tratamiento = txtTratamiento.Text,
                        Observaciones = txtObservaciones.Text
                    };


                    db.Informe_Medico.Add(nuevoInforme);
                    db.SaveChanges();
                    MessageBox.Show("✅ Informe creado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    puedeCerrar = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la cita:\n{ex.InnerException?.Message ?? ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!puedeCerrar)
            {
                e.Cancel = true; // ❌ Cancela el cierre
                MessageBox.Show("No puedes cerrar esta ventana hasta guardar el informe.");
            }
        }
    }
}
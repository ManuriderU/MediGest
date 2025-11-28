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
    /// Lógica de interacción para ReemplazarMedico.xaml
    /// </summary>
    public partial class ReemplazarMedico : Window
    {
        private Medico medicoActual;
        public ReemplazarMedico(Medico pMedico)
        {
            InitializeComponent();
            medicoActual = pMedico;
            CargarMedicos();
        }

        private void CargarMedicos()
        {
            using (var db = new MediGestContext())
            {
                var medicos = db.Medico
                    .Select(m => new { m.Id_medico, NombreCompleto = m.Nombre + " " + m.Apellidos })
                    .Where(m => m.Id_medico != medicoActual.Id_medico)
                    .ToList();
                cmbMedicos.ItemsSource = medicos;
                cmbMedicos.DisplayMemberPath = "NombreCompleto";
                cmbMedicos.SelectedValuePath = "Id_medico";
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMedicos.SelectedItem != null) {
                using (var db = new MediGestContext()) {
                    var citas = db.Cita.
                        Where(c => c.Id_medico == medicoActual.Id_medico)
                        .ToList();

                    var informesMedicos = db.Informe_Medico
                        .Where(i => i.Id_medico == medicoActual.Id_medico)
                        .ToList();


                    if (citas.Count > 0 && informesMedicos.Count > 0)
                    {
                        foreach (var cita in citas) {

                            cita.Id_medico = (int)cmbMedicos.SelectedValue;
                        }

                        foreach (var informe in informesMedicos) {

                            informe.Id_medico = (int)cmbMedicos.SelectedValue;
                        }

                        db.SaveChanges();
                        MessageBox.Show("Medico Reemplazado,procediendo a la eliminacion de este y su Usuario...");
                        this.Close();
                    }
                    else if (citas.Count > 0 && informesMedicos.Count == 0) {

                        foreach (var cita in citas)
                        {
                            cita.Id_medico = (int)cmbMedicos.SelectedValue;
                        }
                        db.SaveChanges();
                        MessageBox.Show("Medico Reemplazado,procediendo a la eliminacion de este y su Usuario...");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No hay citas relacionadas con este Medico por lo cual tampoco hay informes relacionados,procediendo a la eliminacion de este y su Usuario...");
                        this.Close();
                    }
                }
            }
        }
    }
}

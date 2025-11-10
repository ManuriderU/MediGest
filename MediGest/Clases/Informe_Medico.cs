using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    internal class Informe_Medico
    {
        private int id_informe;
        //private int id_cita;             // FK a Cita
        private string id_paciente;         // FK a Paciente
        private string id_medico;           // FK a Medico
        private string fecha_emision;
        private string diagnostico;
        private string tratamiento;
        private string observaciones;

        //Constructores
        public Informe_Medico(int id_informe, string id_paciente, string id_medico, string fecha_emision, string diagnostico, string tratamiento, string observaciones)
        {
            this.id_informe = id_informe;
            this.id_paciente = id_paciente;
            this.id_medico = id_medico;
            this.fecha_emision = fecha_emision;
            this.diagnostico = diagnostico;
            this.tratamiento = tratamiento;
            this.observaciones = observaciones;
        }

        public Informe_Medico()
        {
            this.id_informe = 0;
            this.id_paciente = "";
            this.id_medico = "";
            this.fecha_emision = "";
            this.diagnostico = "";
            this.tratamiento = "";
            this.observaciones = "";
        }

        //Getters y Setters
        public int Id_informe { get => id_informe; set => id_informe = value; }
        public string Id_paciente { get => id_paciente; set => id_paciente = value; }
        public string Id_medico { get => id_medico; set => id_medico = value; }
        public string Fecha_emision { get => fecha_emision; set => fecha_emision = value; }
        public string Diagnostico { get => diagnostico; set => diagnostico = value; }
        public string Tratamiento { get => tratamiento; set => tratamiento = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }


        //Métodos
        private void GenerarInforme()
        {
            //Lógica para generar un informe médico
        }

        private void GuardarInforme()
        {
            //Lógica para guardar un informe médico
        }

        private void EnviarInformeAlPaciente()
        {
            //Lógica para enviar el informe médico al paciente
        }

        private void ActualizarInforme()
        {
            //Lógica para actualizar un informe médico
        }

        private void EliminarInforme()
        {
            //Lógica para eliminar un informe médico
        }

        private void ConsultarInforme()
        {
            //Lógica para consultar un informe médico
        }

        private void ImprimirInforme()
        {
            //Lógica para imprimir un informe médico
        }

        private void FirmarInforme()
        {
            //Lógica para firmar un informe médico
        }

        private void DescargarInforme()
        {
            //Lógica para descargar un informe médico
        }

        private void EnviarInformePorCorreo()
        {
            //Lógica para enviar el informe médico por correo electrónico
        }

        private void ExportarInformeAFormatoPDF()
        {
            //Lógica para exportar el informe médico a formato PDF
        }

    }
}

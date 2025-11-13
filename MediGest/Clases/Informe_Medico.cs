using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    public class Informe_Medico
    {
        private int id_informe;
        private int id_cita;             // FK a Cita
        private int id_paciente;         // FK a Paciente
        private int id_medico;           // FK a Medico
        private DateTime fecha_emision;
        private string motivo_consulta;
        private string diagnostico;
        private string tratamiento;
        private string observaciones;

        //Constructores
        public Informe_Medico(int id_informe, int id_paciente,int id_cita, int id_medico, DateTime fecha_emision, string motivo_consulta, string diagnostico, string tratamiento, string observaciones)
        {
            this.id_informe = id_informe;
            this.id_paciente = id_paciente;
            this.id_cita = id_cita;
            this.id_medico = id_medico;
            this.fecha_emision = fecha_emision;
            this.motivo_consulta = motivo_consulta;
            this.diagnostico = diagnostico;
            this.tratamiento = tratamiento;
            this.observaciones = observaciones;
        }

        public Informe_Medico()
        {
            this.id_informe = 0;
            this.id_paciente = 0;
            this.id_cita= 0;
            this.id_medico = 0;
            this.fecha_emision = DateTime.MinValue;
            this.motivo_consulta = "";
            this.diagnostico = "";
            this.tratamiento = "";
            this.observaciones = "";
        }

        //Getters y Setters
        [Key]
        public int Id_informe { get => id_informe; set => id_informe = value; }
        public int Id_paciente { get => id_paciente; set => id_paciente = value; }
        public int Id_medico { get => id_medico; set => id_medico = value; }
        public int Id_cita { get => id_cita; set => id_cita = value; }
        public DateTime Fecha_emision { get => fecha_emision; set => fecha_emision = value; }
        public string Motivo_consulta { get => motivo_consulta; set => motivo_consulta = value; }
        public string Diagnostico { get => diagnostico; set => diagnostico = value; }
        public string Tratamiento { get => tratamiento; set => tratamiento = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }

    }
}

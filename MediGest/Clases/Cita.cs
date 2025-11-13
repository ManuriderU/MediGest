using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    public class Cita
    {
        
        private int id_cita;
        private int id_paciente;         // FK a Paciente
        private int id_medico;          // FK a Medico
        private int id_recepcionista;    // FK a Recepcionista
        private DateTime fecha;
        private TimeSpan hora;
        private string estado;
        private string observaciones;

        //Constructores
        public Cita(int id_cita, int id_paciente, int id_medico, int id_recepcionista, DateTime fecha, TimeSpan hora, string estado, string observaciones)
        {
            this.id_cita = id_cita;
            this.id_paciente = id_paciente;
            this.id_medico = id_medico;
            this.id_recepcionista = id_recepcionista;
            this.fecha = fecha;
            this.hora = hora;
            this.estado = estado;
            this.observaciones = observaciones;
        }

        public Cita()
        {
            this.id_cita = 0;
            this.id_paciente = 0;
            this.id_medico = 0;
            this.id_recepcionista = 0;
            this.fecha = DateTime.MinValue;
            this.hora = TimeSpan.Zero;
            this.estado = "";
            this.observaciones = "";
        }

        //Getters y Setters
        [Key]
        public int Id_cita { get => id_cita; set => id_cita = value; }
        public int Id_paciente { get => id_paciente; set => id_paciente = value; }
        public int Id_medico { get => id_medico; set => id_medico = value; }
        public int Id_recepcionista { get => id_recepcionista; set => id_recepcionista = value; }
        public DateTime Fecha { get => fecha; set => fecha = value; }
        public TimeSpan Hora { get => hora; set => hora = value; }
        public string Estado { get => estado; set => estado = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }

    }
}

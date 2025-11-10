using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    internal class Paciente
    {
        private int id_paciente;
        private string nombre;
        private string apellidos;
        private string dni;
        private string cipa;
        private string num_historial_clinica;
        private string num_seguridad_social;
        //private string sexo;
        //private string telefono;
        //private string email;
        private DateTime fecha_nacimiento;
        //private string direccion;

        //Constructores
        public Paciente(int id_paciente, string nombre, string apellidos, string dni, string cipa, string num_historial_clinica, string num_seguridad_social, DateTime fecha_nacimiento)
        {
            this.id_paciente = id_paciente;
            this.nombre = nombre;
            this.apellidos = apellidos;
            this.dni = dni;
            this.cipa = cipa;
            this.num_historial_clinica = num_historial_clinica;
            this.num_seguridad_social = num_seguridad_social;
            this.fecha_nacimiento = fecha_nacimiento;
        }

        public Paciente()
        {
            this.id_paciente = 0;
            this.nombre = "";
            this.apellidos = "";
            this.dni = "";
            this.cipa = "";
            this.num_historial_clinica = "";
            this.num_seguridad_social = "";
            this.fecha_nacimiento = DateTime.MinValue;
        }

        //Getters y Setters
        public int Id_paciente { get => id_paciente; set => id_paciente = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellidos { get => apellidos; set => apellidos = value; }
        public string Dni { get => dni; set => dni = value; }
        public string Cipa { get => cipa; set => cipa = value; }
        public string Num_historial_clinica { get => num_historial_clinica; set => num_historial_clinica = value; }
        public string Num_seguridad_social { get => num_seguridad_social; set => num_seguridad_social = value; }
        public DateTime Fecha_nacimiento { get => fecha_nacimiento; set => fecha_nacimiento = value; }

    }
}

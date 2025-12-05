using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    public class Paciente
    {
        private int id_paciente;
        private string nombre;
        private string apellidos;
        private string dni;
        private string cipa;
        private string num_historia_clinica;
        private string num_seguridad_social;
        private DateTime fecha_nacimiento;
        private string correo;

        //Constructores
        public Paciente(int id_paciente, string nombre, string apellidos, string dni, string cipa, string num_historia_clinica, string num_seguridad_social, DateTime fecha_nacimiento, string correo)
        {
            this.id_paciente = id_paciente;
            this.nombre = nombre;
            this.apellidos = apellidos;
            this.dni = dni;
            this.cipa = cipa;
            this.num_historia_clinica = num_historia_clinica;
            this.num_seguridad_social = num_seguridad_social;
            this.fecha_nacimiento = fecha_nacimiento;
            this.correo = correo;
        }

        public Paciente()
        {
            this.id_paciente = 0;
            this.nombre = "";
            this.apellidos = "";
            this.dni = "";
            this.cipa = "";
            this.num_historia_clinica = "";
            this.num_seguridad_social = "";
            this.fecha_nacimiento = DateTime.MinValue;
            this.correo = "";
        }

        //Getters y Setters
        [Key]
        public int Id_paciente { get => id_paciente; set => id_paciente = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellidos { get => apellidos; set => apellidos = value; }
        public string Dni { get => dni; set => dni = value; }
        public string Cipa { get => cipa; set => cipa = value; }
        public string Num_historia_clinica { get => num_historia_clinica; set => num_historia_clinica = value; }
        public string Num_seguridad_social { get => num_seguridad_social; set => num_seguridad_social = value; }
        public DateTime Fecha_nacimiento { get => fecha_nacimiento; set => fecha_nacimiento = value; }
        public string Correo { get => correo; set => correo = value; }

    }
}

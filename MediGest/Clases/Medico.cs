using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    public class Medico
    {
        private int id_medico;
        private int id_usuario;          // FK a Usuario
        private int id_especialidad;     // FK a Especialidad
        private string nombre;
        private string apellidos;
        private string num_colegiado;
        private string correo_corporativo;

        //Constructores
        public Medico(int id_medico, int id_usuario, int id_especialidad, string nombre, string apellidos, string num_colegiado, string correo_corporativo)
        {
            this.id_medico = id_medico;
            this.id_usuario = id_usuario;
            this.id_especialidad = id_especialidad;
            this.nombre = nombre;
            this.apellidos = apellidos;
            this.num_colegiado = num_colegiado;
            this.correo_corporativo = correo_corporativo;
        }

        public Medico()
        {
            this.id_medico = 0;
            this.id_usuario = 0;
            this.id_especialidad = 0;
            this.nombre = "";
            this.apellidos = "";
            this.num_colegiado = "";
            this.correo_corporativo = "";
        }

        //Getters y Setters
        [Key]
        public int Id_medico { get => id_medico; set => id_medico = value; }
        public int Id_usuario { get => id_usuario; set => id_usuario = value; }
        public int Id_especialidad { get => id_especialidad; set => id_especialidad = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellidos { get => apellidos; set => apellidos = value; }
        public string Num_colegiado { get => num_colegiado; set => num_colegiado = value; }
        public string Correo_corporativo { get => correo_corporativo; set => correo_corporativo = value; }

    }
}

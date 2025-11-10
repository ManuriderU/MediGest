using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    internal class Especialidad
    {
        private int id_especialidad;
        private string nombre;
        private string descripcion;

        //Constructores
        public Especialidad(int id_especialidad, string nombre, string descripcion)
        {
            this.id_especialidad = id_especialidad;
            this.nombre = nombre;
            this.descripcion = descripcion;
        }

        public Especialidad()
        {
            this.id_especialidad = 0;
            this.nombre = "";
            this.descripcion = "";
        }

        //Getters y Setters
        public int Id_especialidad { get => id_especialidad; set => id_especialidad = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        
    }
}

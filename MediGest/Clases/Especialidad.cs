using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    public class Especialidad
    {
        private int id_especialidad;
        private string nombre;
        private string descripcion;
        private decimal precio;

        //Constructores
        public Especialidad(int id_especialidad, string nombre, string descripcion,decimal precio)
        {
            this.id_especialidad = id_especialidad;
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.precio = precio;
        }

        public Especialidad()
        {
            this.id_especialidad = 0;
            this.nombre = "";
            this.descripcion = "";
            this.precio = 0;
        }

        //Getters y Setters
        [Key]
        public int Id_especialidad { get => id_especialidad; set => id_especialidad = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public decimal Precio { get => precio; set => precio = value; } 
        
    }
}

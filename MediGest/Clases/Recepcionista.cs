using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    internal class Recepcionista
    {
        private int id_recepcionista;
        private string id_usuario;      // FK a Usuario
        private string nombre;
        private string apellidos;
        private string telefono;
        private string email;

        //Constructores

        public Recepcionista(int id_recepcionista, string id_usuario, string nombre, string apellidos, string telefono, string email)
        {
            this.id_recepcionista = id_recepcionista;
            this.id_usuario = id_usuario;
            this.nombre = nombre;
            this.apellidos = apellidos;
            this.telefono = telefono;
            this.email = email;
        }
        
        public Recepcionista()
        {
            this.id_recepcionista = 0;
            this.id_usuario = "";
            this.nombre = "";
            this.apellidos = "";
            this.telefono = "";
            this.email = "";
        }

        //Getters y Setters
        public int Id_recepcionista { get => id_recepcionista; set => id_recepcionista = value; }
        public string Id_usuario { get => id_usuario; set => id_usuario = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellidos { get => apellidos; set => apellidos = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public string Email { get => email; set => email = value; }

        //Métodos
        private void AgendarCita()
        {
            //Lógica para agendar una cita
        }

        private void GestionarPacientes()
        {
            //Lógica para gestionar pacientes
        }
        
        private void GestionarHistoriales()
        {
            //Lógica para gestionar historiales médicos
        }

        private void CoordinarConMedicos()
        {
            //Lógica para coordinar con médicos
        }

        private void ManejarDocumentacion()
        {
            //Lógica para manejar documentación
        }

        private void FacturacionYPagos()
        {
            //Lógica para facturación y pagos
        }
       
    }
}

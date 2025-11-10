using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    internal class Usuario
    {
        private int id_usuario;
        private string login;
        private string password;
        private string rol;


        //Constructores 
        
        public Usuario(int id_usuario, string login, string password, string rol)
        {
            this.id_usuario = id_usuario;
            this.login = login;
            this.password = password;
            this.rol = rol;
        }

        public Usuario()
        {
            this.id_usuario = 0;
            this.login = "";
            this.password = "";
            this.rol = "";
        }

        //Getters y Setters
        public int Id_usuario { get => id_usuario; set => id_usuario = value; }
        public string Login { get => login; set => login = value; }
        public string Password { get => password; set => password = value; }
        public string Rol { get => rol; set => rol = value; }

    }
}

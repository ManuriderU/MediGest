using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediGest.Clases
{
    public static class SessionManager
    {
        public static int IdUsuario { get; set; }
        public static string Nombre { get; set; }
        public static string Apellidos { get; set; }
        public static string Rol { get; set; }

        public static void Reset()
        {
            IdUsuario = 0;
            Nombre = "";
            Apellidos = "";
            Rol = "";
        }


    }

}

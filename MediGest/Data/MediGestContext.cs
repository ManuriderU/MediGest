using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediGest.Clases;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

namespace MediGest.Data
{
    public class MediGestContext : DbContext
    {
        public DbSet<Paciente> Paciente { get; set; }
        public DbSet<Medico> Medico { get; set; }
        public DbSet<Recepcionista> Recepcionista { get; set; }
        public DbSet<Especialidad> Especialidad { get; set; }
        public DbSet<Cita> Cita { get; set; }
        public DbSet<Informe_Medico> Informe_Medico { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(
                "server=127.0.0.1;port=3306;database=medigest;user=root;",
                new MySqlServerVersion(new Version(10, 4, 32))
            );
        }
    }
}

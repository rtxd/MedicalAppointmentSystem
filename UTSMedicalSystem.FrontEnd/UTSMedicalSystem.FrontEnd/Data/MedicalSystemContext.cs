using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTSMedicalSystem.FrontEnd.Models;

namespace UTSMedicalSystem.FrontEnd.Data
{
    public class MedicalSystemContext : DbContext
    {
        public MedicalSystemContext(DbContextOptions<MedicalSystemContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Appointment>().ToTable("Appointment");
        }
    }
}

using APBD9.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD9.Context;

public class HospitalDbContext : DbContext {
    
    public HospitalDbContext() {
    }

    public HospitalDbContext(DbContextOptions options) : base(options) {
    }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=APBD9;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Doctor>(opt => {
            opt.HasKey(e => e.IdDoctor);
            opt.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.Email).HasMaxLength(100).IsRequired();
        });
        
        modelBuilder.Entity<Patient>(opt => {
            opt.HasKey(e => e.IdPatient);
            opt.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.BirthDate).IsRequired();
        });
        modelBuilder.Entity<Medicament>(opt => {
            opt.HasKey(e => e.IdMedicament);
            opt.Property(e => e.Name).HasMaxLength(100).IsRequired();
            opt.Property(e => e.Description).HasMaxLength(100).IsRequired();
            opt.Property(e => e.Type).HasMaxLength(100).IsRequired();
            opt.HasMany(e => e.PrescriptionMedicaments)
                .WithOne(e => e.Medicament)
                .HasForeignKey(e => e.idMedicament);
        });
        modelBuilder.Entity<Prescription>(opt => {
            opt.HasKey(e => e.IdPrescription);
            opt.Property(e => e.Date).IsRequired();
            opt.Property(e => e.DueDate).IsRequired();
            opt.HasOne(e => e.Doctor)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(e => e.IdDoctor);
            opt.HasOne(e => e.Patient)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(e => e.IdPatient);
            opt.HasMany(e => e.PrescriptionMedicaments)
                .WithOne(e => e.Prescription)
                .HasForeignKey(e => e.idPrescription);
        });
        modelBuilder.Entity<Prescription_Medicament>(opt => {
            opt.HasKey(e => new {
                e.idMedicament,
                e.idPrescription
            });
            opt.Property(e => e.Dose);
            opt.Property(e => e.Details).HasMaxLength(100).IsRequired();
        });

    }
}
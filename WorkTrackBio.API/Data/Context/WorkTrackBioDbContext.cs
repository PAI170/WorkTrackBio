using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Data.Context
{
    public class WorkTrackBioDbContext : DbContext
    {
        public WorkTrackBioDbContext(DbContextOptions<WorkTrackBioDbContext> options) : base(options)
        {
        }

        // DbSets para las entidades
        public DbSet<State> States { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<InternUser> InternUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<EmployeeInfo> EmployeeInfos { get; set; }
        public DbSet<Assistance> Assistances { get; set; }
        public DbSet<ProjectsAssigns> ProjectsAssigns { get; set; }
        public DbSet<AuditRegister> AuditRegisters { get; set; }
        public DbSet<FingerPrint> FingerPrints { get; set; }
        public DbSet<ProjectMaintenance> ProjectMaintenances { get; set; }
        public DbSet<ProjectWarranty> ProjectWarranties { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            ConfigureTableNames(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigureIndexes(modelBuilder);
            ConfigureConstraints(modelBuilder);
            ConfigureComputedColumns(modelBuilder);
        }

        private void ConfigureTableNames(ModelBuilder modelBuilder)
        {
            // Configurar nombres de tablas para que coincidan con la BD
            modelBuilder.Entity<State>().ToTable("States");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<InternUser>().ToTable("InternUsers");
            modelBuilder.Entity<Project>().ToTable("Projects");
            modelBuilder.Entity<DocumentType>().ToTable("DocumentType");
            modelBuilder.Entity<EmployeeInfo>().ToTable("EmployeeInfo");
            modelBuilder.Entity<Assistance>().ToTable("Assistance");
            modelBuilder.Entity<ProjectsAssigns>().ToTable("ProjectsAssigns");
            modelBuilder.Entity<AuditRegister>().ToTable("AuditRegister");
            modelBuilder.Entity<FingerPrint>().ToTable("FingerPrint");
            modelBuilder.Entity<ProjectMaintenance>().ToTable("ProjectMaintenance");
            modelBuilder.Entity<ProjectWarranty>().ToTable("ProjectWarranty");
            modelBuilder.Entity<Device>().ToTable("Devices");
            modelBuilder.Entity<Session>().ToTable("Sessions");
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Configurar relaciones y comportamientos de cascada
            
            // InternUsers
            modelBuilder.Entity<InternUser>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InternUser>()
                .HasOne(u => u.State)
                .WithMany()
                .HasForeignKey(u => u.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Projects
            modelBuilder.Entity<Project>()
                .HasOne(p => p.State)
                .WithMany()
                .HasForeignKey(p => p.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            // EmployeeInfo
            modelBuilder.Entity<EmployeeInfo>()
                .HasOne(e => e.DocumentType)
                .WithMany()
                .HasForeignKey(e => e.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeInfo>()
                .HasOne(e => e.State)
                .WithMany()
                .HasForeignKey(e => e.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assistance
            modelBuilder.Entity<Assistance>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assistance>()
                .HasOne(a => a.Project)
                .WithMany()
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Configurar índices únicos básicos
            
            // States
            modelBuilder.Entity<State>()
                .HasIndex(s => new { s.StateName, s.StateType })
                .IsUnique()
                .HasDatabaseName("IX_States_StateName");

            // Roles
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique()
                .HasDatabaseName("IX_Roles_RoleName");

            // InternUsers
            modelBuilder.Entity<InternUser>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_InternUsers_Email");

            // Projects
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.ProjectName)
                .IsUnique()
                .HasDatabaseName("IX_Projects_ProjectName");

            // DocumentType
            modelBuilder.Entity<DocumentType>()
                .HasIndex(dt => dt.DocumentName)
                .IsUnique()
                .HasDatabaseName("IX_DocumentType_DocumentName");

            // EmployeeInfo
            modelBuilder.Entity<EmployeeInfo>()
                .HasIndex(e => e.DocumentNumber)
                .IsUnique()
                .HasDatabaseName("IX_EmployeeInfo_DocumentNumber");
        }

        private void ConfigureConstraints(ModelBuilder modelBuilder)
        {
            // Configurar constraints básicos usando la nueva sintaxis
            
            // Projects - validación de fechas
            modelBuilder.Entity<Project>()
                .ToTable(t => t.HasCheckConstraint("CK_Projects_Dates_Logical", 
                    "[EndDate] IS NULL OR [StartDate] IS NULL OR [EndDate] >= [StartDate]"));

            // Assistance - validaciones
            modelBuilder.Entity<Assistance>()
                .ToTable(t => t.HasCheckConstraint("CK_Assistance_CheckOut_After_CheckIn", 
                    "[CheckOut] IS NULL OR [CheckOut] >= [CheckIn]"));
        }

        private void ConfigureComputedColumns(ModelBuilder modelBuilder)
        {
            // Configurar columnas computadas
            
            // Assistance CheckInDateOnly como columna computada
            modelBuilder.Entity<Assistance>()
                .Property(a => a.CheckInDateOnly)
                .HasComputedColumnSql("CAST([CheckIn] AS DATE)", stored: true);
        }
    }
}

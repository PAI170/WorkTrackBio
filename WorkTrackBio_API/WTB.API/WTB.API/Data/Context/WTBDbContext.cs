using Microsoft.EntityFrameworkCore;
using WTB.API.Models.Entities;

namespace WTB.API.Data.Context
{
    public class WTBDbContext : DbContext
    {
        public WTBDbContext(DbContextOptions<WTBDbContext> options) : base(options)
        {
        }

        // DbSets - Representan las tablas en la base de datos
        public DbSet<States> States { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<InternUsers> InternUsers { get; set; }
        public DbSet<EmployeeInfo> EmployeeInfo { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<Assistance> Assistance { get; set; }
        public DbSet<ProjectsAssigns> ProjectsAssigns { get; set; }
        public DbSet<AuditRegister> AuditRegister { get; set; }
        public DbSet<FingerPrint> FingerPrint { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<ProjectMaintenance> ProjectMaintenance { get; set; }
        public DbSet<ProjectWarranty> ProjectWarranty { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===============================
            // CONFIGURACIONES DE TABLAS
            // ===============================

            // Estados - Configuración de índices únicos
            modelBuilder.Entity<States>(entity =>
            {
                entity.HasIndex(e => new { e.StateName, e.StateType })
                      .IsUnique()
                      .HasDatabaseName("IX_States_StateName");

                entity.Property(e => e.StateName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.StateType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(50);
            });

            // Roles - Configuración de índices únicos
            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasIndex(e => e.RoleName)
                      .IsUnique()
                      .HasDatabaseName("IX_Roles_RoleName");

                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
            });

            // Tipo de Documento - Configuración de índices únicos
            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasIndex(e => e.DocumentName)
                      .IsUnique()
                      .HasDatabaseName("IX_DocumentType_DocumentName");

                entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
            });

            // Usuarios Internos
            modelBuilder.Entity<InternUsers>(entity =>
            {
                // Índice único para email
                entity.HasIndex(e => e.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_InternUsers_Email");

                // Índice para RolId (para consultas frecuentes)
                entity.HasIndex(e => e.RolId)
                      .HasDatabaseName("IX_InternUsers_RolId");

                // Propiedades
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordSalt).IsRequired().HasMaxLength(100);
                // La entidad InternUsers hereda de AuditableEntity que ya tiene CreatedDate

                // Relaciones Foreign Key
                entity.HasOne(d => d.Role)
                      .WithMany(p => p.InternUsers)
                      .HasForeignKey(d => d.RolId)
                      .HasConstraintName("FK_InternUsers_RolId");

                entity.HasOne(d => d.State)
                      .WithMany(p => p.InternUsers)
                      .HasForeignKey(d => d.StateId)
                      .HasConstraintName("FK_InternUsers_StateId");
            });

            // Información de Empleados
            modelBuilder.Entity<EmployeeInfo>(entity =>
            {
                // Índice único para número de documento
                entity.HasIndex(e => e.DocumentNumber)
                      .IsUnique()
                      .HasDatabaseName("IX_EmployeeInfo_DocumentNumber");

                // Índice para StateId
                entity.HasIndex(e => e.StateId)
                      .HasDatabaseName("IX_EmployeeInfo_StateId");

                // Índice para DocumentTypeId
                entity.HasIndex(e => e.DocumentTypeId)
                      .HasDatabaseName("IX_EmployeeInfo_DocumentTypeId");

                // Índice compuesto para nombre completo (consultas frecuentes)
                entity.HasIndex(e => new { e.FirstName, e.LastName })
                      .HasDatabaseName("IX_EmployeeInfo_FullName");

                // Propiedades
                entity.Property(e => e.DocumentNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.EmergencyContact).HasMaxLength(100);
                entity.Property(e => e.EmergencyContactPhoneNumber).HasMaxLength(20);
                entity.Property(e => e.RegisterDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CostPerHour).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.IBAN).HasMaxLength(50);

                // Relaciones Foreign Key
                entity.HasOne(d => d.DocumentType)
                      .WithMany(p => p.EmployeeInfo)
                      .HasForeignKey(d => d.DocumentTypeId)
                      .HasConstraintName("FK_EmployeeDocumentType_DocumentTypeId");

                entity.HasOne(d => d.State)
                      .WithMany(p => p.EmployeeInfo)
                      .HasForeignKey(d => d.StateId)
                      .HasConstraintName("FK_Employee_StateId");
            });

            // Proyectos
            modelBuilder.Entity<Projects>(entity =>
            {
                // Índice único para nombre de proyecto
                entity.HasIndex(e => e.ProjectName)
                      .IsUnique()
                      .HasDatabaseName("IX_Projects_ProjectName");

                // Índice para StateId
                entity.HasIndex(e => e.StateId)
                      .HasDatabaseName("IX_Projects_StateId");

                // Propiedades
                entity.Property(e => e.ProjectName).IsRequired().HasMaxLength(150);

                // Relación Foreign Key
                entity.HasOne(d => d.State)
                      .WithMany(p => p.Projects)
                      .HasForeignKey(d => d.StateId)
                      .HasConstraintName("FK_Projects_StateId");
            });

            // Asistencia
            modelBuilder.Entity<Assistance>(entity =>
            {
                // Índice compuesto para consultas frecuentes (empleado, proyecto, fecha)
                entity.HasIndex(e => new { e.EmployeeId, e.ProjectId, e.CheckIn })
                      .HasDatabaseName("IX_Assistance_EmployeeProjectCheckIn");

                // Índice para ProjectId
                entity.HasIndex(e => e.ProjectId)
                      .HasDatabaseName("IX_Assistance_ProjectId");

                // Índice para consultas por fecha (usando columna computada)
                entity.HasIndex(e => e.CheckInDateOnly)
                      .HasDatabaseName("IX_Assistance_CheckInDate");

                // Propiedades
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.RegisterType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalHours).HasColumnType("decimal(5,2)");

                // Columna computada - se calcula automáticamente en la DB
                entity.Property(e => e.CheckInDateOnly)
                      .HasComputedColumnSql("CAST([CheckIn] AS DATE)")
                      .ValueGeneratedOnAddOrUpdate();

                // Relaciones Foreign Key
                entity.HasOne(d => d.Employee)
                      .WithMany(p => p.Assistances)
                      .HasForeignKey(d => d.EmployeeId)
                      .HasConstraintName("FK_EmployeeId_EmployeeInfo");

                entity.HasOne(d => d.Project)
                      .WithMany(p => p.Assistances)
                      .HasForeignKey(d => d.ProjectId)
                      .HasConstraintName("FK_ProjectId_Projects");

                // Evitar que EF Core cree relaciones automáticas con Devices
                entity.Ignore("DeviceId");
            });

            // Asignaciones de Proyecto
            modelBuilder.Entity<ProjectsAssigns>(entity =>
            {
                // Índice compuesto para consultas frecuentes
                entity.HasIndex(e => new { e.EmployeeId, e.ProjectId, e.AssignDate })
                      .HasDatabaseName("IX_ProjectsAssigns_EmployeeProjectAssign");

                // Índice para ProjectId
                entity.HasIndex(e => e.ProjectId)
                      .HasDatabaseName("IX_ProjectsAssigns_ProjectId");

                // Propiedades
                entity.Property(e => e.AssignDate).HasDefaultValueSql("GETDATE()");

                // Relaciones Foreign Key
                entity.HasOne(d => d.Employee)
                      .WithMany(p => p.ProjectsAssigns)
                      .HasForeignKey(d => d.EmployeeId)
                      .HasConstraintName("FK_ProjectsAssigns_EmployeeId");

                entity.HasOne(d => d.Project)
                      .WithMany(p => p.ProjectsAssigns)
                      .HasForeignKey(d => d.ProjectId)
                      .HasConstraintName("FK_ProjectAssigns_ProjectId");
            });

            // Registro de Auditoría
            modelBuilder.Entity<AuditRegister>(entity =>
            {
                // Índices para consultas frecuentes
                entity.HasIndex(e => e.AssistanceId)
                      .HasDatabaseName("IX_AuditRegister_AssistanceId");

                entity.HasIndex(e => e.AdminId)
                      .HasDatabaseName("IX_AuditRegister_AdminId");

                entity.HasIndex(e => e.ActionDate)
                      .HasDatabaseName("IX_AuditRegister_ActionDate");

                // Propiedades
                entity.Property(e => e.ActionType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DetailChange).IsRequired();
                entity.Property(e => e.ActionDate).HasDefaultValueSql("GETDATE()");

                // Relaciones Foreign Key
                entity.HasOne(d => d.Assistance)
                      .WithMany(p => p.AuditRegisters)
                      .HasForeignKey(d => d.AssistanceId)
                      .HasConstraintName("FK_AssitanceId_Assistance");

                entity.HasOne(d => d.Admin)
                      .WithMany(p => p.AuditRegisters)
                      .HasForeignKey(d => d.AdminId)
                      .HasConstraintName("FK_AuditRegister_InterUser");
            });

            // Huella Dactilar
            modelBuilder.Entity<FingerPrint>(entity =>
            {
                // Índice para EmployeeId
                entity.HasIndex(e => e.EmployeeId)
                      .HasDatabaseName("IX_FingerPrint_EmployeeId");

                // Propiedades
                entity.Property(e => e.TemplateFingerPrint).IsRequired();
                entity.Property(e => e.IssueDate).HasDefaultValueSql("GETDATE()");

                // Relación Foreign Key
                entity.HasOne(d => d.Employee)
                      .WithMany(p => p.FingerPrints)
                      .HasForeignKey(d => d.EmployeeId)
                      .HasConstraintName("FK_FingerPrint_EmployeeId");
            });

            // Mantenimiento de Proyecto
            modelBuilder.Entity<ProjectMaintenance>(entity =>
            {
                // Índices
                entity.HasIndex(e => e.IdProject)
                      .HasDatabaseName("IX_ProjectMaintenance_IdProject");

                entity.HasIndex(e => e.MadeById)
                      .HasDatabaseName("IX_ProjectMaintenance_MadeById");

                // Propiedades
                entity.Property(e => e.MaintenanceDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.MaintenanceDescription).IsRequired();
                entity.Property(e => e.MaintenanceCost).HasColumnType("decimal(10,2)");
                entity.Property(e => e.AdditionalInfo).HasMaxLength(255);

                // Relaciones Foreign Key
                entity.HasOne(d => d.Project)
                      .WithMany(p => p.ProjectMaintenances)
                      .HasForeignKey(d => d.IdProject)
                      .HasConstraintName("FK_ProjectMaintenance_ProjectId");

                entity.HasOne(d => d.MadeBy)
                      .WithMany(p => p.ProjectMaintenancesMadeBy)
                      .HasForeignKey(d => d.MadeById)
                      .HasConstraintName("FK_Maintenance_MadeById");

                entity.HasOne(d => d.State)
                      .WithMany(p => p.ProjectMaintenances)
                      .HasForeignKey(d => d.StateId)
                      .HasConstraintName("FK_ProjectMaintenance_StateId");
            });

            // Garantía de Proyecto
            modelBuilder.Entity<ProjectWarranty>(entity =>
            {
                // Índices
                entity.HasIndex(e => e.IdProject)
                      .HasDatabaseName("IX_ProjectWarranty_IdProject");

                entity.HasIndex(e => e.MadeById)
                      .HasDatabaseName("IX_ProjectWarranty_MadeById");

                // Propiedades
                entity.Property(e => e.WarrantyDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.WarrantyDescription).IsRequired();
                entity.Property(e => e.WarrantyCost).HasColumnType("decimal(10,2)");

                // Relaciones Foreign Key
                entity.HasOne(d => d.Project)
                      .WithMany(p => p.ProjectWarranties)
                      .HasForeignKey(d => d.IdProject)
                      .HasConstraintName("FK_ProjectWarranty_IdProject");

                entity.HasOne(d => d.MadeBy)
                      .WithMany(p => p.ProjectWarrantiesMadeBy)
                      .HasForeignKey(d => d.MadeById)
                      .HasConstraintName("FK_ProjectWarranty_MadeById");

                entity.HasOne(d => d.State)
                      .WithMany(p => p.ProjectWarranties)
                      .HasForeignKey(d => d.StateId)
                      .HasConstraintName("FK_ProjectWarranty_StateId");
            });
        }
    }
}
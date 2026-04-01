using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Common;
using WorkTrackBio.API.Data.Models;
using System.Linq.Expressions;
using System.Text.Json;

namespace WorkTrackBio.API.Data
{
    public class AppDbContext : DbContext
    {
        // ─── Campos privados ──────────────────────────────────────────────────
        private readonly IHttpContextAccessor _httpContextAccessor = null!;

        // ─── Constructor ──────────────────────────────────────────────────────
        public AppDbContext(DbContextOptions<AppDbContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // ─── DbSets ───────────────────────────────────────────────────────────

        // Catálogos
        public DbSet<State> States { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Empleados
        public DbSet<EmployeeInfo> EmployeeInfos { get; set; }
        public DbSet<EmployeeDeduction> EmployeeDeductions { get; set; }
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

        // Usuarios y seguridad
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<SystemAuditLog> SystemAuditLogs { get; set; }

        // Clientes y proyectos
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMaintenance> ProjectMaintenances { get; set; }
        public DbSet<ProjectWarranty> ProjectWarranties { get; set; }

        // Asistencia y gastos
        public DbSet<Assistance> Assistances { get; set; }
        public DbSet<TravelExpense> TravelExpenses { get; set; }

        // Planillas
        public DbSet<PayrollRun> PayrollRuns { get; set; }
        public DbSet<PayrollEntry> PayrollEntries { get; set; }
        public DbSet<PayrollDeduction> PayrollDeductions { get; set; }

        // Beneficios y obligaciones laborales
        public DbSet<VacationRequest> VacationRequests { get; set; }
        public DbSet<AbsenceRecord> AbsenceRecords { get; set; }
        public DbSet<Disability> Disabilities { get; set; }
        public DbSet<ChristmasBonus> ChristmasBonuses { get; set; }
        public DbSet<ChristmasBonusDetail> ChristmasBonusDetails { get; set; }
        public DbSet<Liquidation> Liquidations { get; set; }

        // ─── OnModelCreating ──────────────────────────────────────────────────

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Soft delete: filtro global para todas las entidades BaseEntity ─
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)
                    && !entityType.ClrType.IsAbstract)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(BuildIsDeletedFilter(entityType.ClrType));
                }
            }

            // ── Enums como string ──────────────────────────────────────────────

            modelBuilder.Entity<State>()
                .Property(s => s.StateType)
                .HasConversion<string>();

            modelBuilder.Entity<EmployeeInfo>()
                .OwnsOne(e => e.Salary, salary =>
                {
                    salary.Property(s => s.PaymentFrequency)
                          .HasConversion<string>();
                });

            modelBuilder.Entity<PayrollRun>()
                .Property(p => p.PayFrequency)
                .HasConversion<string>();

            modelBuilder.Entity<PayrollRun>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<VacationRequest>()
                .Property(v => v.Status)
                .HasConversion<string>();

            modelBuilder.Entity<AbsenceRecord>()
                .Property(a => a.AbsenceType)
                .HasConversion<string>();

            modelBuilder.Entity<Disability>()
                .Property(d => d.DisabilityType)
                .HasConversion<string>();

            modelBuilder.Entity<Liquidation>()
                .Property(l => l.Status)
                .HasConversion<string>();

            // ── Relaciones con múltiples FK al mismo modelo ────────────────────

            // Assistance → EmployeeInfo y Project
            modelBuilder.Entity<Assistance>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Assistances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assistance>()
                .HasOne(a => a.Project)
                .WithMany(p => p.Assistances)
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // PayrollRun → AppUser (creado por / aprobado por)
            modelBuilder.Entity<PayrollRun>()
                .HasOne(p => p.CreatedBy)
                .WithMany(u => u.CreatedPayrollRuns)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollRun>()
                .HasOne(p => p.ApprovedBy)
                .WithMany(u => u.ApprovedPayrollRuns)
                .HasForeignKey(p => p.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // PayrollEntry → EmployeeInfo
            modelBuilder.Entity<PayrollEntry>()
                .HasOne(p => p.EmployeeInfo)
                .WithMany(e => e.PayrollEntries)
                .HasForeignKey(p => p.EmployeeInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            // TravelExpense → EmployeeInfo (empleado / aprobado por)
            modelBuilder.Entity<TravelExpense>()
                .HasOne(t => t.EmployeeInfo)
                .WithMany()
                .HasForeignKey(t => t.EmployeeInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TravelExpense>()
                .HasOne(t => t.ApprovedBy)
                .WithMany()
                .HasForeignKey(t => t.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // TravelExpense → Project y State
            modelBuilder.Entity<TravelExpense>()
                .HasOne(t => t.Project)
                .WithMany(p => p.TravelExpenses)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TravelExpense>()
                .HasOne(t => t.State)
                .WithMany(s => s.TravelExpenses)
                .HasForeignKey(t => t.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectMaintenance → EmployeeInfo y Project
            modelBuilder.Entity<ProjectMaintenance>()
                .HasOne(p => p.Project)
                .WithMany(pr => pr.ProjectMaintenances)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMaintenance>()
                .HasOne(p => p.MadeBy)
                .WithMany()
                .HasForeignKey(p => p.MadeById)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectWarranty → EmployeeInfo y Project
            modelBuilder.Entity<ProjectWarranty>()
                .HasOne(p => p.Project)
                .WithMany(pr => pr.ProjectWarranties)
                .HasForeignKey(p => p.IdProject)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectWarranty>()
                .HasOne(p => p.MadeBy)
                .WithMany()
                .HasForeignKey(p => p.MadeById)
                .OnDelete(DeleteBehavior.Restrict);

            // AbsenceRecord → EmployeeInfo (empleado / registrado por)
            modelBuilder.Entity<AbsenceRecord>()
                .HasOne(a => a.EmployeeInfo)
                .WithMany(e => e.AbsenceRecords)
                .HasForeignKey(a => a.EmployeeInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AbsenceRecord>()
                .HasOne(a => a.RegisteredBy)
                .WithMany()
                .HasForeignKey(a => a.RegisteredById)
                .OnDelete(DeleteBehavior.Restrict);

            // VacationRequest → EmployeeInfo (empleado / aprobado por)
            modelBuilder.Entity<VacationRequest>()
                .HasOne(v => v.EmployeeInfo)
                .WithMany(e => e.VacationRequests)
                .HasForeignKey(v => v.EmployeeInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VacationRequest>()
                .HasOne(v => v.ApprovedBy)
                .WithMany()
                .HasForeignKey(v => v.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ChristmasBonus → EmployeeInfo (empleado / confirmado por)
            modelBuilder.Entity<ChristmasBonus>()
                .HasOne(c => c.EmployeeInfo)
                .WithMany(e => e.ChristmasBonuses)
                .HasForeignKey(c => c.EmployeeInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChristmasBonus>()
                .HasOne(c => c.ConfirmedBy)
                .WithMany()
                .HasForeignKey(c => c.ConfirmedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Liquidation → EmployeeInfo (empleado / confirmado por)
            modelBuilder.Entity<Liquidation>()
                .HasOne(l => l.EmployeeInfo)
                .WithMany(e => e.Liquidations)
                .HasForeignKey(l => l.EmployeeInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Liquidation>()
                .HasOne(l => l.ConfirmedBy)
                .WithMany()
                .HasForeignKey(l => l.ConfirmedById)
                .OnDelete(DeleteBehavior.Restrict);

            // SystemAuditLog y UserSession → AppUser
            // AppUser tiene global filter de soft delete; estas tablas son logs/sesiones
            // que no participan en soft delete, por eso se configuran explícitamente.
            modelBuilder.Entity<SystemAuditLog>()
                .HasOne(s => s.AppUser)
                .WithMany(u => u.SystemAuditLogs)
                .HasForeignKey(s => s.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserSession>()
                .HasOne(s => s.AppUser)
                .WithMany(u => u.UserSessions)
                .HasForeignKey(s => s.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Índices ────────────────────────────────────────────────────────

            modelBuilder.Entity<PayrollRun>()
                .HasIndex(p => p.PaymentDate);

            modelBuilder.Entity<PayrollEntry>()
                .HasIndex(p => p.EmployeeInfoId);

            modelBuilder.Entity<Assistance>()
                .HasIndex(a => new { a.EmployeeId, a.CheckIn });

            modelBuilder.Entity<SystemAuditLog>()
                .HasIndex(s => new { s.TableName, s.RecordId });

            modelBuilder.Entity<VacationRequest>()
                .HasIndex(v => new { v.EmployeeInfoId, v.StartDate });

            modelBuilder.Entity<AbsenceRecord>()
                .HasIndex(a => new { a.EmployeeInfoId, a.AbsenceDate });

            modelBuilder.Entity<ChristmasBonus>()
                .HasIndex(c => new { c.EmployeeInfoId, c.Year })
                .IsUnique();
        }

        // ─── Auditoría automática ─────────────────────────────────────────────

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            HandleAuditableEntities();
            AuditChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void HandleAuditableEntities()
        {
            var now = DateTime.UtcNow;
            var userId = GetCurrentUserId();

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        break;

                    case EntityState.Deleted:
                        // Convertir hard delete en soft delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedById = userId;
                        break;
                }
            }
        }

        private void AuditChanges()
        {
            if (_httpContextAccessor?.HttpContext == null) return;

            var userId = GetCurrentUserId();
            if (userId == null) return;

            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                if (entry.Entity is SystemAuditLog) continue;

                // Detectar si es un soft delete
                string action = entry.State.ToString();
                if (entry.State == EntityState.Modified
                    && entry.Entity is BaseEntity baseEntity
                    && baseEntity.IsDeleted
                    && entry.Property(nameof(BaseEntity.IsDeleted)).IsModified)
                {
                    action = "SoftDeleted";
                }

                var audit = new SystemAuditLog
                {
                    AppUserId = userId.Value,
                    TableName = entry.Metadata.GetTableName()
                                ?? entry.Entity.GetType().Name,
                    Action = action,
                    Timestamp = DateTime.UtcNow,
                    RecordId = entry.State != EntityState.Added
                                ? (int?)entry.Property("Id").CurrentValue
                                : null,
                    OldValues = entry.State == EntityState.Modified
                             || entry.State == EntityState.Deleted
                                ? JsonSerializer.Serialize(
                                    entry.Properties
                                         .Where(p => p.IsModified
                                                  || entry.State == EntityState.Deleted)
                                         .ToDictionary(
                                             p => p.Metadata.Name,
                                             p => p.OriginalValue))
                                : null,
                    NewValues = entry.State == EntityState.Added
                             || entry.State == EntityState.Modified
                                ? JsonSerializer.Serialize(
                                    entry.Properties
                                         .Where(p => p.IsModified
                                                  || entry.State == EntityState.Added)
                                         .ToDictionary(
                                             p => p.Metadata.Name,
                                             p => p.CurrentValue))
                                : null,
                };

                SystemAuditLogs.Add(audit);
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private static LambdaExpression BuildIsDeletedFilter(Type entityType)
        {
            var parameter = Expression.Parameter(entityType, "e");
            var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            return Expression.Lambda(condition, parameter);
        }
    }
}

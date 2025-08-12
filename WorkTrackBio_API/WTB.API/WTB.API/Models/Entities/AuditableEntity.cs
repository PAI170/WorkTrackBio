using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.Entities
{
    /// <summary>
    /// Clase base para entidades que requieren auditoría automática
    /// </summary>
    public abstract class AuditableEntity
    {
        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de la última modificación
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// ID del usuario que creó el registro
        /// </summary>
        public int? CreatedById { get; set; }

        /// <summary>
        /// ID del usuario que realizó la última modificación
        /// </summary>
        public int? ModifiedById { get; set; }

        /// <summary>
        /// Marca la entidad como modificada
        /// </summary>
        /// <param name="userId">ID del usuario que realiza la modificación</param>
        public virtual void MarkAsModified(int userId)
        {
            ModifiedDate = DateTime.UtcNow;
            ModifiedById = userId;
        }

        /// <summary>
        /// Obtiene el ID de la entidad para auditoría
        /// </summary>
        /// <returns>ID de la entidad como string</returns>
        public abstract string GetEntityId();

        /// <summary>
        /// Obtiene el nombre del tipo de entidad para auditoría
        /// </summary>
        /// <returns>Nombre del tipo de entidad</returns>
        public abstract string GetEntityType();
    }
}

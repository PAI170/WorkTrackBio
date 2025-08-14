using WTB.API.Models.Entities;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de lookups que proporciona datos de catálogos y referencias
    /// </summary>
    public interface ILookupService
    {
        #region Estados
        
        /// <summary>
        /// Obtiene todos los estados del sistema
        /// </summary>
        Task<IEnumerable<States>> GetAllStatesAsync();
        
        /// <summary>
        /// Obtiene estados por tipo específico
        /// </summary>
        /// <param name="stateType">Tipo de estado (General, Project, Employee)</param>
        Task<IEnumerable<States>> GetStatesByTypeAsync(string stateType);
        
        /// <summary>
        /// Obtiene un estado por ID
        /// </summary>
        /// <param name="id">ID del estado</param>
        Task<States?> GetStateByIdAsync(int id);
        
        #endregion
        
        #region Roles
        
        /// <summary>
        /// Obtiene todos los roles del sistema
        /// </summary>
        Task<IEnumerable<Roles>> GetAllRolesAsync();
        
        /// <summary>
        /// Obtiene un rol por ID
        /// </summary>
        /// <param name="id">ID del rol</param>
        Task<Roles?> GetRoleByIdAsync(int id);
        
        #endregion
        
        #region Tipos de Documento
        
        /// <summary>
        /// Obtiene todos los tipos de documento
        /// </summary>
        Task<IEnumerable<DocumentType>> GetAllDocumentTypesAsync();
        
        /// <summary>
        /// Obtiene un tipo de documento por ID
        /// </summary>
        /// <param name="id">ID del tipo de documento</param>
        Task<DocumentType?> GetDocumentTypeByIdAsync(int id);
        
        #endregion
        
        #region Operaciones Combinadas
        
        /// <summary>
        /// Obtiene todos los lookups del sistema en una sola llamada
        /// </summary>
        Task<LookupData> GetAllLookupsAsync();
        
        #endregion
    }
    
    /// <summary>
    /// DTO que contiene todos los datos de lookup del sistema
    /// </summary>
    public class LookupData
    {
        public IEnumerable<States> States { get; set; } = Enumerable.Empty<States>();
        public IEnumerable<Roles> Roles { get; set; } = Enumerable.Empty<Roles>();
        public IEnumerable<DocumentType> DocumentTypes { get; set; } = Enumerable.Empty<DocumentType>();
    }
}

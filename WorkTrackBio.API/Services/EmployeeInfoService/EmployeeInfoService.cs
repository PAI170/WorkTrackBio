using AutoMapper;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.EmployeeInfo;
using WorkTrackBio.API.Repositories.EmployeeInfoRepository;
using WorkTrackBio.API.Repositories.StateRepository;
using WorkTrackBio.API.Repositories.DocumentTypeRepository;
using WorkTrackBio.API.Validators.DocumentValidator;
using WorkTrackBio.API.Validators.PhoneNumberFormatter;
using System.Linq;

namespace WorkTrackBio.API.Services.EmployeeInfoService
{
    public class EmployeeInfoService : IEmployeeInfoService
    {
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IDocumentValidator _documentValidator;
        private readonly IPhoneNumberFormatter _phoneNumberFormatter;
        private readonly IMapper _mapper;

        public EmployeeInfoService(
            IEmployeeInfoRepository employeeInfoRepository,
            IStateRepository stateRepository,
            IDocumentTypeRepository documentTypeRepository,
            IDocumentValidator documentValidator,
            IPhoneNumberFormatter phoneNumberFormatter,
            IMapper mapper)
        {
            _employeeInfoRepository = employeeInfoRepository ?? throw new ArgumentNullException(nameof(employeeInfoRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
            _documentValidator = documentValidator ?? throw new ArgumentNullException(nameof(documentValidator));
            _phoneNumberFormatter = phoneNumberFormatter ?? throw new ArgumentNullException(nameof(phoneNumberFormatter));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<EmployeeInfoDataTransferObject>> GetAllEmployeesAsync()
        {
            var employees = await _employeeInfoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeInfoDataTransferObject>>(employees);
        }

        public async Task<EmployeeInfoDataTransferObject?> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(id));

            var employee = await _employeeInfoRepository.GetByIdAsync(id);
            return _mapper.Map<EmployeeInfoDataTransferObject>(employee);
        }

        public async Task<EmployeeInfoDataTransferObject?> GetEmployeeByDocumentNumberAsync(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("El número de documento no puede estar vacío", nameof(documentNumber));

            if (documentTypeId <= 0)
                throw new ArgumentException("El ID del tipo de documento debe ser mayor que 0", nameof(documentTypeId));

            var employee = await _employeeInfoRepository.GetByDocumentNumberAsync(documentNumber, documentTypeId);
            return _mapper.Map<EmployeeInfoDataTransferObject>(employee);
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            var employee = await _employeeInfoRepository.GetByIdAsync(id);
            return employee != null;
        }

        public async Task<bool> EmployeeDocumentExistsAsync(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return false;

            if (documentTypeId <= 0)
                return false;

            return await _employeeInfoRepository.ExistsByDocumentNumberAsync(documentNumber, documentTypeId);
        }

        public async Task<IEnumerable<EmployeeInfoDataTransferObject>> GetEmployeesByStateAsync(int stateId)
        {
            if (stateId <= 0)
                throw new ArgumentException("El ID del estado debe ser mayor que 0", nameof(stateId));

            // Verificar que el estado exista
            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
                throw new ArgumentException($"No existe un estado con ID {stateId}", nameof(stateId));

            var employees = await _employeeInfoRepository.GetByStateAsync(stateId);
            return _mapper.Map<IEnumerable<EmployeeInfoDataTransferObject>>(employees);
        }

        public async Task<IEnumerable<EmployeeInfoDataTransferObject>> GetEmployeesByDocumentTypeAsync(int documentTypeId)
        {
            if (documentTypeId <= 0)
                throw new ArgumentException("El ID del tipo de documento debe ser mayor que 0", nameof(documentTypeId));

            // Verificar que el tipo de documento exista
            var documentTypeExists = await _documentTypeRepository.GetByIdAsync(documentTypeId);
            if (documentTypeExists == null)
                throw new ArgumentException($"No existe un tipo de documento con ID {documentTypeId}", nameof(documentTypeId));

            var employees = await _employeeInfoRepository.GetByDocumentTypeAsync(documentTypeId);
            return _mapper.Map<IEnumerable<EmployeeInfoDataTransferObject>>(employees);
        }

        public async Task<EmployeeInfoDataTransferObject> CreateEmployeeAsync(CreateEmployeeInfoDataTransferObject createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            // Verificar que el estado exista
            var stateExists = await _stateRepository.GetByIdAsync(createDto.StateId);
            if (stateExists == null)
                throw new ArgumentException($"No existe un estado con ID {createDto.StateId}", nameof(createDto.StateId));

            // Verificar que el tipo de documento exista
            var documentTypeExists = await _documentTypeRepository.GetByIdAsync(createDto.DocumentTypeId);
            if (documentTypeExists == null)
                throw new ArgumentException($"No existe un tipo de documento con ID {createDto.DocumentTypeId}", nameof(createDto.DocumentTypeId));

            // Validar formato del documento según su tipo
            var documentValidation = await _documentValidator.ValidateDocumentAsync(createDto.DocumentNumber, createDto.DocumentTypeId);
            if (!documentValidation.IsValid)
                throw new InvalidOperationException($"Error en el formato del documento: {documentValidation.ErrorMessage}");

            // Verificar que no exista otro empleado con el mismo número de documento y tipo
            var existingEmployee = await _employeeInfoRepository.GetByDocumentNumberAsync(createDto.DocumentNumber, createDto.DocumentTypeId);
            if (existingEmployee != null)
                throw new InvalidOperationException($"Ya existe un empleado con el número de documento '{createDto.DocumentNumber}' del tipo '{documentTypeExists.DocumentName}'");

            // Validar fechas
            if (createDto.DocumentExpire.HasValue && createDto.DocumentExpire.Value < DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("La fecha de expiración del documento no puede ser anterior a hoy");

            if (createDto.Birthday > DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("La fecha de nacimiento no puede ser futura");

            if (createDto.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-100)))
                throw new InvalidOperationException("La fecha de nacimiento no puede ser anterior a 100 años");

            // Validar costo por hora
            if (createDto.CostPerHour.HasValue && createDto.CostPerHour.Value < 0)
                throw new InvalidOperationException("El costo por hora no puede ser negativo");

            var employee = _mapper.Map<WorkTrackBio.API.Data.Models.EmployeeInfo>(createDto);
            
            // Formatear números según las restricciones de la base de datos
            employee.DocumentNumber = _phoneNumberFormatter.CleanDocumentNumber(employee.DocumentNumber) ?? employee.DocumentNumber;
            employee.PhoneNumber = _phoneNumberFormatter.FormatPhoneNumber(employee.PhoneNumber);
            employee.EmergencyContactPhoneNumber = _phoneNumberFormatter.FormatPhoneNumber(employee.EmergencyContactPhoneNumber);
            
            var createdEmployee = await _employeeInfoRepository.CreateAsync(employee);

            return _mapper.Map<EmployeeInfoDataTransferObject>(createdEmployee);
        }

        public async Task<EmployeeInfoDataTransferObject?> UpdateEmployeeAsync(UpdateEmployeeInfoDataTransferObject updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(updateDto.Id));

            var existingEmployee = await _employeeInfoRepository.GetByIdAsync(updateDto.Id);
            if (existingEmployee == null)
                return null;

            bool hasChanges = false;

            // Crear un diccionario para rastrear qué campos fueron explícitamente enviados
            var sentFields = new HashSet<string>();
            var updateDtoType = typeof(UpdateEmployeeInfoDataTransferObject);
            foreach (var property in updateDtoType.GetProperties())
            {
                if (property.Name != "Id" && property.GetValue(updateDto) != null)
                {
                    sentFields.Add(property.Name);
                }
            }

            // Validar y actualizar número de documento
            if (sentFields.Contains("DocumentNumber"))
            {
                // Determinar el tipo de documento a usar para la validación
                var documentTypeIdForValidation = updateDto.DocumentTypeId ?? existingEmployee.DocumentTypeId;
                
                // Validar formato del documento según su tipo
                var documentValidation = await _documentValidator.ValidateDocumentAsync(updateDto.DocumentNumber!, documentTypeIdForValidation);
                if (!documentValidation.IsValid)
                    throw new InvalidOperationException($"Error en el formato del documento: {documentValidation.ErrorMessage}");

                if (updateDto.DocumentTypeId.HasValue)
                {
                    var newDocumentTypeId = updateDto.DocumentTypeId.Value;
                    var otherEmployee = await _employeeInfoRepository.GetByDocumentNumberAsync(updateDto.DocumentNumber!, newDocumentTypeId);
                    if (otherEmployee != null && otherEmployee.Id != updateDto.Id)
                        throw new InvalidOperationException($"Ya existe otro empleado con el número de documento '{updateDto.DocumentNumber}' del tipo especificado");
                }
                else
                {
                    var otherEmployee = await _employeeInfoRepository.GetByDocumentNumberAsync(updateDto.DocumentNumber!, existingEmployee.DocumentTypeId);
                    if (otherEmployee != null && otherEmployee.Id != updateDto.Id)
                        throw new InvalidOperationException($"Ya existe otro empleado con el número de documento '{updateDto.DocumentNumber}' del mismo tipo");
                }

                var cleanedDocumentNumber = _phoneNumberFormatter.CleanDocumentNumber(updateDto.DocumentNumber!);
                if (!string.Equals(existingEmployee.DocumentNumber, cleanedDocumentNumber, StringComparison.OrdinalIgnoreCase))
                {
                    existingEmployee.DocumentNumber = cleanedDocumentNumber ?? updateDto.DocumentNumber!;
                    hasChanges = true;
                }
            }

            // Validar y actualizar tipo de documento
            if (sentFields.Contains("DocumentTypeId") && updateDto.DocumentTypeId.HasValue)
            {
                var documentTypeExists = await _documentTypeRepository.GetByIdAsync(updateDto.DocumentTypeId.Value);
                if (documentTypeExists == null)
                    throw new ArgumentException($"No existe un tipo de documento con ID {updateDto.DocumentTypeId.Value}", nameof(updateDto.DocumentTypeId));

                if (existingEmployee.DocumentTypeId != updateDto.DocumentTypeId.Value)
                {
                    // Verificar que no haya conflicto con el nuevo tipo de documento
                    if (sentFields.Contains("DocumentNumber") && updateDto.DocumentNumber != null)
                    {
                        var otherEmployee = await _employeeInfoRepository.GetByDocumentNumberAsync(updateDto.DocumentNumber, updateDto.DocumentTypeId.Value);
                        if (otherEmployee != null && otherEmployee.Id != updateDto.Id)
                            throw new InvalidOperationException($"Ya existe otro empleado con el número de documento '{updateDto.DocumentNumber}' del tipo '{documentTypeExists.DocumentName}'");
                    }

                    existingEmployee.DocumentTypeId = updateDto.DocumentTypeId.Value;
                    hasChanges = true;
                }
            }

            // Validar y actualizar fecha de expiración
            if (sentFields.Contains("DocumentExpire") && updateDto.DocumentExpire.HasValue)
            {
                if (updateDto.DocumentExpire.Value < DateOnly.FromDateTime(DateTime.Today))
                    throw new InvalidOperationException("La fecha de expiración del documento no puede ser anterior a hoy");

                if (existingEmployee.DocumentExpire != updateDto.DocumentExpire.Value)
                {
                    existingEmployee.DocumentExpire = updateDto.DocumentExpire.Value;
                    hasChanges = true;
                }
            }

            // Validar y actualizar nombres
            if (sentFields.Contains("FirstName") && updateDto.FirstName != null &&
                !string.Equals(existingEmployee.FirstName, updateDto.FirstName, StringComparison.OrdinalIgnoreCase))
            {
                existingEmployee.FirstName = updateDto.FirstName;
                hasChanges = true;
            }

            if (sentFields.Contains("LastName") && updateDto.LastName != null &&
                !string.Equals(existingEmployee.LastName, updateDto.LastName, StringComparison.OrdinalIgnoreCase))
            {
                existingEmployee.LastName = updateDto.LastName;
                hasChanges = true;
            }

            // Validar y actualizar teléfono
            if (sentFields.Contains("PhoneNumber"))
            {
                if (updateDto.PhoneNumber == null)
                {
                    // Campo explícitamente enviado como null
                    existingEmployee.PhoneNumber = null;
                    hasChanges = true;
                }
                else
                {
                    // Campo con valor, formatear y actualizar
                    var formattedPhoneNumber = _phoneNumberFormatter.FormatPhoneNumber(updateDto.PhoneNumber);
                    if (existingEmployee.PhoneNumber != formattedPhoneNumber)
                    {
                        existingEmployee.PhoneNumber = formattedPhoneNumber;
                        hasChanges = true;
                    }
                }
            }

            // Validar y actualizar contacto de emergencia
            if (sentFields.Contains("EmergencyContact"))
            {
                existingEmployee.EmergencyContact = updateDto.EmergencyContact;
                hasChanges = true;
            }

            if (sentFields.Contains("EmergencyContactPhoneNumber"))
            {
                if (updateDto.EmergencyContactPhoneNumber == null)
                {
                    // Campo explícitamente enviado como null
                    existingEmployee.EmergencyContactPhoneNumber = null;
                    hasChanges = true;
                }
                else
                {
                    // Campo con valor, formatear y actualizar
                    var formattedEmergencyPhone = _phoneNumberFormatter.FormatPhoneNumber(updateDto.EmergencyContactPhoneNumber);
                    if (existingEmployee.EmergencyContactPhoneNumber != formattedEmergencyPhone)
                    {
                        existingEmployee.EmergencyContactPhoneNumber = formattedEmergencyPhone;
                        hasChanges = true;
                    }
                }
            }

            // Validar y actualizar fecha de nacimiento
            if (sentFields.Contains("Birthday") && updateDto.Birthday.HasValue)
            {
                if (updateDto.Birthday.Value > DateOnly.FromDateTime(DateTime.Today))
                    throw new InvalidOperationException("La fecha de nacimiento no puede ser futura");

                if (updateDto.Birthday.Value < DateOnly.FromDateTime(DateTime.Today.AddYears(-100)))
                    throw new InvalidOperationException("La fecha de nacimiento no puede ser anterior a 100 años");

                if (existingEmployee.Birthday != updateDto.Birthday.Value)
                {
                    existingEmployee.Birthday = updateDto.Birthday.Value;
                    hasChanges = true;
                }
            }

            // Validar y actualizar costo por hora
            if (sentFields.Contains("CostPerHour") && updateDto.CostPerHour.HasValue)
            {
                if (updateDto.CostPerHour.Value < 0)
                    throw new InvalidOperationException("El costo por hora no puede ser negativo");

                if (existingEmployee.CostPerHour != updateDto.CostPerHour.Value)
                {
                    existingEmployee.CostPerHour = updateDto.CostPerHour.Value;
                    hasChanges = true;
                }
            }

            // Validar y actualizar estado
            if (sentFields.Contains("StateId") && updateDto.StateId.HasValue)
            {
                var stateExists = await _stateRepository.GetByIdAsync(updateDto.StateId.Value);
                if (stateExists == null)
                    throw new ArgumentException($"No existe un estado con ID {updateDto.StateId.Value}", nameof(updateDto.StateId));

                if (existingEmployee.StateId != updateDto.StateId.Value)
                {
                    existingEmployee.StateId = updateDto.StateId.Value;
                    hasChanges = true;
                }
            }

            // Validar y actualizar dirección
            if (sentFields.Contains("Address"))
            {
                existingEmployee.Address = updateDto.Address;
                hasChanges = true;
            }

            // Validar y actualizar IBAN
            if (sentFields.Contains("IBAN"))
            {
                existingEmployee.IBAN = updateDto.IBAN;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return _mapper.Map<EmployeeInfoDataTransferObject>(existingEmployee);
            }

            var updatedEmployee = await _employeeInfoRepository.UpdateAsync(existingEmployee);
            return _mapper.Map<EmployeeInfoDataTransferObject>(updatedEmployee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
                return false;

            var employee = await _employeeInfoRepository.GetByIdAsync(id);
            if (employee == null)
                return false;

            var isEmployeeInUse = await IsEmployeeInUseAsync(id);
            if (isEmployeeInUse)
                throw new InvalidOperationException($"No se puede eliminar el empleado '{employee.FirstName} {employee.LastName}' porque está siendo usado por otras entidades del sistema");

            return await _employeeInfoRepository.DeleteAsync(id);
        }

        private async Task<bool> IsEmployeeInUseAsync(int employeeInfoId)
        {
            var hasDependencies = await _employeeInfoRepository.HasDependenciesAsync(employeeInfoId);
            return hasDependencies;
        }
    }
}

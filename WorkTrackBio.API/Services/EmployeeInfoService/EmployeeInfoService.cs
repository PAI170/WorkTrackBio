using AutoMapper;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.EmployeeInfo;
using WorkTrackBio.API.Repositories.EmployeeInfoRepository;
using WorkTrackBio.API.Repositories.StateRepository;
using WorkTrackBio.API.Repositories.DocumentTypeRepository;
using WorkTrackBio.API.Validators.DocumentValidator;
using WorkTrackBio.API.Validators.PhoneNumberFormatter;

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

        public async Task<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>> GetAllEmployeesAsync()
        {
            var employees = await _employeeInfoRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<EmployeeInfoDataTransferObject>>(employees);
            return ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.SuccessResponse(result, "Empleados obtenidos exitosamente");
        }

        public async Task<ApiResponse<EmployeeInfoDataTransferObject>> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var employee = await _employeeInfoRepository.GetByIdAsync(id);

            if (employee == null)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"No se encontró un empleado con ID {id}", 404);

            var result = _mapper.Map<EmployeeInfoDataTransferObject>(employee);
            return ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(result, "Empleado obtenido exitosamente");
        }

        public async Task<ApiResponse<EmployeeInfoDataTransferObject>> GetEmployeeByDocumentNumberAsync(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("El número de documento no puede estar vacío", 400);

            if (documentTypeId <= 0)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("El ID del tipo de documento debe ser mayor que 0", 400);

            var employee = await _employeeInfoRepository.GetByDocumentNumberAsync(documentNumber, documentTypeId);

            if (employee == null)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"No se encontró un empleado con el número de documento '{documentNumber}'", 404);

            var result = _mapper.Map<EmployeeInfoDataTransferObject>(employee);
            return ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(result, "Empleado obtenido exitosamente");
        }

        public async Task<ApiResponse<bool>> EmployeeDocumentExistsAsync(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return ApiResponse<bool>.ErrorResponse("El número de documento no puede estar vacío", 400);

            if (documentTypeId <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID del tipo de documento debe ser mayor que 0", 400);

            var exists = await _employeeInfoRepository.ExistsByDocumentNumberAsync(documentNumber, documentTypeId);
            return ApiResponse<bool>.SuccessResponse(exists, exists ? "El documento ya está registrado" : "El documento no está registrado");
        }

        public async Task<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>> GetEmployeesByStateAsync(int stateId)
        {
            if (stateId <= 0)
                return ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse("El ID del estado debe ser mayor que 0", 400);

            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
                return ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse($"No existe un estado con ID {stateId}", 404);

            var employees = await _employeeInfoRepository.GetByStateAsync(stateId);
            var result = _mapper.Map<IEnumerable<EmployeeInfoDataTransferObject>>(employees);
            return ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.SuccessResponse(result, $"Se encontraron {result.Count()} empleado(s) con el estado {stateId}");
        }

        public async Task<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>> GetEmployeesByDocumentTypeAsync(int documentTypeId)
        {
            if (documentTypeId <= 0)
                return ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse("El ID del tipo de documento debe ser mayor que 0", 400);

            var documentTypeExists = await _documentTypeRepository.GetByIdAsync(documentTypeId);
            if (documentTypeExists == null)
                return ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse($"No existe un tipo de documento con ID {documentTypeId}", 404);

            var employees = await _employeeInfoRepository.GetByDocumentTypeAsync(documentTypeId);
            var result = _mapper.Map<IEnumerable<EmployeeInfoDataTransferObject>>(employees);
            return ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.SuccessResponse(result, $"Se encontraron {result.Count()} empleado(s) con el tipo de documento {documentTypeId}");
        }

        public async Task<ApiResponse<EmployeeInfoDataTransferObject>> CreateEmployeeAsync(CreateEmployeeInfoDataTransferObject createDto)
        {
            if (createDto == null)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("Los datos del empleado no pueden estar vacíos", 400);

            var stateExists = await _stateRepository.GetByIdAsync(createDto.StateId);
            if (stateExists == null)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"No existe un estado con ID {createDto.StateId}", 400);

            var documentTypeExists = await _documentTypeRepository.GetByIdAsync(createDto.DocumentTypeId);
            if (documentTypeExists == null)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"No existe un tipo de documento con ID {createDto.DocumentTypeId}", 400);

            var documentValidation = await _documentValidator.ValidateDocumentAsync(createDto.DocumentNumber, createDto.DocumentTypeId);
            if (!documentValidation.IsValid)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"Error en el formato del documento: {documentValidation.ErrorMessage}", 400);

            var existingEmployee = await _employeeInfoRepository.GetByDocumentNumberAsync(createDto.DocumentNumber, createDto.DocumentTypeId);
            if (existingEmployee != null)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"Ya existe un empleado con el número de documento '{createDto.DocumentNumber}' del tipo '{documentTypeExists.DocumentName}'", 409);

            if (createDto.DocumentExpire.HasValue && createDto.DocumentExpire.Value < DateOnly.FromDateTime(DateTime.Today))
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("La fecha de expiración del documento no puede ser anterior a hoy", 400);

            if (createDto.Birthday > DateOnly.FromDateTime(DateTime.Today))
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("La fecha de nacimiento no puede ser futura", 400);

            if (createDto.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-100)))
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("La fecha de nacimiento no puede ser anterior a 100 años", 400);

            if (createDto.CostPerHour.HasValue && createDto.CostPerHour.Value < 0)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("El costo por hora no puede ser negativo", 400);

            var employee = _mapper.Map<WorkTrackBio.API.Data.Models.EmployeeInfo>(createDto);
            employee.DocumentNumber = _phoneNumberFormatter.CleanDocumentNumber(employee.DocumentNumber) ?? employee.DocumentNumber;
            employee.PhoneNumber = _phoneNumberFormatter.FormatPhoneNumber(employee.PhoneNumber);
            employee.EmergencyContactPhoneNumber = _phoneNumberFormatter.FormatPhoneNumber(employee.EmergencyContactPhoneNumber);

            var createdEmployee = await _employeeInfoRepository.CreateAsync(employee);
            var result = _mapper.Map<EmployeeInfoDataTransferObject>(createdEmployee);
            return ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(result, "Empleado creado exitosamente", 201);
        }

        public async Task<ApiResponse<EmployeeInfoDataTransferObject>> UpdateEmployeeAsync(UpdateEmployeeInfoDataTransferObject updateDto)
        {
            if (updateDto == null)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("Los datos del empleado no pueden estar vacíos", 400);

            if (updateDto.Id <= 0)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var existingEmployee = await _employeeInfoRepository.GetByIdAsync(updateDto.Id);
            if (existingEmployee == null)
                return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"No se encontró un empleado con ID {updateDto.Id}", 404);

            bool hasChanges = false;

            var sentFields = new HashSet<string>();
            var updateDtoType = typeof(UpdateEmployeeInfoDataTransferObject);
            foreach (var property in updateDtoType.GetProperties())
            {
                if (property.Name != "Id" && property.GetValue(updateDto) != null)
                    sentFields.Add(property.Name);
            }

            if (sentFields.Contains("DocumentNumber"))
            {
                var documentTypeIdForValidation = updateDto.DocumentTypeId ?? existingEmployee.DocumentTypeId;
                var documentValidation = await _documentValidator.ValidateDocumentAsync(updateDto.DocumentNumber!, documentTypeIdForValidation);
                if (!documentValidation.IsValid)
                    return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"Error en el formato del documento: {documentValidation.ErrorMessage}", 400);

                if (updateDto.DocumentTypeId.HasValue)
                {
                    var otherEmployee = await _employeeInfoRepository.GetByDocumentNumberAsync(updateDto.DocumentNumber!, updateDto.DocumentTypeId.Value);
                    if (otherEmployee != null && otherEmployee.Id != updateDto.Id)
                        return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"Ya existe otro empleado con el número de documento '{updateDto.DocumentNumber}' del tipo especificado", 409);
                }
                else
                {
                    var otherEmployee = await _employeeInfoRepository.GetByDocumentNumberAsync(updateDto.DocumentNumber!, existingEmployee.DocumentTypeId);
                    if (otherEmployee != null && otherEmployee.Id != updateDto.Id)
                        return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"Ya existe otro empleado con el número de documento '{updateDto.DocumentNumber}' del mismo tipo", 409);
                }

                var cleanedDocumentNumber = _phoneNumberFormatter.CleanDocumentNumber(updateDto.DocumentNumber!);
                if (!string.Equals(existingEmployee.DocumentNumber, cleanedDocumentNumber, StringComparison.OrdinalIgnoreCase))
                {
                    existingEmployee.DocumentNumber = cleanedDocumentNumber ?? updateDto.DocumentNumber!;
                    hasChanges = true;
                }
            }

            if (sentFields.Contains("DocumentTypeId") && updateDto.DocumentTypeId.HasValue)
            {
                var documentTypeExists = await _documentTypeRepository.GetByIdAsync(updateDto.DocumentTypeId.Value);
                if (documentTypeExists == null)
                    return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"No existe un tipo de documento con ID {updateDto.DocumentTypeId.Value}", 400);

                if (existingEmployee.DocumentTypeId != updateDto.DocumentTypeId.Value)
                {
                    if (sentFields.Contains("DocumentNumber") && updateDto.DocumentNumber != null)
                    {
                        var otherEmployee = await _employeeInfoRepository.GetByDocumentNumberAsync(updateDto.DocumentNumber, updateDto.DocumentTypeId.Value);
                        if (otherEmployee != null && otherEmployee.Id != updateDto.Id)
                            return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"Ya existe otro empleado con el número de documento '{updateDto.DocumentNumber}' del tipo '{documentTypeExists.DocumentName}'", 409);
                    }

                    existingEmployee.DocumentTypeId = updateDto.DocumentTypeId.Value;
                    hasChanges = true;
                }
            }

            if (sentFields.Contains("DocumentExpire") && updateDto.DocumentExpire.HasValue)
            {
                if (updateDto.DocumentExpire.Value < DateOnly.FromDateTime(DateTime.Today))
                    return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("La fecha de expiración del documento no puede ser anterior a hoy", 400);

                if (existingEmployee.DocumentExpire != updateDto.DocumentExpire.Value)
                {
                    existingEmployee.DocumentExpire = updateDto.DocumentExpire.Value;
                    hasChanges = true;
                }
            }

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

            if (sentFields.Contains("PhoneNumber"))
            {
                if (updateDto.PhoneNumber == null)
                {
                    existingEmployee.PhoneNumber = null;
                    hasChanges = true;
                }
                else
                {
                    var formattedPhoneNumber = _phoneNumberFormatter.FormatPhoneNumber(updateDto.PhoneNumber);
                    if (existingEmployee.PhoneNumber != formattedPhoneNumber)
                    {
                        existingEmployee.PhoneNumber = formattedPhoneNumber;
                        hasChanges = true;
                    }
                }
            }

            if (sentFields.Contains("EmergencyContact"))
            {
                existingEmployee.EmergencyContact = updateDto.EmergencyContact;
                hasChanges = true;
            }

            if (sentFields.Contains("EmergencyContactPhoneNumber"))
            {
                if (updateDto.EmergencyContactPhoneNumber == null)
                {
                    existingEmployee.EmergencyContactPhoneNumber = null;
                    hasChanges = true;
                }
                else
                {
                    var formattedEmergencyPhone = _phoneNumberFormatter.FormatPhoneNumber(updateDto.EmergencyContactPhoneNumber);
                    if (existingEmployee.EmergencyContactPhoneNumber != formattedEmergencyPhone)
                    {
                        existingEmployee.EmergencyContactPhoneNumber = formattedEmergencyPhone;
                        hasChanges = true;
                    }
                }
            }

            if (sentFields.Contains("Birthday") && updateDto.Birthday.HasValue)
            {
                if (updateDto.Birthday.Value > DateOnly.FromDateTime(DateTime.Today))
                    return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("La fecha de nacimiento no puede ser futura", 400);

                if (updateDto.Birthday.Value < DateOnly.FromDateTime(DateTime.Today.AddYears(-100)))
                    return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("La fecha de nacimiento no puede ser anterior a 100 años", 400);

                if (existingEmployee.Birthday != updateDto.Birthday.Value)
                {
                    existingEmployee.Birthday = updateDto.Birthday.Value;
                    hasChanges = true;
                }
            }

            if (sentFields.Contains("CostPerHour") && updateDto.CostPerHour.HasValue)
            {
                if (updateDto.CostPerHour.Value < 0)
                    return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("El costo por hora no puede ser negativo", 400);

                if (existingEmployee.CostPerHour != updateDto.CostPerHour.Value)
                {
                    existingEmployee.CostPerHour = updateDto.CostPerHour.Value;
                    hasChanges = true;
                }
            }

            if (sentFields.Contains("StateId") && updateDto.StateId.HasValue)
            {
                var stateExists = await _stateRepository.GetByIdAsync(updateDto.StateId.Value);
                if (stateExists == null)
                    return ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse($"No existe un estado con ID {updateDto.StateId.Value}", 400);

                if (existingEmployee.StateId != updateDto.StateId.Value)
                {
                    existingEmployee.StateId = updateDto.StateId.Value;
                    hasChanges = true;
                }
            }

            if (sentFields.Contains("Address"))
            {
                existingEmployee.Address = updateDto.Address;
                hasChanges = true;
            }

            if (sentFields.Contains("IBAN"))
            {
                existingEmployee.IBAN = updateDto.IBAN;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                var unchanged = _mapper.Map<EmployeeInfoDataTransferObject>(existingEmployee);
                return ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(unchanged, "No se detectaron cambios");
            }

            var updatedEmployee = await _employeeInfoRepository.UpdateAsync(existingEmployee);
            var updatedResult = _mapper.Map<EmployeeInfoDataTransferObject>(updatedEmployee);
            return ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(updatedResult, "Empleado actualizado exitosamente");
        }

        public async Task<ApiResponse<bool>> DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var employee = await _employeeInfoRepository.GetByIdAsync(id);
            if (employee == null)
                return ApiResponse<bool>.ErrorResponse($"No se encontró un empleado con ID {id}", 404);

            var isInUse = await _employeeInfoRepository.HasDependenciesAsync(id);
            if (isInUse)
                return ApiResponse<bool>.ErrorResponse($"No se puede eliminar el empleado '{employee.FirstName} {employee.LastName}' porque está siendo usado por otras entidades del sistema", 409);

            await _employeeInfoRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true, "Empleado eliminado exitosamente");
        }
    }
}
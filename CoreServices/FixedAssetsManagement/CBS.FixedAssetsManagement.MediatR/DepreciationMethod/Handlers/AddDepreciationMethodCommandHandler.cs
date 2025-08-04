using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Repository;
using AutoMapper;
using CBS.FixedAssetsManagement.MediatR.Commands;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new depreciation method based on AddDepreciationMethodCommand.
    /// </summary>
    public class AddDepreciationMethodCommandHandler : IRequestHandler<AddDepreciationMethodCommand, ServiceResponse<DepreciationMethodDto>>
    {
        private readonly IDepreciationMethodRepository _depreciationMethodRepository;
        private readonly ILogger<AddDepreciationMethodCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddDepreciationMethodCommandHandler(
            IDepreciationMethodRepository depreciationMethodRepository,
            ILogger<AddDepreciationMethodCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _depreciationMethodRepository = depreciationMethodRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<DepreciationMethodDto>> Handle(AddDepreciationMethodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a depreciation method with the same name already exists
                var existingMethod = await _depreciationMethodRepository.FindBy(x => x.MethodName == request.MethodName).FirstOrDefaultAsync();
                if (existingMethod != null)
                {
                    string message = $"Depreciation method '{request.MethodName}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<DepreciationMethodDto>.Return409(message);
                }

                // Create the depreciation method entity
                var depreciationMethod = new DepreciationMethod
                {
                    MethodName = request.MethodName,
                    Description = request.Description,
                    Id= BaseUtilities.GenerateInsuranceUniqueNumber(8,"DPI")
                };

                // Add the depreciation method to the repository
                _depreciationMethodRepository.Add(depreciationMethod);
                await _uow.SaveAsync();

                // Log the successful creation
                string successMessage = $"Depreciation method '{request.MethodName}' created successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the depreciation method entity to a DTO for response
                var depreciationMethodDto = _mapper.Map<DepreciationMethodDto>(depreciationMethod);
                return ServiceResponse<DepreciationMethodDto>.ReturnResultWith200(depreciationMethodDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response
                string errorMessage = $"Error occurred while adding depreciation method: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<DepreciationMethodDto>.Return500(errorMessage);
            }
        }
    }
}

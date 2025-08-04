using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.BudgetManagement.Common;
 
using CBS.BudgetManagement.Repository;
using AutoMapper;
using CBS.BudgetManagement.MediatR.Commands;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new SpendingLimit based on AddSpendingLimitCommand.
    /// </summary>
    public class AddSpendingLimitCommandHandler : IRequestHandler<AddSpendingLimitCommand, ServiceResponse<SpendingLimitDto>>
    {
        private readonly ISpendingLimitRepository _SpendingLimitRepository;
 
        private readonly ILogger<AddSpendingLimitCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddSpendingLimitCommandHandler(
            ISpendingLimitRepository SpendingLimitRepository,
 
            ILogger<AddSpendingLimitCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _SpendingLimitRepository = SpendingLimitRepository;
 
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<SpendingLimitDto>> Handle(AddSpendingLimitCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an SpendingLimit with the same serial number already exists
                var existingSpendingLimit = await _SpendingLimitRepository.FindBy(x => x.FiscalYearId == request.FiscalYearId).FirstOrDefaultAsync();
                string message = $"SpendingLimit '{request.FiscalYearId}' created successfully.";

                if (existingSpendingLimit != null)
                {
                    message = $"SpendingLimit with '{request.FiscalYearId}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<SpendingLimitDto>.Return409(message);
                }
 

                // Map the request to an SpendingLimit entity
                var SpendingLimit = _mapper.Map<SpendingLimit>(request);
                SpendingLimit.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "BDT");


                // Add the new SpendingLimit to the repository
                _SpendingLimitRepository.Add(SpendingLimit);
                await _uow.SaveAsync();

                // Log successful creation of the SpendingLimit
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the SpendingLimit entity back to a DTO for response
                var SpendingLimitDto = _mapper.Map<SpendingLimitDto>(SpendingLimit);
                return ServiceResponse<SpendingLimitDto>.ReturnResultWith200(SpendingLimitDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding SpendingLimit: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<SpendingLimitDto>.Return500(msg);
            }
        }
    }
}

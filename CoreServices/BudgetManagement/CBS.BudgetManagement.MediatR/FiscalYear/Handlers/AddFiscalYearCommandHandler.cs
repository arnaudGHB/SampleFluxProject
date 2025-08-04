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
    /// Handles the command to add a new FiscalYear based on AddFiscalYearCommand.
    /// </summary>
    public class AddFiscalYearCommandHandler : IRequestHandler<AddFiscalYearCommand, ServiceResponse<FiscalYearDto>>
    {
        private readonly IFiscalYearRepository _FiscalYearRepository;
 
        private readonly ILogger<AddFiscalYearCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddFiscalYearCommandHandler(
            IFiscalYearRepository FiscalYearRepository,
 
            ILogger<AddFiscalYearCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _FiscalYearRepository = FiscalYearRepository;
 
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<FiscalYearDto>> Handle(AddFiscalYearCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an FiscalYear with the same serial number already exists
                var existingFiscalYear = await _FiscalYearRepository.FindBy(x => x.EndDate == request.EndDate&&x.StartDate==request.StartDate).FirstOrDefaultAsync();
                string message = $"FiscalYear '{request.EndDate.Date}' created successfully.";

                if (existingFiscalYear != null)
                {
                    message = $"FiscalYear with '{request.EndDate.Date}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<FiscalYearDto>.Return409(message);
                }
 

                // Map the request to an FiscalYear entity
                var FiscalYear = _mapper.Map<FiscalYear>(request);
                FiscalYear.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "BDT");


                // Add the new FiscalYear to the repository
                _FiscalYearRepository.Add(FiscalYear);
                await _uow.SaveAsync();

                // Log successful creation of the FiscalYear
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the FiscalYear entity back to a DTO for response
                var FiscalYearDto = _mapper.Map<FiscalYearDto>(FiscalYear);
                return ServiceResponse<FiscalYearDto>.ReturnResultWith200(FiscalYearDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding FiscalYear: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<FiscalYearDto>.Return500(msg);
            }
        }
    }
}

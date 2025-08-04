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
    /// Handles the command to add a new Expenditure based on AddExpenditureCommand.
    /// </summary>
    public class AddExpenditureCommandHandler : IRequestHandler<AddExpenditureCommand, ServiceResponse<ExpenditureDto>>
    {
        private readonly IExpenditureRepository _ExpenditureRepository;
 
        private readonly ILogger<AddExpenditureCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddExpenditureCommandHandler(
            IExpenditureRepository ExpenditureRepository,
 
            ILogger<AddExpenditureCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _ExpenditureRepository = ExpenditureRepository;
 
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<ExpenditureDto>> Handle(AddExpenditureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an Expenditure with the same serial number already exists
                var existingExpenditure = await _ExpenditureRepository.FindBy(x => x.Description == request.Description).FirstOrDefaultAsync();
                string message = $"Expenditure '{request.Description}' created successfully.";

                if (existingExpenditure != null)
                {
                    message = $"Expenditure with '{request.Description}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<ExpenditureDto>.Return409(message);
                }
 

                // Map the request to an Expenditure entity
                var Expenditure = _mapper.Map<Expenditure>(request);
                Expenditure.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "BDT");


                // Add the new Expenditure to the repository
                _ExpenditureRepository.Add(Expenditure);
                await _uow.SaveAsync();

                // Log successful creation of the Expenditure
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the Expenditure entity back to a DTO for response
                var ExpenditureDto = _mapper.Map<ExpenditureDto>(Expenditure);
                return ServiceResponse<ExpenditureDto>.ReturnResultWith200(ExpenditureDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding Expenditure: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<ExpenditureDto>.Return500(msg);
            }
        }
    }
}

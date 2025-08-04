using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new BudgetName.
    /// </summary>
    public class AddBudgetCommandHandler : IRequestHandler<AddBudgetCommand, ServiceResponse<BudgetDto>>
    {
        private readonly IBudgetRepository _BudgetNameRepository; // Repository for accessing BudgetName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBudgetCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddBudgetNameCommandHandler.
        /// </summary>
        /// <param name="BudgetNameRepository">Repository for BudgetName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddBudgetCommandHandler(
            IBudgetRepository BudgetNameRepository,
            IMapper mapper,
            ILogger<AddBudgetCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _BudgetNameRepository = BudgetNameRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken= userInfoToken;
        }

        /// <summary>
        /// Handles the AddBudgetNameCommand to add a new BudgetName.
        /// </summary>
        /// <param name="request">The AddBudgetNameCommand containing BudgetName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetDto>> Handle(AddBudgetCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = "";
             try
            {
                // Check if a BudgetName with the same name already exists (case-insensitive)
                var existingBudgetName =   _BudgetNameRepository.All.Where(c => c.Year == request.Year && c.Year == request.Year&&c.IsDeleted==false&&c.BranchId==_userInfoToken.BranchId);

                // If a BudgetName with the same name already exists, return a conflict response
                if (existingBudgetName.Any())
                {
                      errorMessage = $"Budget of {request.Year}{request.BudgetPeriodId} already exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetCommand",
                      request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
       
                    return ServiceResponse<BudgetDto>.Return409(errorMessage);
                }

                // Map the AddBudgetNameAttributesCommand to a BudgetNameAttributes entity
                var BudgetNameAttributesEntity = _mapper.Map<Data.Budget>(request);
                BudgetNameAttributesEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "BGT");
                _BudgetNameRepository.Add(BudgetNameAttributesEntity);
                await _uow.SaveAsync();
                errorMessage = $"Budget of {request.Year}{request.BudgetPeriodId} was successfully created.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetCommand",
                        request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);


                // Map the BudgetName entity to BudgetNameDto and return it with a success response
                var BudgetNameDto = _mapper.Map<BudgetDto>(BudgetNameAttributesEntity);
                return ServiceResponse<BudgetDto>.ReturnResultWith200(BudgetNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while creating  Budget  {request.Year}{request.BudgetPeriodId}: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetCommand",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);


                return ServiceResponse<BudgetDto>.Return500(e);
            }
        }
    }
}
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
    public class AddTrailBalanceUploudCommandHandler : IRequestHandler<AddTrailBalanceUploudCommand, ServiceResponse<TrailBalanceUploudDto>>
    {
        private readonly ITrailBalanceUploudRepository _TrailBalanceUploudRepository; // Repository for accessing BudgetName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTrailBalanceUploudCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddBudgetNameCommandHandler.
        /// </summary>
        /// <param name="BudgetNameRepository">Repository for BudgetName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddTrailBalanceUploudCommandHandler(
            ITrailBalanceUploudRepository BudgetNameRepository,
            IMapper mapper,
            ILogger<AddTrailBalanceUploudCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _TrailBalanceUploudRepository = BudgetNameRepository;
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
        public async Task<ServiceResponse<TrailBalanceUploudDto>> Handle(AddTrailBalanceUploudCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = "";
             try
            {
                // Check if a BudgetName with the same name already exists (case-insensitive)
              
                // Map the AddBudgetNameAttributesCommand to a BudgetNameAttributes entity
                var TrailBalanceUploudEntity = _mapper.Map<TrailBalanceUploud>(request);
                TrailBalanceUploudEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "TB");
                _TrailBalanceUploudRepository.Add(TrailBalanceUploudEntity);
                await _uow.SaveAsync();
                errorMessage = $"TrailBalance was uploaded was successfully created.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddTrailBalanceUploudCommand",
                        request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);


                // Map the BudgetName entity to BudgetNameDto and return it with a success response
                var BudgetNameDto = _mapper.Map<TrailBalanceUploudDto>(TrailBalanceUploudEntity);
                return ServiceResponse<TrailBalanceUploudDto>.ReturnResultWith200(BudgetNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while creating   : {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetCommand",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);


                return ServiceResponse<TrailBalanceUploudDto>.Return500(e);
            }
        }
    }
}
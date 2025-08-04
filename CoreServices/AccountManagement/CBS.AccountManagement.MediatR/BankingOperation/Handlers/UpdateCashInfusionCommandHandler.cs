using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountCategory.
    /// </summary>
    public class UpdateCashInfusionCommandHandler : IRequestHandler<UpdateCashInfusionCommand, ServiceResponse<bool>>
    {
        private readonly ICashReplenishmentRepository _cashReplenishmentRepository; // Repository for accessing AccountCategory data.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateCashInfusionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCashInfusionCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public UpdateCashInfusionCommandHandler(ICashReplenishmentRepository cashReplenishmentRepository, IAccountClassRepository AccountClassRepository,
            ILogger<UpdateCashInfusionCommandHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper, IUnitOfWork<POSContext> work)
        {
            _cashReplenishmentRepository = cashReplenishmentRepository;

            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = work;
        }

        /// <summary>
        /// Handles the AddAccountClassCommand to add a new AccountClass.
        /// </summary>
        /// <param name="request">The AddAccountClassCommand containing AccountClass data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(UpdateCashInfusionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var Entity =await _cashReplenishmentRepository.FindAsync(request.Id);
                if (Entity==null)
                {

                    throw new Exception($"{Entity.Id} does not exist in the current context");
                }
                else
                {
                    if (Entity.IsDeleted == true)
                    {
                        throw new Exception($"{Entity.Id} is already deleted");
                    }
                    else
                    {
                        Entity.AmountRequested = request.Amount == 0 ? Entity.AmountRequested : request.Amount;
                        Entity.RequestMessage = request.RequestMessage;
                    }
                  

                }

                _cashReplenishmentRepository.Update(Entity);
                await _uow.SaveAsync();

                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $" {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e + errorMessage);
            }
        }
    }
}
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.ChargesWaivedP;
using CBS.TransactionManagement.Commands.ChargesWaivedP;
using CBS.TransactionManagement.Data.ChargesWaivedP;
using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;

namespace CBS.TransactionManagement.Handlers.ChargesWaivedP
{
    /// <summary>
    /// Handles the command to add a new ChargesWaived.
    /// </summary>
    public class AddChargesWaivedCommandHandler : IRequestHandler<AddChargesWaivedCommand, ServiceResponse<ChargesWaivedDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IChargesWaivedRepository _ChargesWaivedRepository; // Repository for accessing ChargesWaived data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddChargesWaivedCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddChargesWaivedCommandHandler.
        /// </summary>
        /// <param name="ChargesWaivedRepository">Repository for ChargesWaived data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddChargesWaivedCommandHandler(
            IChargesWaivedRepository ChargesWaivedRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddChargesWaivedCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _ChargesWaivedRepository = ChargesWaivedRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddChargesWaivedCommand to add a new ChargesWaived.
        /// </summary>
        /// <param name="request">The AddChargesWaivedCommand containing ChargesWaived data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<ChargesWaivedDto>> Handle(AddChargesWaivedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if there is any pending charge waiver for the customer
                await CheckChargeWaive(request.CustomerId);

                // Map the request to a ChargesWaived entity
                var ChargesWaivedEntity = MapToChargesWaivedEntity(request);

                // Add the ChargesWaived entity to the repository and save changes
                await AddChargesWaivedAsync(ChargesWaivedEntity);

                // Map the ChargesWaived entity to a DTO
                var ChargesWaivedDto = _mapper.Map<ChargesWaivedDto>(ChargesWaivedEntity);

                // Return a successful service response with the DTO and a message
                string message = "Operation was successful.";
                return ServiceResponse<ChargesWaivedDto>.ReturnResultWith200(ChargesWaivedDto, message);
            }
            catch (Exception e)
            {
                // Handle and log the error, then return a 500 Internal Server Error response
                return HandleErrorSavingChargesWaived(e);
            }
        }

        // Maps the AddChargesWaivedCommand request to a ChargesWaived entity
        private ChargesWaived MapToChargesWaivedEntity(AddChargesWaivedCommand request)
        {
            var ChargesWaivedEntity = _mapper.Map<ChargesWaived>(request);
            ChargesWaivedEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            ChargesWaivedEntity.Id = BaseUtilities.GenerateUniqueNumber();
            ChargesWaivedEntity.DateOfWaiverRequest = BaseUtilities.UtcNowToDoualaTime();
            ChargesWaivedEntity.TransactionReference = BaseUtilities.GenerateUniqueNumber();
            ChargesWaivedEntity.WaiverInitiator = _userInfoToken.FullName;
            return ChargesWaivedEntity;
        }

        // Adds the ChargesWaived entity to the repository and saves changes
        private async Task AddChargesWaivedAsync(ChargesWaived ChargesWaivedEntity)
        {
            _ChargesWaivedRepository.Add(ChargesWaivedEntity);
            await _uow.SaveAsync();
        }

        // Handles errors that occur while saving ChargesWaived and logs them
        private ServiceResponse<ChargesWaivedDto> HandleErrorSavingChargesWaived(Exception e)
        {
            var errorMessage = $"Error occurred while saving ChargesWaived: {e.Message}";
            _logger.LogError(errorMessage);
            return ServiceResponse<ChargesWaivedDto>.Return500(e);
        }

        // Checks if there is any pending charge waiver for the customer
        private async Task<bool> CheckChargeWaive(string customerId)
        {
            var waived = await _ChargesWaivedRepository
                .FindBy(x => x.CustomerId == customerId && !x.IsWaiverDone && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (waived != null)
            {
                if (waived.DateOfWaiverRequest < BaseUtilities.UtcNowToDoualaTime())
                {
                    return true;
                }
                var errorMessage = "Member already has a pending charge waiver.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            return true;
        }





    }

}

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository.ChargesWaivedP;
using CBS.TransactionManagement.Commands.ChargesWaivedP;
using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;

namespace CBS.TransactionManagement.Handlers.ChargesWaivedP
{

    /// <summary>
    /// Handles the command to update a ChargesWaived based on UpdateChargesWaivedCommand.
    /// </summary>
    public class UpdateChargesWaivedCommandHandler : IRequestHandler<UpdateChargesWaivedCommand, ServiceResponse<ChargesWaivedDto>>
    {
        private readonly IChargesWaivedRepository _ChargesWaivedRepository; // Repository for accessing ChargesWaived data.
        private readonly ILogger<UpdateChargesWaivedCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateChargesWaivedCommandHandler.
        /// </summary>
        /// <param name="ChargesWaivedRepository">Repository for ChargesWaived data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateChargesWaivedCommandHandler(
            IChargesWaivedRepository ChargesWaivedRepository,
            ILogger<UpdateChargesWaivedCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _ChargesWaivedRepository = ChargesWaivedRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateChargesWaivedCommand to update a ChargesWaived.
        /// </summary>
        /// <param name="request">The UpdateChargesWaivedCommand containing updated ChargesWaived data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ChargesWaivedDto>> Handle(UpdateChargesWaivedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingChargesWaived = await _ChargesWaivedRepository.FindAsync(request.Id);

                if (existingChargesWaived == null)
                {
                    return ServiceResponse<ChargesWaivedDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingChargesWaived
                _mapper.Map(request, existingChargesWaived);

                // Set the modified date
                existingChargesWaived.ModifiedDate = DateTime.Now;

                // Use the repository to update the existing ChargesWaived entity
                _ChargesWaivedRepository.Update(existingChargesWaived);

                // Save changes
                await _uow.SaveAsync();

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<ChargesWaivedDto>.ReturnResultWith200(_mapper.Map<ChargesWaivedDto>(existingChargesWaived));
                _logger.LogInformation($"ChargesWaived {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating ChargesWaived: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<ChargesWaivedDto>.Return500(e);
            }
        }
    }

}

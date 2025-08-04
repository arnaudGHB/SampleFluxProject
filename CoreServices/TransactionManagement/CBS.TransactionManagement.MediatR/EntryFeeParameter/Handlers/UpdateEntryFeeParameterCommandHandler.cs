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

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a EntryFeeParameter based on UpdateEntryFeeParameterCommand.
    /// </summary>
    public class UpdateEntryFeeParameterCommandHandler : IRequestHandler<UpdateEntryFeeParameterCommand, ServiceResponse<EntryFeeParameterDto>>
    {
        private readonly IEntryFeeParameterRepository _EntryFeeParameterRepository; // Repository for accessing EntryFeeParameter data.
        private readonly ILogger<UpdateEntryFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateEntryFeeParameterCommandHandler.
        /// </summary>
        /// <param name="EntryFeeParameterRepository">Repository for EntryFeeParameter data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateEntryFeeParameterCommandHandler(
            IEntryFeeParameterRepository EntryFeeParameterRepository,
            ILogger<UpdateEntryFeeParameterCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _EntryFeeParameterRepository = EntryFeeParameterRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateEntryFeeParameterCommand to update a EntryFeeParameter.
        /// </summary>
        /// <param name="request">The UpdateEntryFeeParameterCommand containing updated EntryFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EntryFeeParameterDto>> Handle(UpdateEntryFeeParameterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingEntryFeeParameter = await _EntryFeeParameterRepository.FindAsync(request.Id);

                if (existingEntryFeeParameter == null)
                {
                    return ServiceResponse<EntryFeeParameterDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingEntryFeeParameter
                _mapper.Map(request, existingEntryFeeParameter);

                // Use the repository to update the existing EntryFeeParameter entity
                _EntryFeeParameterRepository.Update(existingEntryFeeParameter);

                // Save changes
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<EntryFeeParameterDto>.Return500();
                }

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<EntryFeeParameterDto>.ReturnResultWith200(_mapper.Map<EntryFeeParameterDto>(existingEntryFeeParameter));
                _logger.LogInformation($"EntryFeeParameter {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating EntryFeeParameter: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<EntryFeeParameterDto>.Return500(e);
            }
        }
    }

}

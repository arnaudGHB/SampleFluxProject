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
using CBS.TransactionManagement.Repository.ReopenFeeParameterP;

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a ReopenFeeParameter based on UpdateReopenFeeParameterCommand.
    /// </summary>
    public class UpdateReopenFeeParameterCommandHandler : IRequestHandler<UpdateReopenFeeParameterCommand, ServiceResponse<ReopenFeeParameterDto>>
    {
        private readonly IReopenFeeParameterRepository _ReopenFeeParameterRepository; // Repository for accessing ReopenFeeParameter data.
        private readonly ILogger<UpdateReopenFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateReopenFeeParameterCommandHandler.
        /// </summary>
        /// <param name="ReopenFeeParameterRepository">Repository for ReopenFeeParameter data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateReopenFeeParameterCommandHandler(
            IReopenFeeParameterRepository ReopenFeeParameterRepository,
            ILogger<UpdateReopenFeeParameterCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _ReopenFeeParameterRepository = ReopenFeeParameterRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateReopenFeeParameterCommand to update a ReopenFeeParameter.
        /// </summary>
        /// <param name="request">The UpdateReopenFeeParameterCommand containing updated ReopenFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ReopenFeeParameterDto>> Handle(UpdateReopenFeeParameterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingReopenFeeParameter = await _ReopenFeeParameterRepository.FindAsync(request.Id);

                if (existingReopenFeeParameter == null)
                {
                    return ServiceResponse<ReopenFeeParameterDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingReopenFeeParameter
                _mapper.Map(request, existingReopenFeeParameter);

                // Use the repository to update the existing ReopenFeeParameter entity
                _ReopenFeeParameterRepository.Update(existingReopenFeeParameter);

                // Save changes
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<ReopenFeeParameterDto>.Return500();
                }

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<ReopenFeeParameterDto>.ReturnResultWith200(_mapper.Map<ReopenFeeParameterDto>(existingReopenFeeParameter));
                _logger.LogInformation($"ReopenFeeParameter {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating ReopenFeeParameter: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<ReopenFeeParameterDto>.Return500(e);
            }
        }
    }

}

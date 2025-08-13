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
    /// Handles the command to update a ManagementFeeParameter based on UpdateManagementFeeParameterCommand.
    /// </summary>
    public class UpdateManagementFeeParameterCommandHandler : IRequestHandler<UpdateManagementFeeParameterCommand, ServiceResponse<ManagementFeeParameterDto>>
    {
        private readonly IManagementFeeParameterRepository _ManagementFeeParameterRepository; // Repository for accessing ManagementFeeParameter data.
        private readonly ILogger<UpdateManagementFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateManagementFeeParameterCommandHandler.
        /// </summary>
        /// <param name="ManagementFeeParameterRepository">Repository for ManagementFeeParameter data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateManagementFeeParameterCommandHandler(
            IManagementFeeParameterRepository ManagementFeeParameterRepository,
            ILogger<UpdateManagementFeeParameterCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _ManagementFeeParameterRepository = ManagementFeeParameterRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateManagementFeeParameterCommand to update a ManagementFeeParameter.
        /// </summary>
        /// <param name="request">The UpdateManagementFeeParameterCommand containing updated ManagementFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ManagementFeeParameterDto>> Handle(UpdateManagementFeeParameterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingManagementFeeParameter = await _ManagementFeeParameterRepository.FindAsync(request.Id);

                if (existingManagementFeeParameter == null)
                {
                    return ServiceResponse<ManagementFeeParameterDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingManagementFeeParameter
                _mapper.Map(request, existingManagementFeeParameter);

                // Use the repository to update the existing ManagementFeeParameter entity
                _ManagementFeeParameterRepository.Update(existingManagementFeeParameter);

                // Save changes
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<ManagementFeeParameterDto>.Return500();
                }

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<ManagementFeeParameterDto>.ReturnResultWith200(_mapper.Map<ManagementFeeParameterDto>(existingManagementFeeParameter));
                _logger.LogInformation($"ManagementFeeParameter {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating ManagementFeeParameter: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<ManagementFeeParameterDto>.Return500(e);
            }
        }
    }

}

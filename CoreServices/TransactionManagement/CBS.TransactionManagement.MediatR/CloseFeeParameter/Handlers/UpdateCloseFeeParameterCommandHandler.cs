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
    /// Handles the command to update a CloseFeeParameter based on UpdateCloseFeeParameterCommand.
    /// </summary>
    public class UpdateCloseFeeParameterCommandHandler : IRequestHandler<UpdateCloseFeeParameterCommand, ServiceResponse<CloseFeeParameterDto>>
    {
        private readonly ICloseFeeParameterRepository _CloseFeeParameterRepository; // Repository for accessing CloseFeeParameter data.
        private readonly ILogger<UpdateCloseFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCloseFeeParameterCommandHandler.
        /// </summary>
        /// <param name="CloseFeeParameterRepository">Repository for CloseFeeParameter data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCloseFeeParameterCommandHandler(
            ICloseFeeParameterRepository CloseFeeParameterRepository,
            ILogger<UpdateCloseFeeParameterCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _CloseFeeParameterRepository = CloseFeeParameterRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateCloseFeeParameterCommand to update a CloseFeeParameter.
        /// </summary>
        /// <param name="request">The UpdateCloseFeeParameterCommand containing updated CloseFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CloseFeeParameterDto>> Handle(UpdateCloseFeeParameterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingCloseFeeParameter = await _CloseFeeParameterRepository.FindAsync(request.Id);

                if (existingCloseFeeParameter == null)
                {
                    return ServiceResponse<CloseFeeParameterDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingCloseFeeParameter
                _mapper.Map(request, existingCloseFeeParameter);

                // Use the repository to update the existing CloseFeeParameter entity
                _CloseFeeParameterRepository.Update(existingCloseFeeParameter);

                // Save changes
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CloseFeeParameterDto>.Return500();
                }

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<CloseFeeParameterDto>.ReturnResultWith200(_mapper.Map<CloseFeeParameterDto>(existingCloseFeeParameter));
                _logger.LogInformation($"CloseFeeParameter {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating CloseFeeParameter: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CloseFeeParameterDto>.Return500(e);
            }
        }
    }

}

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

namespace CBS.TransactionManagement.otherCashIn.Handlers
{

    /// <summary>
    /// Handles the command to update a DepositLimit based on UpdateDepositLimitCommand.
    /// </summary>
    public class UpdateOtherTransactionCommandHandler : IRequestHandler<UpdateDepositLimitCommand, ServiceResponse<CashDepositParameterDto>>
    {
        private readonly IDepositLimitRepository _DepositLimitRepository; // Repository for accessing DepositLimit data.
        private readonly ILogger<UpdateOtherTransactionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateDepositLimitCommandHandler.
        /// </summary>
        /// <param name="DepositLimitRepository">Repository for DepositLimit data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateOtherTransactionCommandHandler(
            IDepositLimitRepository DepositLimitRepository,
            ILogger<UpdateOtherTransactionCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _DepositLimitRepository = DepositLimitRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDepositLimitCommand to update a DepositLimit.
        /// </summary>
        /// <param name="request">The UpdateDepositLimitCommand containing updated DepositLimit data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CashDepositParameterDto>> Handle(UpdateDepositLimitCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingDepositLimit = await _DepositLimitRepository.FindAsync(request.Id);

                if (existingDepositLimit == null)
                {
                    return ServiceResponse<CashDepositParameterDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingDepositLimit
                _mapper.Map(request, existingDepositLimit);

                // Use the repository to update the existing DepositLimit entity
                _DepositLimitRepository.Update(existingDepositLimit);

                // Save changes
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CashDepositParameterDto>.Return500();
                }

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<CashDepositParameterDto>.ReturnResultWith200(_mapper.Map<CashDepositParameterDto>(existingDepositLimit));
                _logger.LogInformation($"DepositLimit {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating DepositLimit: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CashDepositParameterDto>.Return500(e);
            }
        }
    }

}

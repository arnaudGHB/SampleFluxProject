using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Currency based on DeleteCurrencyCommand.
    /// </summary>
    public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, ServiceResponse<bool>>
    {
        private readonly ICurrencyRepository _CurrencyRepository; // Repository for accessing Currency data.
        private readonly ILogger<DeleteCurrencyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteCurrencyCommandHandler.
        /// </summary>
        /// <param name="CurrencyRepository">Repository for Currency data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCurrencyCommandHandler(
            ICurrencyRepository CurrencyRepository, IMapper mapper,
            ILogger<DeleteCurrencyCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _CurrencyRepository = CurrencyRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCurrencyCommand to delete a Currency.
        /// </summary>
        /// <param name="request">The DeleteCurrencyCommand containing Currency ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Currency entity with the specified ID exists
                var existingCurrency = await _CurrencyRepository.FindAsync(request.Id);
                if (existingCurrency == null)
                {
                    errorMessage = $"Currency with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingCurrency.IsDeleted = true;
                _CurrencyRepository.Update(existingCurrency);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Currency: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}

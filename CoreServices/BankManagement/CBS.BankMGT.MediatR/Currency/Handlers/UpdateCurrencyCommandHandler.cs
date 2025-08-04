using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a Currency based on UpdateCurrencyCommand.
    /// </summary>
    public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, ServiceResponse<CurrencyDto>>
    {
        private readonly ICurrencyRepository _CurrencyRepository; // Repository for accessing Currency data.
        private readonly ILogger<UpdateCurrencyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCurrencyCommandHandler.
        /// </summary>
        /// <param name="CurrencyRepository">Repository for Currency data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCurrencyCommandHandler(
            ICurrencyRepository CurrencyRepository,
            ILogger<UpdateCurrencyCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _CurrencyRepository = CurrencyRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateCurrencyCommand to update a Currency.
        /// </summary>
        /// <param name="request">The UpdateCurrencyCommand containing updated Currency data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CurrencyDto>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Currency entity to be updated from the repository
                var existingCurrency = await _CurrencyRepository.FindAsync(request.Id);

                // Check if the Currency entity exists
                if (existingCurrency != null)
                {
                    // Update Currency entity properties with values from the request
                    existingCurrency.Name = request.Name;
                    existingCurrency.Code = request.Code;
                    existingCurrency.CountryID = request.CountryID;
                    // Use the repository to update the existing Currency entity
                    _CurrencyRepository.Update(existingCurrency);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<CurrencyDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<CurrencyDto>.ReturnResultWith200(_mapper.Map<CurrencyDto>(existingCurrency));
                    _logger.LogInformation($"Currency {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Currency entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CurrencyDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Currency: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CurrencyDto>.Return500(e);
            }
        }
    }

}

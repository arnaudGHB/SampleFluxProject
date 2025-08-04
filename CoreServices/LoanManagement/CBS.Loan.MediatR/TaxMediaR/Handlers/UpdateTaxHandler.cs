using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.Data.Entity.TaxP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.TaxMediaR.Commands;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.TaxMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateTaxHandler : IRequestHandler<UpdateTaxCommand, ServiceResponse<TaxDto>>
    {
        private readonly ITaxRepository _TaxRepository; // Repository for accessing Tax data.
        private readonly ILogger<UpdateTaxHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateTaxCommandHandler.
        /// </summary>
        /// <param name="TaxRepository">Repository for Tax data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateTaxHandler(
            ITaxRepository TaxRepository,
            ILogger<UpdateTaxHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _TaxRepository = TaxRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateTaxCommand to update a Tax.
        /// </summary>
        /// <param name="request">The UpdateTaxCommand containing updated Tax data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TaxDto>> Handle(UpdateTaxCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Tax entity to be updated from the repository
                var existingTax = await _TaxRepository.FindAsync(request.Id);

                // Check if the Tax entity exists
                if (existingTax != null)
                {
                    // Update Tax entity properties with values from the request
                
             
                    _mapper.Map(request, existingTax);

                    // Use the repository to update the existing Tax entity
                    _TaxRepository.Update(existingTax);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<TaxDto>.ReturnResultWith200(_mapper.Map<TaxDto>(existingTax));
                    _logger.LogInformation($"Tax {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Tax entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TaxDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Tax: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TaxDto>.Return500(e);
            }
        }
    }

}

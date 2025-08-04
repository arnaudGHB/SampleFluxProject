using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.Data.Entity.FundingLineP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FundingLineMediaR.Commands;
using CBS.NLoan.Repository.FundingLineP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FundingLineMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateFundingLineHandler : IRequestHandler<UpdateFundingLineCommand, ServiceResponse<FundingLineDto>>
    {
        private readonly IFundingLineRepository _FundingLineRepository; // Repository for accessing FundingLine data.
        private readonly ILogger<UpdateFundingLineHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateFundingLineCommandHandler.
        /// </summary>
        /// <param name="FundingLineRepository">Repository for FundingLine data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateFundingLineHandler(
            IFundingLineRepository FundingLineRepository,
            ILogger<UpdateFundingLineHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _FundingLineRepository = FundingLineRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateFundingLineCommand to update a FundingLine.
        /// </summary>
        /// <param name="request">The UpdateFundingLineCommand containing updated FundingLine data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FundingLineDto>> Handle(UpdateFundingLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the FundingLine entity to be updated from the repository
                var existingFundingLine = await _FundingLineRepository.FindAsync(request.Id);

                // Check if the FundingLine entity exists
                if (existingFundingLine != null)
                {
                    // Update FundingLine entity properties with values from the request
                    var FundingLineToUpdate = _mapper.Map<FundingLine>(request);
                    // Use the repository to update the existing FundingLine entity
                    _FundingLineRepository.Update(FundingLineToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<FundingLineDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<FundingLineDto>.ReturnResultWith200(_mapper.Map<FundingLineDto>(existingFundingLine));
                    _logger.LogInformation($"FundingLine {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the FundingLine entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<FundingLineDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating FundingLine: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FundingLineDto>.Return500(e);
            }
        }
    }

}

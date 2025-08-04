using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Entity;

namespace CBS.BankMGT.MediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a Bank based on UpdateBankCommand.
    /// </summary>
    public class UpdateThirdPartyBrancheCommandHandler : IRequestHandler<UpdateThirdPartyBrancheCommand, ServiceResponse<CorrespondingBankBranchDto>>
    {
        private readonly IThirdPartyBrancheRepository _thirdPartyBrancheRepository; // Repository for accessing Bank data.
        private readonly ILogger<UpdateThirdPartyBrancheCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateThirdPartyBrancheCommandHandler.
        /// </summary>
        /// <param name="BankRepository">Repository for Bank data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateThirdPartyBrancheCommandHandler(
            IThirdPartyBrancheRepository BankRepository,
            ILogger<UpdateThirdPartyBrancheCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _thirdPartyBrancheRepository = BankRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateBankCommand to update a Bank.
        /// </summary>
        /// <param name="request">The UpdateBankCommand containing updated Bank data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CorrespondingBankBranchDto>> Handle(UpdateThirdPartyBrancheCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Bank entity to be updated from the repository
                var existingBank = await _thirdPartyBrancheRepository.FindAsync(request.Id);

                // Check if the Bank entity exists
                if (existingBank != null)
                {
                    // Map properties from request to existingBank entity
                    _mapper.Map(request, existingBank);

                    // Use the repository to update the existing Bank entity
                    _thirdPartyBrancheRepository.Update(existingBank);

                    // Save changes to the database
                    await _uow.SaveAsync();

                    // Prepare the response and return a successful response with a 200 status code
                    var response = ServiceResponse<CorrespondingBankBranchDto>.ReturnResultWith200(_mapper.Map<CorrespondingBankBranchDto>(existingBank));
                    _logger.LogInformation($"ThirdPartyBranche {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Bank entity was not found, return a 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CorrespondingBankBranchDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Bank: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CorrespondingBankBranchDto>.Return500(e, errorMessage);
            }
        }
    }

}

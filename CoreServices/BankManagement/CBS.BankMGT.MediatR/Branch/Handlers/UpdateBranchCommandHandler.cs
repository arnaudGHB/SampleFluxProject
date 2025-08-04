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
    /// Handles the command to update a Branch based on UpdateBranchCommand.
    /// </summary>
    public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, ServiceResponse<BranchDto>>
    {
        private readonly IBranchRepository _BranchRepository; // Repository for accessing Branch data.
        private readonly ILogger<UpdateBranchCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateBranchCommandHandler.
        /// </summary>
        /// <param name="BranchRepository">Repository for Branch data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateBranchCommandHandler(
            IBranchRepository BranchRepository,
            ILogger<UpdateBranchCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _BranchRepository = BranchRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateBranchCommand to update a Branch.
        /// </summary>
        /// <param name="request">The UpdateBranchCommand containing updated Branch data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BranchDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Branch entity to be updated from the repository
                var existingBranch = await _BranchRepository.FindAsync(request.Id);

                // Check if the Branch entity exists
                if (existingBranch != null)
                {
                    // Use AutoMapper to map properties from request to existingBranch
                    _mapper.Map(request, existingBranch);

                    // Use the repository to update the existing Branch entity
                    _BranchRepository.Update(existingBranch);

                    await _uow.SaveAsync();
                   

                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<BranchDto>.ReturnResultWith200(_mapper.Map<BranchDto>(existingBranch));
                    _logger.LogInformation($"Branch {existingBranch.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Branch entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<BranchDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Branch: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<BranchDto>.Return500(e, errorMessage);
            }
        }
    }

}

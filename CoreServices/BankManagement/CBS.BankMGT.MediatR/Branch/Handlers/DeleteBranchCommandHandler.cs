using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Branch based on DeleteBranchCommand.
    /// </summary>
    public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, ServiceResponse<bool>>
    {
        private readonly IBranchRepository _BranchRepository; // Repository for accessing Branch data.
        private readonly ILogger<DeleteBranchCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteBranchCommandHandler.
        /// </summary>
        /// <param name="BranchRepository">Repository for Branch data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteBranchCommandHandler(
            IBranchRepository BranchRepository, IMapper mapper,
            ILogger<DeleteBranchCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _BranchRepository = BranchRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteBranchCommand to delete a Branch.
        /// </summary>
        /// <param name="request">The DeleteBranchCommand containing Branch ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Branch entity with the specified ID exists
                var existingBranch = await _BranchRepository.FindAsync(request.Id);
                if (existingBranch == null)
                {
                    errorMessage = $"Branch with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingBranch.IsDeleted = true;
                _BranchRepository.Update(existingBranch);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Branch: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}

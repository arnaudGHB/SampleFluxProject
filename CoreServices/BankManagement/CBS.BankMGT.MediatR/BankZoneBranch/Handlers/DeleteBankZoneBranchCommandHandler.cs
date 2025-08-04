using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Common.Repository.Generic;
using CBS.BankMGT.Data;


namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a BankZoneBranch based on DeleteBankZoneBranchCommand.
    /// </summary>
    public class DeleteBankZoneBranchCommandHandler : IRequestHandler<DeleteBankZoneBranchCommand, ServiceResponse<bool>>
    {
 
        //private readonly IMongoGenericRepository<BankZoneBranch> _recordRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DeleteBankZoneBranchCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken? _userInfoToken;
        private readonly IMongoUnitOfWork? _mongoUnitOfWork;
        /// <summary>
        /// Constructor for initializing the AddBankZoneBranchCommandHandler.
        /// </summary>
        /// <param name="BankZoneBranchRepository">Repository for BankZoneBranch data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteBankZoneBranchCommandHandler(

            IMapper mapper,
            ILogger<DeleteBankZoneBranchCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            IMongoUnitOfWork? mongoUnitOfWork,
            UserInfoToken? userInfoToken)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            //_recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;

        }


        /// <summary>
        /// Handles the DeleteBankZoneBranchCommand to delete a BankZoneBranch.
        /// </summary>
        /// <param name="request">The DeleteBankZoneBranchCommand containing BankZoneBranch ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteBankZoneBranchCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
                // Check if the BankZoneBranch entity with the specified ID exists
                var existingBankZoneBranch = await _recordRepository.GetByIdAsync(request.Id);
                if (existingBankZoneBranch == null)
                {
                    errorMessage = $"BankZoneBranch with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingBankZoneBranch.IsDeleted = true;
                BaseUtilities.PrepareMonoDBDataForCreation(existingBankZoneBranch, _userInfoToken, TrackerState.Deleted);
                // Map the AddBankZoneBranchCommand to a BankZoneBranch entity
    


                // Add the new BankZoneBranch entity to the repository
                await _recordRepository.UpdateAsync(existingBankZoneBranch.Id, existingBankZoneBranch);
                errorMessage = "BankZoneBranch pairs was successfully created ";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                                request, HttpStatusCodeEnum.OK, LogAction.DeleteBankZoneBranchCommand, LogLevelInfo.Information);
                // Map the BankZoneBranch entity to BankZoneBranchDto and return it with a success response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting BankZoneBranch: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                            request, HttpStatusCodeEnum.InternalServerError, LogAction.DeleteBankZoneBranchCommand, LogLevelInfo.Error);
                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data;
using CBS.BankZoneBranchMGT.MediatR.Commands;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Common.Repository.Generic;

namespace CBS.BankMGT.MediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a BankZoneBranch based on UpdateBankZoneBranchCommand.
    /// </summary>
    public class UpdateBankZoneBranchCommandHandler : IRequestHandler<UpdateBankZoneBranchCommand, ServiceResponse<BankZoneBranchDto>>
    {
 
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateBankZoneBranchCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken? _userInfoToken;
        private readonly IMongoUnitOfWork? _mongoUnitOfWork;

        /// <summary>
        /// Constructor for initializing the UpdateBankZoneBranchCommandHandler.
        /// </summary>
        /// <param name="BankZoneBranchRepository">Repository for BankZoneBranch data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateBankZoneBranchCommandHandler(

           IMapper mapper,
           ILogger<UpdateBankZoneBranchCommandHandler> logger,
           IUnitOfWork<POSContext> uow,
           IMongoUnitOfWork? mongoUnitOfWork,
           UserInfoToken? userInfoToken)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
 
            _mapper = mapper;
       
            _uow = uow;
            _userInfoToken = userInfoToken;
     
        }


        /// <summary>
        /// Handles the UpdateBankZoneBranchCommand to update a BankZoneBranch.
        /// </summary>
        /// <param name="request">The UpdateBankZoneBranchCommand containing updated BankZoneBranch data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BankZoneBranchDto>> Handle(UpdateBankZoneBranchCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
                // Retrieve the BankZoneBranch entity to be updated from the repository
                var existingBankZoneBranch = _recordRepository.FindBy(x=>x.Type.Equals(request.Type)&&x.BankingZoneId.Equals(request.BankingZoneId) && x.BranchId.Equals(request.BranchId)).FirstOrDefault();

                // Check if the BankZoneBranch entity exists
                if (existingBankZoneBranch != null)
                {
                    // Map properties from request to existingBankZoneBranch entity
                    _mapper.Map(request, existingBankZoneBranch);

                    BaseUtilities.PrepareMonoDBDataForCreation(existingBankZoneBranch, _userInfoToken, TrackerState.Modified);
                    // Use the repository to update the existing BankZoneBranch entity
                    await _recordRepository.UpdateAsync(existingBankZoneBranch.Id, existingBankZoneBranch);
                    // Prepare the response and return a successful response with a 200 status code
                    var response = ServiceResponse<BankZoneBranchDto>.ReturnResultWith200(_mapper.Map<BankZoneBranchDto>(existingBankZoneBranch));
                    errorMessage = $"BankZoneBranch paring was successfully updated.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                            request, HttpStatusCodeEnum.OK, LogAction.UpdateBankZoneBranchCommand, LogLevelInfo.Information);
                    _logger.LogInformation(errorMessage);
                    return response;
                }
                else
                {
                    // If the BankZoneBranch entity was not found, return a 404 Not Found response with an error message
                      errorMessage = $"Pare was not found to be updated.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                             request, HttpStatusCodeEnum.NotFound, LogAction.UpdateBankZoneBranchCommand, LogLevelInfo.Warning);
                    _logger.LogError(errorMessage);
                    return ServiceResponse<BankZoneBranchDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with an error message
                  errorMessage = $"Error occurred while updating BankZoneBranch: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                 request, HttpStatusCodeEnum.InternalServerError, LogAction.UpdateBankZoneBranchCommand, LogLevelInfo.Error);
                _logger.LogError(errorMessage);
                return ServiceResponse<BankZoneBranchDto>.Return500(e, errorMessage);
            }
        }
    }

}

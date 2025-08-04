using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
 
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data;
using CBS.BankMGT.Common.Repository.Generic;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Common.Repository.Uow;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific BankZoneBranch based on its unique identifier.
    /// </summary>
    public class GetBankZoneBranchQueryHandler : IRequestHandler<GetBankZoneBranchQuery, ServiceResponse<BankZoneBranchDto>>
    {
        //private readonly IMongoGenericRepository<BankZoneBranch> _recordRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBankZoneBranchQueryHandler> _logger; // Logger for logging handler actions and errors.
        //private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken? _userInfoToken;
        private readonly IMongoUnitOfWork? _mongoUnitOfWork;

        /// <summary>
        /// Constructor for initializing the GetBankZoneBranchQueryHandler.
        /// </summary>
        /// <param name="BankZoneBranchRepository">Repository for BankZoneBranch data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBankZoneBranchQueryHandler(

       IMapper mapper,
       ILogger<GetBankZoneBranchQueryHandler> logger,
   
       IMongoUnitOfWork? mongoUnitOfWork,
       UserInfoToken? userInfoToken)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            //_recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
            _mapper = mapper;
        
        }

        /// <summary>
        /// Handles the GetBankZoneBranchQuery to retrieve a specific BankZoneBranch.
        /// </summary>
        /// <param name="request">The GetBankZoneBranchQuery containing BankZoneBranch ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BankZoneBranchDto>> Handle(GetBankZoneBranchQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
                errorMessage = "";
                // Retrieve the BankZoneBranch entity with the specified ID from the repository
                var existingBankZoneBranch = await _recordRepository.GetByIdAsync( request.Id);
                if (existingBankZoneBranch != null)
                {
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
              request, HttpStatusCodeEnum.NotFound, LogAction.GetBankZoneBranchQuery, LogLevelInfo.Warning);
                    // Map the BankZoneBranch entity to BankZoneBranchDto and return it with a success response
                    var BankZoneBranchDto = _mapper.Map<BankZoneBranchDto>(existingBankZoneBranch);
               
                    return ServiceResponse<BankZoneBranchDto>.ReturnResultWith200(BankZoneBranchDto);
                  
                }
                else
                {
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                 request, HttpStatusCodeEnum.NotFound, LogAction.GetBankZoneBranchQuery, LogLevelInfo.Warning);
                    // If the BankZoneBranch entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("BankZoneBranch not found.");
                    return ServiceResponse<BankZoneBranchDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting BankZoneBranch: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<BankZoneBranchDto>.Return500(e);
            }
        }
    }

}

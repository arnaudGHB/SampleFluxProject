using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data;
using CBS.BankMGT.MediatR.Handlers;
using CBS.BankMGT.Common.Repository.Generic;
using CBS.BankMGT.Common.Repository.Uow;

namespace CBS.BankmGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all BankZoneBranchs based on the GetAllBankZoneBranchQuery.
    /// </summary>
    public class GetAllBankZoneBranchQueryHandler : IRequestHandler<GetAllBankZoneBranchQuery, ServiceResponse<List<BankZoneBranchDto>>>
    {
        //private readonly IMongoGenericRepository<BankZoneBranch> _recordRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBankZoneBranchQueryHandler> _logger; // Logger for logging handler actions and errors.
        //private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken? _userInfoToken;
        private readonly IMongoUnitOfWork? _mongoUnitOfWork;


        /// <summary>
        /// Constructor for initializing the GetAllBankZoneBranchQueryHandler.
        /// </summary>
        /// <param name="BankZoneBranchRepository">Repository for BankZoneBranchs data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
 
        public GetAllBankZoneBranchQueryHandler(

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
        /// Handles the GetAllBankZoneBranchQuery to retrieve all BankZoneBranchs.
        /// </summary>
        /// <param name="request">The GetAllBankZoneBranchQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BankZoneBranchDto>>> Handle(GetAllBankZoneBranchQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = "Get all record successfully retrieved";
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
                // Retrieve all BankZoneBranchs entities from the repository
                var entities = await _recordRepository.GetAllAsync();
                  await BaseUtilities.LogAndAuditAsync(errorMessage,
                            request, HttpStatusCodeEnum.OK, LogAction.GetAllBankZoneBranchQuery, LogLevelInfo.Information);
                return ServiceResponse<List<BankZoneBranchDto>>.ReturnResultWith200(_mapper.Map<List<BankZoneBranchDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all BankZoneBranchs: {e.Message}");
                errorMessage = $"BankZoneBranch paring was successfully updated.";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                        request, HttpStatusCodeEnum.InternalServerError, LogAction.GetAllBankZoneBranchQuery, LogLevelInfo.Error);
                return ServiceResponse<List<BankZoneBranchDto>>.Return500(e, "Failed to get all BankZoneBranchs");
            }
        }
    }
}

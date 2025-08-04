using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data;
using CBS.BankMGT.MediatR.Queries;
using System.Collections.Generic;
using CBS.BankMGT.Common.Repository.Generic;
using CBS.BankMGT.Common.Repository.Uow;

namespace CBS.BankmGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all BankZoneBranchs based on the GetAllBankZoneBranchQuery.
    /// </summary>
    public class GetBankingZonebyBranchQueryHandler : IRequestHandler<GetBankingZonebyBranchQuery, ServiceResponse<List<BranchDto>>>
    {
          // Repository for accessing BankZoneBranchs data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBankingZonebyBranchQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IBranchRepository _branchRepository;
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMongoGenericRepository<BankZoneBranch> _recordBankZoneBranchRepository;

        /// <summary>
        /// Constructor for initializing the GetAllBankZoneBranchQueryHandler.
        /// </summary>
        /// <param name="BankZoneBranchRepository">Repository for BankZoneBranchs data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBankingZonebyBranchQueryHandler(
    
            IMapper mapper, ILogger<GetBankingZonebyBranchQueryHandler> logger, IBranchRepository? branchRepository, IMongoUnitOfWork? mongoUnitOfWork)
        {
            _branchRepository = branchRepository;
            _mongoUnitOfWork = mongoUnitOfWork;

            //_recordBankZoneBranchRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBankZoneBranchQuery to retrieve all BankZoneBranchs.
        /// </summary>
        /// <param name="request">The GetAllBankZoneBranchQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BranchDto>>> Handle(GetBankingZonebyBranchQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
                // Retrieve all BankZoneBranchs entities from the repository

                var entities = await _recordBankZoneBranchRepository.FindBy(x=>x.Type.Equals(request.Type) &&x.BranchId.Equals(request.Id)).ToListAsync();
                var repositoryBranch = _branchRepository.All.ToList();
                List<BranchDto> Dataset = (from data in entities
                                          join b in repositoryBranch on data.BranchId equals b.Id
                                          select new BranchDto
                                          {
                                               BranchCode= b.BranchCode,
                                               Id=b.Id,
                                              Name= b.Name,
                                              Location= b.Location,
                                              Telephone = b.Telephone
                                          }
                                          ).ToList();
                errorMessage = $"Retreiving branch by BankingZonebyBranch  was successfull.";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                        request, HttpStatusCodeEnum.OK, LogAction.GetBankingZonebyBankBranchQuery, LogLevelInfo.Information);
                return ServiceResponse<List<BranchDto>>.ReturnResultWith200(Dataset);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get all BankZoneBranchs: {e.Message}");
                errorMessage = $"Retreiving branch by BankingZonebyBranch  was successfull.";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                        request, HttpStatusCodeEnum.InternalServerError, LogAction.GetBankingZonebyBankBranchQuery, LogLevelInfo.Error);
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all BankZoneBranchs: {e.Message}");
                return ServiceResponse<List<BranchDto>>.Return500(e, "Failed to get all BankZoneBranchs");
            }
        }
    }
}

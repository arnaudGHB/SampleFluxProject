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
using CBS.BankMGT.Data.Entity;
using CBS.BankMGT.Common.Repository.Uow;

namespace CBS.BankmGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all BankZoneBranchs based on the GetAllBankZoneBranchQuery.
    /// </summary>
    public class GetBankingZonebyBankBranchQueryHandler : IRequestHandler<GetBankingZonebyBankBranchQuery, ServiceResponse<List<LocalBrancheZoneInfo>>>
    {
        private readonly IBankZoneBranchRepository _BankZoneBranchRepository; // Repository for accessing BankZoneBranchs data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBankingZonebyBankBranchQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IBranchRepository _branchRepository;
        private readonly IThirdPartyInstitutionRepository _bankRepository;
        private readonly IBankingZoneRepository _bankingZoneRepository;
        private readonly IMongoUnitOfWork? _mongoUnitOfWork;

        /// <summary>
        /// Constructor for initializing the GetAllBankZoneBranchQueryHandler.
        /// </summary>
        /// <param name="BankZoneBranchRepository">Repository for BankZoneBranchs data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBankingZonebyBankBranchQueryHandler(
            IBankZoneBranchRepository BankZoneBranchRepository,
            IMapper mapper, ILogger<GetBankingZonebyBankBranchQueryHandler> logger, IBranchRepository? branchRepository, IThirdPartyInstitutionRepository? bankRepository, IBankingZoneRepository? bankingZoneRepository, IMongoUnitOfWork? mongoUnitOfWork)
        {
            _branchRepository = branchRepository;
             _BankZoneBranchRepository = BankZoneBranchRepository;
            _bankingZoneRepository = bankingZoneRepository;
            _bankRepository = bankRepository;
            _mapper = mapper;
            _mongoUnitOfWork = mongoUnitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBankZoneBranchQuery to retrieve all BankZoneBranchs.
        /// </summary>
        /// <param name="request">The GetAllBankZoneBranchQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LocalBrancheZoneInfo>>> Handle(GetBankingZonebyBankBranchQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            BankingZone bankingZone = new();
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
                // Retrieve all BankZoneBranchs entities from the repository
                var _recordBankingZoneRepository = _mongoUnitOfWork.GetRepository<BankingZone>();
                var entities = await _recordRepository.GetAllAsync();
                entities= entities.Where(x=>x.Type.Equals(request.Type) &&x.BranchId.Equals(request.Id));
                if (entities.Any())
                {
                    var MODEL = entities.FirstOrDefault();
                      bankingZone = await _recordBankingZoneRepository.GetByIdAsync(MODEL.BankingZoneId);
                }
            var  entitiesDb = await _recordRepository.GetAllAsync();
                entities = entitiesDb.Where(x => x.Type.Equals(request.Type) && x.BankingZoneId.Equals(bankingZone.Id)).ToList();
                var repositoryBranch = _branchRepository.All.ToList();
         
                List<LocalBrancheZoneInfo> Dataset = (from data in entities
                                          join b in repositoryBranch on data.BranchId equals b.Id
                                       
                                          select new LocalBrancheZoneInfo
                                          {
                                             Name= b.Name,
                                             Code=b.BranchCode,
                                              Id = b.Id 
                                          } ).ToList();
                errorMessage = $"Retreiving branch by BankBranch  was successfull.";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                        request, HttpStatusCodeEnum.OK, LogAction.GetBankingZonebyBankBranchQuery, LogLevelInfo.Information);
                return ServiceResponse<List<LocalBrancheZoneInfo>>.ReturnResultWith200(Dataset);
            }
            catch (Exception e)
            {
                errorMessage = $"Failed to get all BankZoneBranchs: {e.Message}";
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all BankZoneBranchs: {e.Message}");
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                     request, HttpStatusCodeEnum.InternalServerError, LogAction.GetBankingZonebyBankBranchQuery, LogLevelInfo.Error);
                return ServiceResponse<List<LocalBrancheZoneInfo>>.Return500(e, "Failed to get all ThirdPartyBrancheDtoInfo");
            }
        }
    }
}

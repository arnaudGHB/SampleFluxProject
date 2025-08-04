using AutoMapper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data.Dto;
using CBS.BankZoneBranchMGT.MediatR.Commands;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Common.Repository.Generic;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new BankZoneBranch.
    /// </summary>
    public class AddBankZoneBranchCommandHandler : IRequestHandler<AddBankZoneBranchCommand, ServiceResponse<BankZoneBranchDto>>
    {
        
        //private readonly IMongoGenericRepository<BankZoneBranch> _recordRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBankZoneBranchCommandHandler> _logger; // Logger for logging handler actions and errors.
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
        public AddBankZoneBranchCommandHandler(
           
            IMapper mapper,
            ILogger<AddBankZoneBranchCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            IMongoUnitOfWork? mongoUnitOfWork,
            UserInfoToken? userInfoToken)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
         
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        
        }

        /// <summary>
        /// Handles the AddBankZoneBranchCommand to add a new BankZoneBranch.
        /// </summary>
        /// <param name="request">The AddBankZoneBranchCommand containing BankZoneBranch data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BankZoneBranchDto>> Handle(AddBankZoneBranchCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = "";
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankZoneBranch>();
                // Check if a BankZoneBranch with the same name already exists (case-insensitive)
                //var existingBankZoneBranch =   _recordRepository.FindBy(c => c.Type.Equals( request.Type)&&c.BankingZoneId.Equals(request.BankingZoneId) && c.BranchId.Equals(request.BranchId));

                //// If a BankZoneBranch with the same name already exists, return a conflict response
                //if (existingBankZoneBranch.Any())
                //{
                //      errorMessage = $"BankZoneBranch paring has already exists.";
                //    _logger.LogError(errorMessage);
                //    await BaseUtilities.LogAndAuditAsync(errorMessage,
                //                     request, HttpStatusCodeEnum.Conflict, LogAction.AddBankZoneBranchCommand, LogLevelInfo.Error);
                //    return ServiceResponse<BankZoneBranchDto>.Return409(errorMessage);
                //}
                List<BankZoneBranch> bankZoneBranches = new List<BankZoneBranch>();
                foreach (var item in request.BranchId)
                {
                    var model = new BankZoneBranch { Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "BBZ"), BranchId = item, BankingZoneId = request.BankingZoneId, Type = request.Type, CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow) };
                      BaseUtilities.PrepareMonoDBDataForCreationWithoutBranchId(model, _userInfoToken, item,TrackerState.Created);
                    bankZoneBranches.Add(model);
                }

                // Map the AddBankZoneBranchCommand to a BankZoneBranch entity
                var BankZoneBranchEntity = _mapper.Map<List<BankZoneBranch>>(bankZoneBranches);

                // Convert UTC to local time and set it in the entity



                // Add the new BankZoneBranch entity to the repository
             await   _recordRepository.InsertManyAsync(bankZoneBranches);
                  errorMessage = "BankZoneBranch pairs was successfully created ";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                                request, HttpStatusCodeEnum.OK, LogAction.AddBankZoneBranchCommand, LogLevelInfo.Information);
                // Map the BankZoneBranch entity to BankZoneBranchDto and return it with a success response
                var BankZoneBranchDto = _mapper.Map<BankZoneBranchDto>(BankZoneBranchEntity[0]);
                return ServiceResponse<BankZoneBranchDto>.ReturnResultWith200(BankZoneBranchDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while saving BankZoneBranch: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                             request, HttpStatusCodeEnum.InternalServerError, LogAction.AddBankZoneBranchCommand, LogLevelInfo.Error);
                _logger.LogError(errorMessage);
                return ServiceResponse<BankZoneBranchDto>.Return500(e, errorMessage);
            }
        }
    }

}

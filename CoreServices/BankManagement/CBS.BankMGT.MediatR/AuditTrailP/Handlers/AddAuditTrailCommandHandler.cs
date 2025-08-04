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
using CBS.BankMGT.Common.Repository.Uow;
using static System.Formats.Asn1.AsnWriter;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AuditTrail.
    /// </summary>

    public class AddAuditTrailCommandHandler : IRequestHandler<AddAuditTrailCommand, ServiceResponse<AuditTrailDto>>
    {
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAuditTrailCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly UserInfoToken _userInfoToken;
        private readonly IBranchRepository _branchRepository; // Repository for accessing branch data.

        /// <summary>
        /// Constructor for initializing the AddAuditTrailCommandHandler.
        /// </summary>
        public AddAuditTrailCommandHandler(UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<AddAuditTrailCommandHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork,
            IBranchRepository branchRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
            _userInfoToken = userInfoToken;
            _branchRepository = branchRepository;
        }

        /// <summary>
        /// Handles the AddAuditTrailCommand to add a new AuditTrail.
        /// </summary>
        public async Task<ServiceResponse<AuditTrailDto>> Handle(AddAuditTrailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Map the AddAuditTrailCommand to an AuditTrail entity
                var auditTrailEntity = _mapper.Map<AuditTrail>(request);
                auditTrailEntity.Timestamp = BaseUtilities.UtcToLocal(DateTime.Now);

                // Set BankID and BranchID, handling potential null values in _userInfoToken
                auditTrailEntity.BankID = _userInfoToken?.BankID ?? "Nan";
                auditTrailEntity.BranchID = _userInfoToken?.BranchID ?? "Nan";

                // Set FullName and UserID, defaulting to request.UserName and "Nan" respectively
                auditTrailEntity.FullName = _userInfoToken?.FullName ?? request.UserName;
                auditTrailEntity.UserID = _userInfoToken?.Id ?? "Nan";

                // Check if BranchID is not null, then get BranchCode and BranchName from the repository
                if (!string.IsNullOrEmpty(_userInfoToken?.BranchID))
                {
                    var branch = await _branchRepository.FindAsync(_userInfoToken.BranchID);
                    if (branch != null)
                    {
                        auditTrailEntity.BranchCode = branch.BranchCode ?? "Nan";
                        auditTrailEntity.BranchName = branch.Name ?? "Nan";
                    }
                    else
                    {
                        auditTrailEntity.BranchCode = "Nan";
                        auditTrailEntity.BranchName = "Nan";
                    }
                }
                auditTrailEntity.Id = BaseUtilities.GenerateUniqueNumber(15);
                // Get the MongoDB repository for AuditTrail
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();

                
                // Add the AuditTrail entity to the MongoDB repository
                await auditTrailRepository.InsertAsync(auditTrailEntity);

                // Map the AuditTrail entity to AuditTrailDto and return it with a success response
                var auditTrailDto = _mapper.Map<AuditTrailDto>(auditTrailEntity);
                return ServiceResponse<AuditTrailDto>.ReturnResultWith200(auditTrailDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AuditTrail: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AuditTrailDto>.Return500(e);
            }
        }
    }



    //public class AddAuditTrailCommandHandler : IRequestHandler<AddAuditTrailCommand, ServiceResponse<AuditTrailDto>>
    //{
    //    private readonly IAuditTrailRepository _AuditTrailRepository; // Repository for accessing AuditTrail data.
    //    private readonly IMapper _mapper; // AutoMapper for object mapping.
    //    private readonly ILogger<AddAuditTrailCommandHandler> _logger; // Logger for logging handler actions and errors.
    //    private readonly IUnitOfWork<POSContext> _uow;
    //    private readonly UserInfoToken _userInfoToken;
    //    private readonly IBranchRepository _branchRepository; // Repository for accessing AuditTrail data.
    //    /// <summary>
    //    /// Constructor for initializing the AddAuditTrailCommandHandler.
    //    /// </summary>
    //    /// <param name="AuditTrailRepository">Repository for AuditTrail data access.</param>
    //    /// <param name="mapper">AutoMapper for object mapping.</param>
    //    /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
    //    /// <param name="logger">Logger for logging handler actions and errors.</param>
    //    public AddAuditTrailCommandHandler(UserInfoToken userInfoToken,
    //        IAuditTrailRepository AuditTrailRepository,
    //        IMapper mapper,
    //        ILogger<AddAuditTrailCommandHandler> logger,
    //        IUnitOfWork<POSContext> uow,
    //        IBranchRepository branchRepository)
    //    {
    //        _AuditTrailRepository = AuditTrailRepository;
    //        _mapper = mapper;
    //        _logger = logger;
    //        _uow = uow;
    //        _userInfoToken = userInfoToken;
    //        _branchRepository = branchRepository;
    //    }

    //    /// <summary>
    //    /// Handles the AddAuditTrailCommand to add a new AuditTrail.
    //    /// </summary>
    //    /// <param name="request">The AddAuditTrailCommand containing AuditTrail data.</param>
    //    /// <param name="cancellationToken">A cancellation token.</param>
    //    public async Task<ServiceResponse<AuditTrailDto>> Handle(AddAuditTrailCommand request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            // Map the AddAuditTrailCommand to an AuditTrail entity
    //            var auditTrailEntity = _mapper.Map<AuditTrail>(request);
    //            auditTrailEntity.Id = BaseUtilities.GenerateUniqueNumber();
    //            auditTrailEntity.Timestamp = BaseUtilities.UtcToLocal(DateTime.Now);

    //            // Set BankID and BranchID, handling potential null values in _userInfoToken
    //            auditTrailEntity.BankID = _userInfoToken?.BankID ?? "Nan";
    //            auditTrailEntity.BranchID = _userInfoToken?.BranchID ?? "Nan";

    //            // Set FullName and UserID, defaulting to request.UserName and "Nan" respectively
    //            auditTrailEntity.FullName = _userInfoToken?.FullName ?? request.UserName;
    //            auditTrailEntity.UserID = _userInfoToken?.Id ?? "Nan";

    //            // Check if BranchID is not null, then get BranchCode and BranchName from the repository
    //            if (!string.IsNullOrEmpty(_userInfoToken?.BranchID))
    //            {
    //                var branch = await _branchRepository.FindAsync(_userInfoToken.BranchID);
    //                if (branch != null)
    //                {
    //                    auditTrailEntity.BranchCode = branch.BranchCode ?? "Nan";
    //                    auditTrailEntity.BranchName = branch.Name ?? "Nan";
    //                }
    //                else
    //                {
    //                    auditTrailEntity.BranchCode = "Nan";
    //                    auditTrailEntity.BranchName = "Nan";
    //                }
    //            }

    //            // Add the new AuditTrail entity to the repository
    //            _AuditTrailRepository.Add(auditTrailEntity);
    //            await _uow.SaveAsync();

    //            // Map the AuditTrail entity to AuditTrailDto and return it with a success response
    //            var auditTrailDto = _mapper.Map<AuditTrailDto>(auditTrailEntity);
    //            return ServiceResponse<AuditTrailDto>.ReturnResultWith200(auditTrailDto);
    //        }
    //        catch (Exception e)
    //        {
    //            // Log error and return 500 Internal Server Error response with error message
    //            var errorMessage = $"Error occurred while saving AuditTrail: {e.Message}";
    //            _logger.LogError(errorMessage);
    //            return ServiceResponse<AuditTrailDto>.Return500(e);
    //        }
    //    }

    //}


}

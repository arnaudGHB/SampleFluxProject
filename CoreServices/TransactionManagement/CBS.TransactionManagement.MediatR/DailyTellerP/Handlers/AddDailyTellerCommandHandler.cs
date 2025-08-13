using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.DailyTellerP.Commands;

namespace CBS.TransactionManagement.DailyTellerP.Handlers
{
    /// <summary>
    /// Handles the command to add a new DailyTeller.
    /// </summary>
    public class AddDailyTellerCommandHandler : IRequestHandler<AddDailyTellerCommand, ServiceResponse<DailyTellerDto>>
    {
        private readonly IDailyTellerRepository _DailyTellerRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDailyTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ITellerRepository _tellerRepository;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddDailyTellerCommandHandler.
        /// </summary>
        /// <param name="DailyTellerRepository">Repository for DailyTeller data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDailyTellerCommandHandler(
            IDailyTellerRepository DailyTellerRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddDailyTellerCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            ITellerRepository tellerRepository = null)
        {
            _DailyTellerRepository = DailyTellerRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _tellerRepository = tellerRepository;
        }

        /// <summary>
        /// Handles the AddDailyTellerCommand to add a new DailyTeller.
        /// </summary>
        /// <param name="request">The AddDailyTellerCommand containing DailyTeller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DailyTellerDto>> Handlex(AddDailyTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var savingProductEntity = await GetDailyTellerAsync(request);

                if (savingProductEntity != null)
                {
                    var errorMessage = $"{request.UserName} is still having an active session in selected teller.";
                    await LogAndAuditError(request, errorMessage, 403);
                    return ServiceResponse<DailyTellerDto>.Return409(errorMessage);
                }
                var dailyTeller = await CheckIfPrimaryTellerAlreadyExist(request);
                if (dailyTeller != null)
                {
                    var errorMessage = $"Primary teller already assiged for the selected branch.";
                    await LogAndAuditError(request, errorMessage, 403);
                    return ServiceResponse<DailyTellerDto>.Return409(errorMessage);
                }
                var teller = await _tellerRepository.GetTeller(request.TellerId);
                if (teller.IsPrimary)
                {
                    request.IsPrimary = true;
                }
                var DailyTellerEntity = _mapper.Map<DailyTeller>(request);
                DailyTellerEntity.Id = BaseUtilities.GenerateUniqueNumber();
                _DailyTellerRepository.Add(DailyTellerEntity);

                await _uow.SaveAsync();
                var DailyTellerDto = _mapper.Map<DailyTellerDto>(DailyTellerEntity);
                string msg = $"Daily teller {request.UserName} was added successfully";
                await LogAndAuditInfo(request, msg, 200);
                return ServiceResponse<DailyTellerDto>.ReturnResultWith200(DailyTellerDto, msg);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving DailyTeller: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);
                return ServiceResponse<DailyTellerDto>.Return500(e);
            }
        }
        public async Task<ServiceResponse<DailyTellerDto>> Handle(AddDailyTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var teller = await _tellerRepository.FindAsync(request.TellerId);
                request.IsPrimary = teller.IsPrimary;
                // Step 1: Check if the user already has an active session in the selected teller.
                var activeTeller = await GetDailyTellerAsync(request);
                if (activeTeller != null)
                {
                    var errorMessage = $"{request.UserName} is still having an active session in the selected teller.";
                    await LogAndAuditError(request, errorMessage, 409);  // Conflict
                    return ServiceResponse<DailyTellerDto>.Return409(errorMessage);
                }

                // Step 2: Check if a primary teller already exists for the selected branch, excluding the current user.
                var existingPrimaryTeller = await CheckIfPrimaryTellerAlreadyExist(request);
                if (existingPrimaryTeller != null && existingPrimaryTeller.UserId != request.UserId && request.IsPrimary)
                {
                    var errorMessage = "Primary teller is already assigned for the selected branch.";
                    await LogAndAuditError(request, errorMessage, 409);  // Conflict
                    return ServiceResponse<DailyTellerDto>.Return403(errorMessage);
                }

                // Step 3: Ensure the teller is not already assigned to another user.
                var isTellerAssigned = await _DailyTellerRepository
                    .FindBy(x => x.TellerId == request.TellerId && x.IsDeleted == false)
                    .FirstOrDefaultAsync();

                if (isTellerAssigned != null)
                {
                    var errorMessage = $"Teller {request.TellerId} is already assigned to another user.";
                    await LogAndAuditError(request, errorMessage, 403);  // Forbidden
                    return ServiceResponse<DailyTellerDto>.Return403(errorMessage);
                }

                // Step 4: Check if the user already has a sub-teller and is trying to add another sub-teller.
                var userHasAnotherSubTeller = await _DailyTellerRepository
                    .FindBy(x => x.UserId == request.UserId && x.IsPrimary == false && x.IsDeleted == false)
                    .Include(x => x.Teller)
                    .FirstOrDefaultAsync();

                if (userHasAnotherSubTeller != null && !request.IsPrimary)
                {
                    var errorMessage = $"User {request.UserName} already occupies a sub-teller: {userHasAnotherSubTeller.Teller.Name}. Cannot assign another sub-teller.";
                    await LogAndAuditError(request, errorMessage, 403);  // Forbidden
                    return ServiceResponse<DailyTellerDto>.Return403(errorMessage);
                }

                // Step 5: Ensure the user is within the same branch for sub-teller assignment.
                if (!request.IsPrimary && request.UserBranchId != request.BranchId)
                {
                    var errorMessage = "Sub-teller can only be assigned to users within the same branch.";
                    await LogAndAuditError(request, errorMessage, 403);  // Forbidden
                    return ServiceResponse<DailyTellerDto>.Return409(errorMessage);
                }

                //// Step 6: Fetch the teller details from the repository.
                //var teller = await _tellerRepository.GetTeller(request.TellerId);
                if (teller == null)
                {
                    var errorMessage = $"Teller with Id {request.TellerId} not found.";
                    await LogAndAuditError(request, errorMessage, 404);  // Not Found
                    return ServiceResponse<DailyTellerDto>.Return404(errorMessage);
                }

                // Step 7: If the teller is a primary teller, mark the request as primary.
                if (teller.IsPrimary)
                {
                    request.IsPrimary = true;
                }

                // Step 8: Create the DailyTeller entity using AutoMapper.
                var DailyTellerEntity = _mapper.Map<DailyTeller>(request);
                DailyTellerEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Step 9: Add the new DailyTeller entity to the repository.
                _DailyTellerRepository.Add(DailyTellerEntity);

                // Step 10: Save changes to the database.
                await _uow.SaveAsync();

                // Step 11: Map the saved entity to a DTO and return a successful response (200 OK).
                var DailyTellerDto = _mapper.Map<DailyTellerDto>(DailyTellerEntity);
                string msg = $"Daily teller {request.UserName} was added successfully.";
                await LogAndAuditInfo(request, msg, 200);  // OK
                return ServiceResponse<DailyTellerDto>.ReturnResultWith200(DailyTellerDto, msg);
            }
            catch (Exception e)
            {
                // Step 12: Handle any exceptions that occur during processing.
                var errorMessage = $"Error occurred while saving DailyTeller: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);  // Internal Server Error
                return ServiceResponse<DailyTellerDto>.Return500(e);
            }
        }

        private async Task<DailyTeller> GetDailyTellerAsync(AddDailyTellerCommand command)
        {
            // Check if the user already has a primary teller in the given branch
            var existingPrimaryTeller = await _DailyTellerRepository
                .FindBy(x => x.BranchId == command.BranchId && x.IsDeleted == false && x.Status && x.IsPrimary)
                .FirstOrDefaultAsync();

            // Check if the user already has a sub-teller in the given branch
            var existingSubTeller = await _DailyTellerRepository
                .FindBy(x => x.UserId == command.UserId && x.BranchId == command.BranchId && x.IsDeleted == false && x.Status && !x.IsPrimary)
                .FirstOrDefaultAsync();

            // Handle primary teller assignment
            if (command.IsPrimary)
            {
                // Allow primary teller if:
                // - There is no existing primary teller in the branch.
                // - User already has a sub-teller in the branch but not a primary teller.
                if (existingPrimaryTeller != null)
                {
                    throw new InvalidOperationException(
                        $"Cannot assign a new primary teller for Branch because a primary teller already exists. " +
                        "Each branch is limited to one primary teller. Please check the existing assignments for this branch."
                    );
                }
            }
            else
            {
                // Handle sub-teller assignment
                // Allow sub-teller if:
                // - There is no existing sub-teller in the branch.
                // - If the user has no tellers in the branch, a sub-teller can be assigned.
                if (existingSubTeller != null)
                {
                    throw new InvalidOperationException(
                        $"Cannot assign a new sub-teller to User the selected in Branch the selected." +
                        $"because the user already has an active sub-teller assigned in this branch." +
                        "A user is limited to only one sub-teller per branch. Please review the current sub-teller assignment."
                    );
                }
            }

            // Proceed to find or create a teller if the user passes the constraints
            var data = await _DailyTellerRepository
                .FindBy(x => x.TellerId == command.TellerId && x.UserId == command.UserId && x.IsDeleted == false && x.Status && !x.IsPrimary)
                .FirstOrDefaultAsync();

            // If no specific non-primary teller is found, search for a general non-primary teller for the user
            if (data == null)
            {
                data = await _DailyTellerRepository
                    .FindBy(x => x.UserId == command.UserId && x.IsDeleted == false && x.Status && !x.IsPrimary)
                    .FirstOrDefaultAsync();
            }

            return data;
        }


        private async Task<DailyTeller> CheckIfPrimaryTellerAlreadyExist(AddDailyTellerCommand command)
        {
            var data = await _DailyTellerRepository.FindBy(x => x.TellerId == command.TellerId && x.BranchId == command.BranchId && x.IsPrimary && x.IsDeleted == false).FirstOrDefaultAsync();

            return data;
        }


        private async Task LogAndAuditError(AddDailyTellerCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private async Task LogAndAuditInfo(AddDailyTellerCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

    }

}

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
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Commands;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;

namespace CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Handlers
{
    /// <summary>
    /// Handles the command to add a new MemberAccountActivationPolicy.
    /// </summary>
    public class AddMemberAccountActivationPCommandHandler : IRequestHandler<AddMemberAccountActivationPolicyCommand, ServiceResponse<MemberAccountActivationPolicyDto>>
    {
        private readonly IMemberAccountActivationPolicyRepository _MemberAccountActivationPolicyRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddMemberAccountActivationPCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddMemberAccountActivationPolicyCommandHandler.
        /// </summary>
        /// <param name="MemberAccountActivationPolicyRepository">Repository for MemberAccountActivationPolicy data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddMemberAccountActivationPCommandHandler(
            IMemberAccountActivationPolicyRepository MemberAccountActivationPolicyRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddMemberAccountActivationPCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _MemberAccountActivationPolicyRepository = MemberAccountActivationPolicyRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddMemberAccountActivationPolicyCommand to add a new MemberAccountActivationPolicy.
        /// </summary>
        /// <param name="request">The AddMemberAccountActivationPolicyCommand containing MemberAccountActivationPolicy data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MemberAccountActivationPolicyDto>> Handle(AddMemberAccountActivationPolicyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var feePolicies = await _MemberAccountActivationPolicyRepository.FindBy(x => x.PolicyName == request.PolicyName && x.LegalForm == request.LegalForm && x.IsDeleted == false).ToListAsync();

                if (feePolicies.Any())
                {
                    var errorMessage = $"{request.PolicyName} for {request.LegalForm} already exist.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<MemberAccountActivationPolicyDto>.Return409(errorMessage);
                }

                var existingMemberAccountActivationPolicy = await _MemberAccountActivationPolicyRepository.FindBy(x => x.IsDeleted == false).ToListAsync();
                if (existingMemberAccountActivationPolicy.Count() == 2)
                {
                    var errorMessage = $"Only two policy for Moral and Physical person can exist for now. You can update.";
                    await LogAndAuditError(request, errorMessage, 403);
                    return ServiceResponse<MemberAccountActivationPolicyDto>.Return403(errorMessage);
                }

                var MemberAccountActivationPolicyEntity = _mapper.Map<MemberRegistrationFeePolicy>(request);
                MemberAccountActivationPolicyEntity.Id = BaseUtilities.GenerateUniqueNumber();
                _MemberAccountActivationPolicyRepository.Add(MemberAccountActivationPolicyEntity);
                await _uow.SaveAsync();

                await LogAndAuditInfo(request, "Policy created successfully", 200);
                var MemberAccountActivationPolicyDto = _mapper.Map<MemberAccountActivationPolicyDto>(MemberAccountActivationPolicyEntity);
                return ServiceResponse<MemberAccountActivationPolicyDto>.ReturnResultWith200(MemberAccountActivationPolicyDto);


            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving MemberAccountActivationPolicy: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);
                return ServiceResponse<MemberAccountActivationPolicyDto>.Return500(e);
            }
        }


        private async Task LogAndAuditError(AddMemberAccountActivationPolicyCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private async Task LogAndAuditInfo(AddMemberAccountActivationPolicyCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

    }

}

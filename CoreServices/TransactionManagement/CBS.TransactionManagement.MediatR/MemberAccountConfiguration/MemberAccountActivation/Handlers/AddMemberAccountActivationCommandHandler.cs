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
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.MemberAccountConfiguration.Commands;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;

namespace CBS.TransactionManagement.MemberAccountConfiguration.Handlers
{
    /// <summary>
    /// Handles the command to add a new MemberAccountActivation.
    /// </summary>
    public class AddMemberAccountActivationCommandHandler : IRequestHandler<AddMemberAccountActivationCommand, ServiceResponse<decimal>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IMemberAccountActivationPolicyRepository _memberAccountActivationPolicyRepository; // Repository for accessing MemberAccountActivation data.
        private readonly IMemberAccountActivationRepository _MemberAccountActivationRepository; // Repository for accessing MemberAccountActivation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddMemberAccountActivationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddMemberAccountActivationCommandHandler.
        /// </summary>
        /// <param name="MemberAccountActivationRepository">Repository for MemberAccountActivation data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddMemberAccountActivationCommandHandler(
            IMemberAccountActivationRepository MemberAccountActivationRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddMemberAccountActivationCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            IMemberAccountActivationPolicyRepository memberAccountActivationPolicyRepository = null)
        {
            _MemberAccountActivationRepository = MemberAccountActivationRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _memberAccountActivationPolicyRepository = memberAccountActivationPolicyRepository;
        }

        /// <summary>
        /// Handles the AddMemberAccountActivationCommand to add a new MemberAccountActivation.
        /// </summary>
        /// <param name="request">The AddMemberAccountActivationCommand containing MemberAccountActivation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<decimal>> Handle(AddMemberAccountActivationCommand request, CancellationToken cancellationToken)
        {
            try
            {


                if (request.IsNew)
                {
                    decimal subcriptionAmount = request.MemberAccountActivations.Sum(x => x.CustomeAmount);
                    foreach (var memberAccount in request.MemberAccountActivations)
                    {
                        memberAccount.Id = BaseUtilities.GenerateUniqueNumber();
                        memberAccount.Status = true;
                        _MemberAccountActivationRepository.Add(memberAccount);
                    }
                    await _uow.SaveAsync();
                    string message = $"Subcription was created successfully with a total of {subcriptionAmount}";
                    return ServiceResponse<decimal>.ReturnResultWith200(subcriptionAmount, message);

                }
                else
                {
                    decimal subcriptionAmount = request.MemberAccountActivations.Sum(x => x.CustomeAmount);
                    foreach (var memberAccount in request.MemberAccountActivations)
                    {
                        memberAccount.Id = BaseUtilities.GenerateUniqueNumber();
                        memberAccount.Status = true;
                        memberAccount.Fee = null;
                        _MemberAccountActivationRepository.Add(memberAccount);
                    }
                    string message = $"Subcription was created successfully with a total of {subcriptionAmount}";
                    return ServiceResponse<decimal>.ReturnResultWith200(subcriptionAmount, message);

                }
            }
            catch (Exception e)
            {
                return HandleErrorSavingMemberAccountActivation(e);
            }
        }

        private ServiceResponse<decimal> HandleUnknownAccountActivation(AddMemberAccountActivationCommand request)
        {
            var errorMessage = "Unknown Saving product.";
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
            return ServiceResponse<decimal>.Return409(errorMessage);
        }

        private ServiceResponse<decimal> HandleExistingMemberAccountActivation(string transferType)
        {
            var errorMessage = $"MemberAccountActivation {transferType} already exists.";
            _logger.LogError(errorMessage);
            return ServiceResponse<decimal>.Return409(errorMessage);
        }

        

        private ServiceResponse<decimal> HandleErrorSavingMemberAccountActivation(Exception e)
        {
            var errorMessage = $"Error occurred while saving MemberAccountActivation: {e.Message}";
            _logger.LogError(errorMessage);
            return ServiceResponse<decimal>.Return500(e, errorMessage);
        }
       
       


    }

}

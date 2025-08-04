using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.MemberAccountConfiguration.Commands;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;

namespace CBS.TransactionManagement.MemberAccountConfiguration.Handlers
{

    /// <summary>
    /// Handles the command to update a MemberAccountActivation based on UpdateMemberAccountActivationCommand.
    /// </summary>
    public class UpdateMemberAccountActivationCommandHandler : IRequestHandler<UpdateMemberAccountActivationCommand, ServiceResponse<MemberAccountActivationDto>>
    {
        private readonly IMemberAccountActivationRepository _MemberAccountActivationRepository; // Repository for accessing MemberAccountActivation data.
        private readonly ILogger<UpdateMemberAccountActivationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateMemberAccountActivationCommandHandler.
        /// </summary>
        /// <param name="MemberAccountActivationRepository">Repository for MemberAccountActivation data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateMemberAccountActivationCommandHandler(
            IMemberAccountActivationRepository MemberAccountActivationRepository,
            ILogger<UpdateMemberAccountActivationCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow)
        {
            _MemberAccountActivationRepository = MemberAccountActivationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateMemberAccountActivationCommand to update a MemberAccountActivation.
        /// </summary>
        /// <param name="request">The UpdateMemberAccountActivationCommand containing updated MemberAccountActivation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MemberAccountActivationDto>> Handle(UpdateMemberAccountActivationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the MemberAccountActivation entity to be updated from the repository
                var existingMemberAccountActivation = await _MemberAccountActivationRepository.FindAsync(request.Id);

                // Check if the MemberAccountActivation entity exists
                if (existingMemberAccountActivation != null)
                {
                    _mapper.Map(request, existingMemberAccountActivation);
                    existingMemberAccountActivation.ModifiedDate = DateTime.Now;
                    // Use the repository to update the existing MemberAccountActivation entity
                    _MemberAccountActivationRepository.Update(existingMemberAccountActivation);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<MemberAccountActivationDto>.ReturnResultWith200(_mapper.Map<MemberAccountActivationDto>(existingMemberAccountActivation));
                    _logger.LogInformation($"MemberAccountActivation {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the MemberAccountActivation entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<MemberAccountActivationDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating MemberAccountActivation: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<MemberAccountActivationDto>.Return500(e);
            }
        }
    }

}

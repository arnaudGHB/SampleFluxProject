using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries;
using CBS.TransactionManagement.Data.Dto.MemberNoneCashOperationP;
using CBS.TransactionManagement.Repository.MemberNoneCashOperationP;
using CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Queries;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Handlers
{
    /// <summary>
    /// Handles the query to retrieve a Member None Cash Operation by its ID.
    /// </summary>
    public class GetMemberNoneCashOperationByIdHandler : IRequestHandler<GetMemberNoneCashOperationByIdQuery, ServiceResponse<MemberNoneCashOperationDto>>
    {
        private readonly IMemberNoneCashOperationRepository _noneCashOperationRepository; // Repository for accessing None Cash Operations.
        private readonly IMapper _mapper; // AutoMapper for mapping between entities and DTOs.
        private readonly ILogger<GetMemberNoneCashOperationByIdHandler> _logger; // Logger for logging actions and errors.

        public GetMemberNoneCashOperationByIdHandler(
            IMemberNoneCashOperationRepository noneCashOperationRepository,
            IMapper mapper,
            ILogger<GetMemberNoneCashOperationByIdHandler> logger)
        {
            _noneCashOperationRepository = noneCashOperationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the query to retrieve a Member None Cash Operation by its ID.
        /// </summary>
        /// <param name="request">The query containing the ID of the operation to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A service response containing the operation details or an appropriate error message.</returns>
        public async Task<ServiceResponse<MemberNoneCashOperationDto>> Handle(GetMemberNoneCashOperationByIdQuery request, CancellationToken cancellationToken)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber(); // Generate a unique reference for logging.

            try
            {
                // Step 1: Validate input.
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    string errorMessage = "Operation ID cannot be null or empty.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.GetMemberNoneCashOperationById, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<MemberNoneCashOperationDto>.Return400(errorMessage);
                }

                // Step 2: Fetch the operation by ID from the repository.
                var operation = await _noneCashOperationRepository.All.Include(x=>x.Account).FirstOrDefaultAsync(x=>x.Id==request.Id);

                // Step 3: Check if the operation exists and is not deleted.
                if (operation == null || operation.IsDeleted)
                {
                    string errorMessage = $"Member None Cash Operation with ID {request.Id} not found.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.GetMemberNoneCashOperationById, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<MemberNoneCashOperationDto>.Return404(errorMessage);
                }

                // Step 4: Map the operation to its corresponding DTO.
                var operationDto = _mapper.Map<MemberNoneCashOperationDto>(operation);

                // Step 5: Log and return the successful response.
                string successMessage = "Operation retrieved successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.GetMemberNoneCashOperationById, LogLevelInfo.Information, logReference);
                return ServiceResponse<MemberNoneCashOperationDto>.ReturnResultWith200(operationDto, successMessage);
            }
            catch (Exception ex)
            {
                // Step 6: Handle and log any unexpected exceptions.
                string errorMessage = $"An error occurred while retrieving the Member None Cash Operation: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GetMemberNoneCashOperationById, LogLevelInfo.Error, logReference);
                return ServiceResponse<MemberNoneCashOperationDto>.Return500(errorMessage);
            }
        }
    }

}

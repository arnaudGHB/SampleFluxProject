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
using CBS.TransactionManagement.DailyTellerP.Commands;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.DailyTellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a DailyTeller based on UpdateDailyTellerCommand.
    /// </summary>
    public class UpdateDailyTellerCommandHandler : IRequestHandler<UpdateDailyTellerCommand, ServiceResponse<DailyTellerDto>>
    {
        private readonly IDailyTellerRepository _DailyTellerRepository; // Repository for accessing DailyTeller data.
        private readonly ILogger<UpdateDailyTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ITellerRepository _tellerRepository; // Repository for accessing DailyTeller data.

        /// <summary>
        /// Constructor for initializing the UpdateDailyTellerCommandHandler.
        /// </summary>
        /// <param name="DailyTellerRepository">Repository for DailyTeller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateDailyTellerCommandHandler(
            IDailyTellerRepository DailyTellerRepository,
            ILogger<UpdateDailyTellerCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            ITellerRepository tellerRepository = null)
        {
            _DailyTellerRepository = DailyTellerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _tellerRepository = tellerRepository;
        }

        /// <summary>
        /// Handles the UpdateDailyTellerCommand to update a DailyTeller.
        /// </summary>
        /// <param name="request">The UpdateDailyTellerCommand containing updated DailyTeller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DailyTellerDto>> Handle(UpdateDailyTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Fetch the existing DailyTeller record by its Id to ensure it exists
                var existingDailyTeller = await _DailyTellerRepository.FindAsync(request.Id);

                // Check if the DailyTeller exists, return a 404 error if not found
                if (existingDailyTeller == null)
                {
                    return ServiceResponse<DailyTellerDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Step 2: Fetch the related teller from the repository using the provided TellerId
                var teller = await _tellerRepository.FindAsync(request.TellerId);

                if (teller == null)
                {
                    return ServiceResponse<DailyTellerDto>.Return404($"Teller with Id {request.TellerId} not found.");
                }

                // Step 3: Check if the teller is already assigned to another user
                var isTellerAssigned = await _DailyTellerRepository
                    .FindBy(x => x.TellerId == request.TellerId && !x.Status && x.Id != request.Id && !x.IsDeleted == false && !x.IsPrimary)
                    .FirstOrDefaultAsync();

                if (isTellerAssigned != null)
                {
                    return ServiceResponse<DailyTellerDto>.Return403($"Teller {teller.Name} is already assigned to another user: {isTellerAssigned.UserName}");
                }

                // Step 4: Check if the request is attempting to make the teller a primary teller
                if (request.IsPrimary)
                {
                    // Ensure no other primary teller exists for the same branch
                    var existingPrimaryTeller = await _DailyTellerRepository
                        .FindBy(x => x.IsPrimary && !x.IsDeleted && !x.Status && x.BranchId == request.BranchId)
                        .FirstOrDefaultAsync();

                    // If another primary teller exists in the same branch (and it's not the current teller being updated), return an error
                    if (existingPrimaryTeller != null && existingPrimaryTeller.Id != request.Id)
                    {
                        return ServiceResponse<DailyTellerDto>.Return403($"A primary teller already exists for the selected branch.");
                    }
                }

                // Step 5: Ensure the user does not occupy more than one sub-teller
                var userHasAnotherSubTeller = await _DailyTellerRepository
                    .FindBy(x => x.UserId == request.UserId && x.IsPrimary == false && x.Id != request.Id && x.IsDeleted == false && !x.Status)
                    .Include(x => x.Teller)
                    .FirstOrDefaultAsync();

                if (userHasAnotherSubTeller != null && !request.IsPrimary)
                {
                    return ServiceResponse<DailyTellerDto>.Return403($"User {existingDailyTeller.UserName} already occupies a sub-teller: {userHasAnotherSubTeller.Teller.Name}");
                }

                // Step 6: Ensure the user is within the same branch for sub-teller assignment
                if (!request.IsPrimary)
                {
                    if (request.UserBranchId != request.BranchId)
                    {
                        return ServiceResponse<DailyTellerDto>.Return403($"Sub-teller can only be assigned to users within the same branch.");
                    }
                }

                // Step 7: Update the `IsPrimary` status if the teller being updated is a primary teller
                if (teller.IsPrimary)
                {
                    request.IsPrimary = true;
                }

                // Step 8: Use AutoMapper to map the properties from the UpdateDailyTellerCommand (request)
                // to the existing DailyTeller entity.
                _mapper.Map(request, existingDailyTeller);

                // Step 9: Update the existing DailyTeller entity in the repository
                _DailyTellerRepository.Update(existingDailyTeller);

                // Step 10: Save changes to the unit of work
                await _uow.SaveAsync();

                // Step 11: Prepare the success message
                string msg = $"Daily teller {request.UserName} was successfully assigned.";

                // Step 12: Map the updated DailyTeller entity to a DTO and return a successful response (200 OK)
                var response = ServiceResponse<DailyTellerDto>.ReturnResultWith200(_mapper.Map<DailyTellerDto>(existingDailyTeller), msg);

                // Step 13: Log the success message for tracking purposes
                _logger.LogInformation(msg);

                // Step 14: Return the successful response
                return response;
            }
            catch (Exception e)
            {
                // Handle exceptions and return a 500 Internal Server Error response
                string errorMessage = $"Error occurred while updating DailyTeller: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DailyTellerDto>.Return500(e);
            }
        }

        public async Task<ServiceResponse<DailyTellerDto>> Handlex(UpdateDailyTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingDailyTeller = await _DailyTellerRepository.FindAsync(request.Id);

                if (existingDailyTeller == null)
                {
                    return ServiceResponse<DailyTellerDto>.Return404($"{request.Id} was not found to be updated.");
                }
                var teller = await _tellerRepository.FindAsync(request.TellerId);
                if (teller.IsPrimary)
                {
                    request.IsPrimary = true;
                }
                // Use AutoMapper to map properties from the request to the existingDailyTeller
                _mapper.Map(request, existingDailyTeller);

                // Use the repository to update the existing DailyTeller entity
                _DailyTellerRepository.Update(existingDailyTeller);

                // Save changes
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<DailyTellerDto>.Return500();
                }
                string msg = $"Daily teller {request.UserName} was successfully updated.";
                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<DailyTellerDto>.ReturnResultWith200(_mapper.Map<DailyTellerDto>(existingDailyTeller), msg);
                _logger.LogInformation(msg);
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating DailyTeller: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DailyTellerDto>.Return500(e);
            }
        }
    }

}

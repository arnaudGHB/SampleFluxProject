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
using CBS.TransactionManagement.OldLoanConfiguration.Commands;
using CBS.TransactionManagement.Repository.OldLoanConfiguration;
using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;

namespace CBS.TransactionManagement.OldLoanConfiguration.Handlers
{

  

    /// <summary>
    /// Handles the UpdateOldLoanAccountingMapingCommand to update an existing OldLoanAccountingMaping.
    /// </summary>
    /// <param name="request">The UpdateOldLoanAccountingMapingCommand containing the updated OldLoanAccountingMaping data.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public class UpdateOldLoanAccountingMapingCommandHandler : IRequestHandler<UpdateOldLoanAccountingMapingCommand, ServiceResponse<OldLoanAccountingMapingDto>>
    {
        private readonly IOldLoanAccountingMapingRepository _OldLoanAccountingMapingRepository;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOldLoanAccountingMapingCommandHandler> _logger;

        public UpdateOldLoanAccountingMapingCommandHandler(
            IOldLoanAccountingMapingRepository OldLoanAccountingMapingRepository,
            IUnitOfWork<TransactionContext> uow,
            IMapper mapper,
            ILogger<UpdateOldLoanAccountingMapingCommandHandler> logger)
        {
            _OldLoanAccountingMapingRepository = OldLoanAccountingMapingRepository;
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<OldLoanAccountingMapingDto>> Handle(UpdateOldLoanAccountingMapingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch existing mapping from the repository by Id
                var existingOldLoanAccountingMaping = await _OldLoanAccountingMapingRepository.FindAsync(request.Id);

                if (existingOldLoanAccountingMaping == null)
                {
                    string notFoundMessage = $"OldLoanAccountingMaping with Id {request.Id} was not found.";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.AccountingMapping, LogLevelInfo.Warning);
                    return ServiceResponse<OldLoanAccountingMapingDto>.Return404(notFoundMessage);
                }

                // Use AutoMapper to map updated properties from the request to the existing entity
                _mapper.Map(request, existingOldLoanAccountingMaping);

                // Update the entity in the repository
                _OldLoanAccountingMapingRepository.Update(existingOldLoanAccountingMaping);

                // Save changes
                await _uow.SaveAsync();

                // Log and audit the successful update
                string successMessage = $"Successfully updated OldLoanAccountingMaping {existingOldLoanAccountingMaping.LoanTypeName}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, existingOldLoanAccountingMaping, HttpStatusCodeEnum.OK, LogAction.AccountingMapping, LogLevelInfo.Information);

                // Prepare response DTO and return success result
                var updatedOldLoanAccountingMapingDto = _mapper.Map<OldLoanAccountingMapingDto>(existingOldLoanAccountingMaping);
                _logger.LogInformation(successMessage);
                return ServiceResponse<OldLoanAccountingMapingDto>.ReturnResultWith200(updatedOldLoanAccountingMapingDto);
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response
                string errorMessage = $"Error occurred while updating OldLoanAccountingMaping: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingMapping, LogLevelInfo.Error);
                return ServiceResponse<OldLoanAccountingMapingDto>.Return500(errorMessage);
            }
        }
    }

}

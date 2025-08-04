using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.Repository;
using CBS.NLoan.Helper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.Repository.LoanApplicationP;

namespace CBS.NLoan.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a loan application (soft delete).
    /// Logs and audits the deletion process for accountability.
    /// </summary>
    public class DeleteLoanCommandHandler : IRequestHandler<DeleteLoanApplicationCommand, ServiceResponse<bool>>
    {
        private readonly ILoanApplicationRepository _LoanRepository; // Repository for accessing Loan data.
        private readonly ILogger<DeleteLoanCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow; // Unit of Work for managing database transactions.

        /// <summary>
        /// Constructor for initializing the DeleteLoanCommandHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loan data access.</param>
        /// <param name="mapper">Mapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public DeleteLoanCommandHandler(
            ILoanApplicationRepository LoanRepository,
            IMapper mapper,
            ILogger<DeleteLoanCommandHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanRepository = LoanRepository ?? throw new ArgumentNullException(nameof(LoanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        /// <summary>
        /// Handles the DeleteLoanApplicationCommand to delete a loan application.
        /// </summary>
        /// <param name="request">The command containing the Loan ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanApplicationCommand request, CancellationToken cancellationToken)
        {
            string correlationId = Guid.NewGuid().ToString(); // Unique tracking ID for logging
            try
            {
                // 🔍 Step 1: Check if the Loan entity with the specified ID exists
                var existingLoan = await _LoanRepository.FindAsync(request.Id);
                if (existingLoan == null)
                {
                    string notFoundMessage = $"[WARNING] Loan with ID {request.Id} not found. CorrelationId: '{correlationId}'";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.Delete, LogLevelInfo.Warning, "System", "N/A", correlationId);
                    return ServiceResponse<bool>.Return404(notFoundMessage);
                }

                // 🛠 Step 2: Perform Soft Delete (Mark Loan as Deleted)
                _LoanRepository.Remove(existingLoan);
                await _uow.SaveAsync();

                // ✅ Step 3: Log and Audit the Deletion
                string successMessage = $"[SUCCESS] Loan application with ID {request.Id} has been successfully deleted. CorrelationId: '{correlationId}'";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.Delete, LogLevelInfo.Information, "System", "N/A", correlationId);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception e)
            {
                // ❌ Step 4: Handle and Log Unexpected Errors
                string errorMessage = $"[ERROR] Failed to delete Loan (ID: {request.Id}). Error: {e.Message}. CorrelationId: '{correlationId}'";
                _logger.LogError(e, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Delete, LogLevelInfo.Error, "System", "N/A", correlationId);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}

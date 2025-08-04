using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Division based on DeleteDivisionCommand.
    /// </summary>
    public class DeleteDivisionCommandHandler : IRequestHandler<DeleteDivisionCommand, ServiceResponse<bool>>
    {
        private readonly IDivisionRepository _DivisionRepository; // Repository for accessing Division data.
        private readonly ILogger<DeleteDivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteDivisionCommandHandler.
        /// </summary>
        /// <param name="DivisionRepository">Repository for Division data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDivisionCommandHandler(
            IDivisionRepository DivisionRepository, IMapper mapper,
            ILogger<DeleteDivisionCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _DivisionRepository = DivisionRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDivisionCommand to delete a Division.
        /// </summary>
        /// <param name="request">The DeleteDivisionCommand containing Division ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Division entity with the specified ID exists
                var existingDivision = await _DivisionRepository.AllIncluding(c => c.Subdivisions, cy => cy.Region).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (existingDivision == null)
                {
                    errorMessage = $"Division with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingDivision.IsDeleted = true;
                _DivisionRepository.Update(existingDivision);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Division: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
